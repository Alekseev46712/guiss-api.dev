# Provider
aws_region       = "us-east-1"
aws_profile      = "aaa-sdlc-preprod"

# Shared
asset_id                    = "206104"
environment                 = "DEVELOPMENT"
resource_owner              = "AAADevelopmentNottingham2@refinitiv.com"
name_suffix                 = "dev"

#app_version_number         = ""
deploy_name                 = "develop"

# Lambda
lambda_name                 = "aaa-template-api"
filename                    = "../../Refinitiv.Aaa.GuissApi-"
handler                     = "Refinitiv.Aaa.GuissApi::Refinitiv.Aaa.GuissApi.LambdaEntryPoint::FunctionHandlerAsync"
memory_size                 = "1024"
publish                     = true
logging_identity_pool_id    = "us-east-1:5e737732-975a-4c30-8b4b-1b31da87044d"
lambda_alias_current        = "current_version"
lambda_description          = "Automated deployment of Guiss API Lambda"
routing_api_table_name      = "a206104-aaa-routing-api-dev-table"
routing_api_table_hash_key  = "Id"
routing_id        			= "templateapi"
should_update_routing_table = true

# IAM Role
iam_role_name               = "aaa-template-lambda"
policy_name                 = "template-dynamodb"
sqs_policy_name             = "template-sqs"
ssm_policy_name             = "template-ssm"
sns_policy_name             = "template-sns"
logging_policy_name         = "template-logging"
replay_dynamodb_table_arn   = "arn:aws:dynamodb:us-east-1:653551970210:table/a206104-aaa-replay-dynamodb-dev"

# API-Gateway
apigateway_name                  = "aaa-template-api-gateway"
api_gateway_description          = "Automated deployment of Guiss AWS API-Gateway "


should_create_custom_domainname  = 0
domain_name                      = "aaa-template"
certificate_arn                  = "arn:aws:acm:us-east-1:653551970210:certificate/e8ae8779-d41a-4cff-8d04-62ab937c8aea"
production_stage_name            = ""
target_hosted_zone_name          = "aaa-preprod.aws-int.thomsonreuters.com"
type                             = "A"

# DynamoDB
dynamodb_name               = "aaa-template-dynamodb"
billing_mode                = "PAY_PER_REQUEST"
read_capacity               = 0
write_capacity              = 0
secondary_read_capacity     = 0
secondary_write_capacity    = 0
audit_logger_lambda_arn     = "arn:aws:lambda:us-east-1:653551970210:function:a206104-aaa-auditing-lambda-dev"
stream_enabled              = true
stream_view_type            = "NEW_AND_OLD_IMAGES"

# Route53
should_create_record_set    = 0
# Alarms
alarms = {
  "GuissApiCritical" = {
      "namespace" = "LogMetrics"
      "description" = "A Critical issue occurred during an API execution. See the AAA runbook for recommended actions"
      "comparison_operator"       = "GreaterThanOrEqualToThreshold"
      "datapoints_to_alarm"       = "1"
      "evaluation_periods"        = "1"
      "period"                    = "300"
      "statistic"                 = "Sum"
      "threshold"                 = "1"
      "alarm_actions"             = [
          "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification",
          "arn:aws:sns:us-east-1:653551970210:a206104-aaa-support-topic-dev"
      ]
      "ok_actions"                = [
          "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification",
          "arn:aws:sns:us-east-1:653551970210:a206104-aaa-support-topic-dev"
      ]
      "insufficient_data_actions" = []
      "treat_missing_data"        = "missing"
      "dimensions"                = {}
      "tags"                      = {
        "tr:project-name" = "AAA-ENTITLEMENTS"
        "tr:service-name" = "a206104-aaa-template-api-dev"
      }
  }
}

# Filters
filters = {
  "GuissApiCritical" = {
    "pattern" = "CRITICAL"
    "value" = "1"
    "namespace" = "LogMetrics"
  }
}
