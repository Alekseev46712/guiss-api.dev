module "api_gateway" {
  source            = "../modules/api-gateway"
  prefix            = local.prefix
  asset_id          = var.asset_id
  name              = "${var.lambda_name}-${var.name_suffix}-${local.abbreviations}"
  name_suffix       = var.name_suffix
  description       = var.api_gateway_description
  uri               = "arn:aws:apigateway:${var.aws_region}:lambda:path/2015-03-31/functions/${module.lambda.lambda_arn}/invocations"
  function_name     = module.lambda.lambda_name
  gateway_whitelist = var.api_gateway_whitelist 
  lambda_alias_name = var.lambda_alias_current
  tags              = local.tags
  aws_region        = var.aws_region
  log_group_arn     = "arn:aws:logs:${var.aws_region}:${data.aws_caller_identity.current.account_id}:log-group:/aws/api-gateway/${var.lambda_name}-${var.name_suffix}-${local.abbreviations}"

  api_gateway_custom_domain  = var.api_gateway_custom_domain
  api_gateway_hostname       = var.route53_hostname
  api_gateway_domain         = var.route53_domain
}


# Variable
variable "api_gateway_custom_domain" {
  type = string
}
variable "api_gateway_description" {
  type = string
}
variable "api_gateway_whitelist" {
  type = list(string)
}
