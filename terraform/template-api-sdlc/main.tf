provider "aws" {
  region  = var.aws_region
  profile = var.aws_profile
  version = "~> 2.21"
  max_retries = 1 # Stops Terraform from hanging if your credentials have expired
}

terraform {
  backend "s3" {}
}

locals {
  tags = {
    "tr:application-asset-insight-id" = var.asset_id
    "tr:environment-type"             = var.environment
    "tr:resource-owner"               = var.resource_owner
  }
}

locals {
  abbreviations = lookup({"eu-west-1"="euw1", "us-east-1"="use1"}, var.aws_region, var.aws_region)
}

data "aws_caller_identity" "current" {}

# The list of IP ranges below is copied from the "WebCorp" security group in tr-fr-preprod.
data "aws_iam_policy_document" "refinitiv_ip_addresses_only" {
  statement {
    actions   = ["execute-api:Invoke"]
    resources = ["execute-api:/*"]
    effect    = "Allow"
    principals {
      type        = "*"
      identifiers = ["*"]
    }
    condition {
      test     = "IpAddress"
      variable = "aws:SourceIp"
      values = [
        "10.0.0.0/8",
        "159.220.0.0/16",
        "159.42.0.0/16",
        "163.231.0.0/16",
        "164.57.0.0/16",
        "167.68.0.0/16",
        "192.165.208.0/20",
        "198.179.137.0/24",
        "198.80.128.0/18",
        "199.224.128.0/17",
        "203.191.132.0/24",
        "206.197.182.88/32",
        "84.18.160.0/19",
        "34.250.63.0/24",
		"52.31.174.229"
      ]
    }
  }
}

module "template_api" {
  //source                    = "s3::https://s3-eu-west-1.amazonaws.com/a206104-aaa-sdlc-apis-notts/permanent-resources/aaa-shared-modules/APIs/modules/local-lambda"
  source                      = "../modules/local-lambda"
  prefix                      = "a${var.asset_id}"
  asset_id                    = var.asset_id
  name                        = "${var.lambda_name}-${var.name_suffix}"
  role_arn                    = module.template_api_role.arn
  description                 = var.lambda_description
  filename                    = "${var.filename}${var.app_version_number}.zip"
  //filename                  = "../../Refinitiv.Aaa.Group.Api/bin/Release/netcoreapp2.1/Refinitiv.Aaa.Group.Api.zip"
  handler                     = var.handler
  memory_size                 = var.memory_size
  publish                     = var.publish
  lambda_alias_current        = var.lambda_alias_current
  routing_api_table_name      = var.routing_api_table_name
  routing_api_table_hash_key  = var.routing_api_table_hash_key
  routing_id                  = var.routing_id
  environment_variables = {
    "AppSettings__DynamoDb__GuissTableName" = module.template_dynamodb_table.table_name
    "AppSettings__DynamoDb__ServiceUrl"      = ""
    "AppSettings__Sqs__QueueName"            = "a${var.asset_id}-aaa-api-${var.name_suffix}.fifo"
    "AppSettings__Sns__Topics__Guiss"        = "a${var.asset_id}_aaa_api_template_${var.name_suffix}"
    "AppSettings__Sns__ReplayTableName"      = var.deploy_name == "manual" ? "a${var.asset_id}-aaa-replay-dynamodb-dev" : "a${var.asset_id}-aaa-replay-dynamodb-${var.name_suffix}"
    "Logging__IdentityPoolId"                = var.logging_identity_pool_id
    "Logging__LoggingEndpoint"               = var.deploy_name == "manual" ? "a${var.asset_id}-logging-dev" :"a${var.asset_id}-logging-${var.name_suffix}"
    "Logging__LogLevel__Default"             = var.log_level
    "Logging__Target"                        = "${var.log_target}"
    "Swagger__Endpoint"                      = "/${var.deploy_name}/swagger/v1/swagger.json"
    "Version"                                = var.app_version_number
  }
  tags = "${local.tags}"
}

module "template_api_role" {
  source                     = "../modules/template-api-role"
  iam_role_name              = "a${var.asset_id}-${var.iam_role_name}-${local.abbreviations}-${var.name_suffix}-role"

  policy_name                = "a${var.asset_id}-${var.policy_name}-${var.name_suffix}-policy" 
  template_dynamodb_table_arn  = module.template_dynamodb_table.table_arn
  sqs_policy_name            = "a${var.asset_id}-${var.sqs_policy_name}-${var.name_suffix}-policy" 
  sqs_name                   = "a${var.asset_id}-aaa-api-${var.name_suffix}.fifo"
  ssm_policy_name            = "a${var.asset_id}-${var.ssm_policy_name}-${var.name_suffix}-policy"
  sns_policy_name            = "a${var.asset_id}-${var.sns_policy_name}-${var.name_suffix}-policy"
  sns_topics_list            = ["a${var.asset_id}_aaa_api_template_${var.name_suffix}"]
  replay_dynamodb_table_arn  = var.replay_dynamodb_table_arn
  aws_account                = data.aws_caller_identity.current.account_id
}

module "template_api_gateway" {
  //source             = "s3::https://s3-eu-west-1.amazonaws.com/a206104-aaa-sdlc-apis-notts/permanent-resources/aaa-shared-modules/APIs/modules/api-gateway"
  source            = "../modules/api-gateway"
  prefix            = "a${var.asset_id}"
  asset_id          = var.asset_id
  name              = "${var.apigateway_name}-${var.name_suffix}"
  description       = var.api_gateway_description
  uri               = module.template_api.lambda_alias_current_invoke_arn
  function_name     = module.template_api.lambda_name
  deploy_name       = var.deploy_name
  gateway_policy    = data.aws_iam_policy_document.refinitiv_ip_addresses_only.json
  lambda_alias_name = var.lambda_alias_current
  tags              = local.tags

  should_create_custom_domainname = var.should_create_custom_domainname
  domain_name                     = "${var.domain_name}-${var.name_suffix}.${var.target_hosted_zone_name}"
  certificate_arn                 = var.certificate_arn
  production_stage_name           = var.production_stage_name
}

module "template_dynamodb_table" {
  source                   = "../modules/template-dynamodb-table"
  prefix                   = "a${var.asset_id}"
  name                     = "${var.dynamodb_name}-${var.name_suffix}"
  billing_mode             = var.billing_mode
  read_capacity            = var.read_capacity
  write_capacity           = var.write_capacity
  secondary_read_capacity  = var.secondary_read_capacity
  secondary_write_capacity = var.secondary_write_capacity
  tags                     = local.tags
  stream_enabled           = var.stream_enabled
  stream_view_type         = var.stream_view_type
}

module "dynamodb_alarms" {
  source           = "../modules/dynamodb-alarms"
  table_name       = module.template_dynamodb_table.table_name
  is_global_table  = false
  alarm_topic_name = var.alarm_topic_name
  thresholds       = var.dynamodb_alarm_thresholds
  tags             = local.tags
}

data "aws_route53_zone" "hosted_zone" {
  name         = var.target_hosted_zone_name
  private_zone = true
}

module "template_route53_record" {
  source                     = "../modules/route53"
  should_create_record_set   = var.should_create_record_set
  target_hosted_zone_id      = data.aws_route53_zone.hosted_zone.zone_id
  domain_name                = "${var.domain_name}-${var.name_suffix}.${var.target_hosted_zone_name}"
  type                       = var.type
  alias_regional_domain_name = module.template_api_gateway.targeted_domain_name
  alias_regional_zone_id     = module.template_api_gateway.custom_domain_id
}

module "template_alarms" {
  source      = "../modules/metric-alarm"
  alarms      = var.alarms
  filters     = var.filters
  asset_id    = var.asset_id
  name_suffix = var.name_suffix
  tags        = local.tags
  group_name  = module.template_api.log_group_name
}

module "routing_table_update" {
  source                         = "../modules/routing-table-details"
  routing_api_table_name         = var.routing_api_table_name
  routing_api_table_hash_key     = var.routing_api_table_hash_key
  routing_id                     = var.routing_id
  lambda_function_arn            = "${module.template_api.lambda_alias_current_arn}"
  should_update_routing_table    = var.should_update_routing_table
}

resource "aws_lambda_event_source_mapping" "audit_trigger" {
  event_source_arn  = module.template_dynamodb_table.stream_arn
  function_name     = var.audit_logger_lambda_arn
  starting_position = "LATEST"
}
