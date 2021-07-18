resource "aws_api_gateway_rest_api" "api_gateway" {
  name        = "${var.prefix}-${var.name}"
  description = var.description
  policy      = var.gateway_policy
}

resource "aws_api_gateway_resource" "proxy" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  parent_id   = aws_api_gateway_rest_api.api_gateway.root_resource_id
  path_part   = "{proxy+}"
}

resource "aws_api_gateway_method" "proxy" {
  rest_api_id   = aws_api_gateway_rest_api.api_gateway.id
  resource_id   = aws_api_gateway_resource.proxy.id
  http_method   = "ANY"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "integration_resource" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  resource_id = aws_api_gateway_method.proxy.resource_id
  http_method = aws_api_gateway_method.proxy.http_method

  content_handling        = "CONVERT_TO_TEXT"

  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = var.uri
}

resource "aws_api_gateway_method" "proxy_root" {
  rest_api_id   = aws_api_gateway_rest_api.api_gateway.id
  resource_id   = aws_api_gateway_rest_api.api_gateway.root_resource_id
  http_method   = "ANY"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "integration_root" {
  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  resource_id = aws_api_gateway_method.proxy_root.resource_id
  http_method = aws_api_gateway_method.proxy_root.http_method

  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = var.uri
}

resource "aws_api_gateway_deployment" "deployment" {
  depends_on = [
    aws_api_gateway_integration.integration_resource,
    aws_api_gateway_integration.integration_root,
  ]

  rest_api_id = aws_api_gateway_rest_api.api_gateway.id
  stage_name  = var.deploy_name
  # Workaround for https://github.com/hashicorp/terraform/issues/6613 -
  # We need to force a redeploy if the gateway's resource policy changes.
  stage_description = md5(var.gateway_policy)
}

resource "aws_lambda_permission" "publisher_function_permission_for_apigateway_root" {
  action                        = "lambda:InvokeFunction"
  function_name                 = var.function_name
  principal                     = "apigateway.amazonaws.com"
  source_arn                    = "${aws_api_gateway_rest_api.api_gateway.execution_arn}/*/*/"
}

resource "aws_lambda_permission" "publisher_function_permission_for_apigateway" {
  action                        = "lambda:InvokeFunction"
  function_name                 = var.function_name
  principal                     = "apigateway.amazonaws.com"
  source_arn                    = "${aws_api_gateway_rest_api.api_gateway.execution_arn}/*/*/*"
}

resource "aws_lambda_permission" "publisher_function_alias_permission_for_apigateway_root" {
  action                        = "lambda:InvokeFunction"
  function_name                 = "${var.function_name}:${var.lambda_alias_name}"
  principal                     = "apigateway.amazonaws.com"
  source_arn                    = "${aws_api_gateway_rest_api.api_gateway.execution_arn}/*/*/"
  qualifier                     = var.lambda_alias_name
}

resource "aws_lambda_permission" "publisher_function_alias_permission_for_apigateway" {
  action                        = "lambda:InvokeFunction"
  function_name                 = "${var.function_name}:${var.lambda_alias_name}"
  principal                     = "apigateway.amazonaws.com"
  source_arn                    = "${aws_api_gateway_rest_api.api_gateway.execution_arn}/*/*/*"
  qualifier                     = var.lambda_alias_name
}

# API Domain

resource "aws_api_gateway_domain_name" "gateway_domain_name" {
    count                           = var.should_create_custom_domainname ? 1 : 0
    domain_name                     = var.domain_name
    certificate_arn                 = var.certificate_arn
}

resource "aws_api_gateway_base_path_mapping" "gateway_base_path_mapping" {
    count                           = var.should_create_custom_domainname ? 1 : 0
    api_id                          = aws_api_gateway_rest_api.api_gateway.id
    stage_name                      = var.production_stage_name
    domain_name                     = aws_api_gateway_domain_name.gateway_domain_name[0].domain_name
    depends_on                      = [
        aws_api_gateway_domain_name.gateway_domain_name
    ]
}
