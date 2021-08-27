resource "aws_api_gateway_rest_api" "api_gateway" {
  name        = "${var.prefix}-${var.name}"
  description = "${var.description}"
  endpoint_configuration {
     types            = [ "REGIONAL", ]
  }
  tags = var.tags
}

resource "aws_api_gateway_rest_api_policy" "api_gateway" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action    = [ "execute-api:Invoke" ]
        Effect    = "Allow"
        Resource  = "${aws_api_gateway_rest_api.api_gateway.execution_arn}/*"
        Principal = "*"
        Condition = {
          IpAddress = {
            "aws:SourceIp" = var.gateway_whitelist
          }
        }
      }
    ]
  })
}

resource "aws_api_gateway_resource" "proxy" {
  rest_api_id = "${aws_api_gateway_rest_api.api_gateway.id}"
  parent_id   = "${aws_api_gateway_rest_api.api_gateway.root_resource_id}"
  path_part   = "{proxy+}"
}

resource "aws_api_gateway_method" "proxy" {
  rest_api_id   = "${aws_api_gateway_rest_api.api_gateway.id}"
  resource_id   = "${aws_api_gateway_resource.proxy.id}"
  http_method   = "ANY"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "integration_resource" {
  rest_api_id = "${aws_api_gateway_rest_api.api_gateway.id}"
  resource_id = "${aws_api_gateway_method.proxy.resource_id}"
  http_method = "${aws_api_gateway_method.proxy.http_method}"

  content_handling        = "CONVERT_TO_TEXT"

  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = "${var.uri}"
}

resource "aws_api_gateway_method" "proxy_root" {
  rest_api_id   = "${aws_api_gateway_rest_api.api_gateway.id}"
  resource_id   = "${aws_api_gateway_rest_api.api_gateway.root_resource_id}"
  http_method   = "ANY"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "integration_root" {
  rest_api_id = "${aws_api_gateway_rest_api.api_gateway.id}"
  resource_id = "${aws_api_gateway_method.proxy_root.resource_id}"
  http_method = "${aws_api_gateway_method.proxy_root.http_method}"

  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = "${var.uri}"
}

resource "aws_api_gateway_deployment" "deployment" {
  depends_on = [
    "aws_api_gateway_integration.integration_resource",
    "aws_api_gateway_integration.integration_root",
  ]

  rest_api_id = "${aws_api_gateway_rest_api.api_gateway.id}"
  stage_name  = "${var.deploy_name}"
  # Workaround for https://github.com/hashicorp/terraform/issues/6613 -
  # We need to force a redeploy if the gateway's resource policy changes.
  #stage_description = "${md5(var.gateway_policy)}"
  
  # We need to force a redeploy if the lambda's name changes.
  variables = {
    "lambda_name" = "${var.function_name}"
  }
  lifecycle {
    create_before_destroy = true
  }

}

resource "aws_lambda_permission" "publisher_function_permission_for_apigateway_root" {
  action                        = "lambda:InvokeFunction"
  function_name                 = "${var.function_name}"
  principal                     = "apigateway.amazonaws.com"
  source_arn                    = "${aws_api_gateway_rest_api.api_gateway.execution_arn}/*/*/"
}

resource "aws_lambda_permission" "publisher_function_permission_for_apigateway" {
  action                        = "lambda:InvokeFunction"
  function_name                 = "${var.function_name}"
  principal                     = "apigateway.amazonaws.com"
  source_arn                    = "${aws_api_gateway_rest_api.api_gateway.execution_arn}/*/*/*"
}

resource "aws_lambda_permission" "publisher_function_alias_permission_for_apigateway_root" {
  action                        = "lambda:InvokeFunction"
  function_name                 = "${var.function_name}"
  principal                     = "apigateway.amazonaws.com"
  source_arn                    = "${aws_api_gateway_rest_api.api_gateway.execution_arn}/*/*/"
  qualifier                     = "${var.lambda_alias_name}"
}

resource "aws_lambda_permission" "publisher_function_alias_permission_for_apigateway" {
  action                        = "lambda:InvokeFunction"
  function_name                 = "${var.function_name}"
  principal                     = "apigateway.amazonaws.com"
  source_arn                    = "${aws_api_gateway_rest_api.api_gateway.execution_arn}/*/*/*"
  qualifier                     = "${var.lambda_alias_name}"
}

# API Domain
data "aws_acm_certificate" "issued" {
  count                           = "${var.api_gateway_custom_domain ? 1 : 0}"

  domain      = "*.${var.api_gateway_domain}"
  statuses    = ["ISSUED"]
  most_recent = true
}

resource "aws_api_gateway_domain_name" "gateway_domain_name" {
  count                           = "${var.api_gateway_custom_domain ? 1 : 0}"

  domain_name                     = "${var.api_gateway_hostname}.${var.api_gateway_domain}"
  regional_certificate_arn        = data.aws_acm_certificate.issued[0].arn
  endpoint_configuration {
    types = ["REGIONAL"]
  }
}

resource "aws_api_gateway_base_path_mapping" "gateway_base_path_mapping" {
  count                           = "${var.api_gateway_custom_domain ? 1 : 0}"

  api_id                          = aws_api_gateway_rest_api.api_gateway.id
  stage_name                      = aws_api_gateway_deployment.deployment.stage_name
  domain_name                     = aws_api_gateway_domain_name.gateway_domain_name[0].domain_name
  depends_on                      = [
        aws_api_gateway_domain_name.gateway_domain_name
    ]
}

resource "aws_api_gateway_method_settings" "method_settings" {
  depends_on  = [aws_api_gateway_deployment.deployment]
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  stage_name  = var.deploy_name
  method_path = "*/*"

  settings {
    metrics_enabled = var.metrics_enabled
    logging_level   = "INFO"
    throttling_burst_limit  = 5000
   throttling_rate_limit    = 10000
    
  }
}

#resource "null_resource" "update_deployment_stage" {
#    triggers = {
#      metrics_enabled = var.metrics_enabled
#    }
#
#  provisioner "local-exec" {
#    command = "aws apigateway update-stage --region ${var.aws_region} --rest-api-id ${aws_api_gateway_rest_api.api_gateway.id} --stage-name ${var.deploy_name} --patch-operations op=replace,path=/tracingEnabled,value=${self.triggers.metrics_enabled}"
#  }
#
#  provisioner "local-exec" {
#    command = "aws apigateway update-stage --region ${var.aws_region} --rest-api-id ${aws_api_gateway_rest_api.api_gateway.id} --stage-name ${var.deploy_name} --patch-operations op=replace,path=/accessLogSettings/destinationArn,value=${var.log_group_arn}"
#  }
#}


resource "aws_cloudwatch_log_group" "log_group" {
  name              = "${var.log_group_prefix}${var.prefix}-${var.name}"
  retention_in_days = 365
  tags              = var.tags
}

#
#resource "aws_xray_sampling_rule" "Users_Api" {
#  rule_name      = "${var.prefix}-users-api-${var.name_suffix}"
#  priority       = 9996
#  version        = 1
#  reservoir_size = 1
#  fixed_rate     = 0.05
#  url_path       = "*"
#  host           = "*"
#  http_method    = "*"
#  service_type   = "*"
#  service_name   = "users"
#  resource_arn   = "*"
#
#  attributes = {}
#}
