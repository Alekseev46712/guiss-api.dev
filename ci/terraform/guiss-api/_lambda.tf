module "lambda_role" {
  source              = "../modules/lambda-role"
  iam_role_name       = "a${var.asset_id}-role-${var.lambda_name}-${var.name_suffix}-${local.abbreviations}"
  aws_account         = data.aws_caller_identity.current.account_id
  tags                = local.tags
}

module "lambda" {
  source                          = "../modules/lambda"
  prefix                          = local.prefix
  asset_id                        = var.asset_id
  name                            = local.lambda_name_suffix
  role_arn                        = module.lambda_role.arn
  description                     = var.lambda_description
  filename                        = "${var.filename}${var.app_version_number}.zip"
  runtime                         = var.runtime
  handler                         = var.handler
  memory_size                     = var.memory_size
  timeout                         = var.lambda_timeout
  publish                         = var.publish
  lambda_alias_current            = var.lambda_alias_current
  environment_variables           = merge(var.lambda_env_vars, {
                                      "AWS__ParameterStorePath" = "/${local.prefix}/${var.name_suffix}/${var.param_path}",
                                      "Version"                 = var.app_version_number
                                    })
  tags                            = local.tags
}
