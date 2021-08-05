# Provider
aws_region       = "us-east-1"
aws_profile      = "aaa-sdlc-preprod"

# Shared
asset_id       = "250395"
environment    = "DEVELOPMENT"
resource_owner = "chris.brightman@refinitiv.com"
name_suffix    = "dev"
financial_id   = "520151002"
service_name   = "ciam-pingintegration-api"

# Lambda
lambda_name                 = "guiss-api"
filename                    = "../../guiss-api-"
handler                     = "Refinitiv.Aaa.GuissApi::Refinitiv.Aaa.GuissApi.LambdaEntryPoint::FunctionHandlerAsync"
memory_size                 = "512"
runtime                     = "dotnetcore3.1"
lambda_timeout              = "15"
publish                     = true
lambda_alias_current        = "current_version"
lambda_description          = "Automated deployment of Guiss API Lambda"
aws_vpc                     = "fr-vpc-1"
lambda_subnets              = ["aaa-sdlc-preprod-enterprise-us-east-1b"]
lambda_security_groups      = ["default"]
lambda_env_vars             = {
  "AppSettings__DynamoDb__DefaultQueryLimit"     = "50",
  "AppSettings__Services__UserApi"               = "https://aaa-users-dev.aaa-preprod.aws-int.thomsonreuters.com/develop"
  "Logging__IdentityPoolId"                      = "us-east-1:5e737732-975a-4c30-8b4b-1b31da87044d",
  "Logging__Target"                              = "CloudWatch",
  "Logging__LogLevel__Default"                   = "Debug",
  "ParameterStore__PaginationParameterStorePath" = "/Refinitiv.Aaa.GuissApi/DataProtection",
  "Swagger__Endpoint"                            = "/main/swagger/v1/swagger.json"
}

# Parameters
param_path = "Refinitiv.Aaa.GuissApi"
params = []
secure_params = []

# DynamoDB
db_billing_mode   = "PAY_PER_REQUEST"
db_read_capacity  = "0"
db_write_capacity = "0"
db_hash_key       = "UserUuid"
db_range_key      = "Name"
db_encryption     = true
db_attributes = [
    { name = "UserUuid", type = "S" },
    { name = "Name", type = "S" },
    { name = "Namespace", type = "S" }
  ]
db_global_secondary_indexes = [
    { name = "Name-index", hash_key = "Name", write_capacity = 0, read_capacity = 0, projection_type = "ALL" },
    { name = "Namespace-index", hash_key = "Namespace", write_capacity = 0, read_capacity = 0, projection_type = "ALL" }
  ]

# Route53
route53_hostname = "aaa-guiss-dev"
route53_domain   = "aaa-preprod.aws-int.thomsonreuters.com"

# API Gateway
api_gateway_custom_domain = 1
api_gateway_description = "Automated deployment of Guiss API-Gateway"

# The list of IP ranges below is copied from the "WebCorp" security group in tr-fr-preprod.
# 34.234.230.251, 3.214.233.172, 3.214.140.7 - nat gateway ips for aaa-sdlc-preprod in us-east-1
api_gateway_whitelist = ["10.0.0.0/8","159.220.0.0/16","159.42.0.0/16","163.231.0.0/16","164.57.0.0/16",
"167.68.0.0/16","192.165.208.0/20","198.179.137.0/24","198.80.128.0/18","199.224.128.0/17",
"203.191.132.0/24","206.197.182.88/32","84.18.160.0/19","34.250.63.0/24","52.31.174.229",
"34.234.230.251","3.214.233.172","3.214.140.7"]

# Alarms
alarms = {
  "guiss-api-critical" = {
    "namespace"           = "LogMetrics"
    "description"         = "A Critical issue occurred with Guiss-API Lambda"
    "comparison_operator" = "GreaterThanOrEqualToThreshold"
    "datapoints_to_alarm" = "1"
    "evaluation_periods"  = "1"
    "period"              = "300"
    "statistic"           = "Sum"
    "threshold"           = "1"
    "alarm_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "ok_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "insufficient_data_actions" = []
    "treat_missing_data"        = "notBreaching"
    "dimensions"                = {}
    "tags" = {}
  }
}

# Filters
filters = {
  "guiss-api-critical" = {
    "pattern"   = "Critical"
    "value"     = "1"
    "namespace" = "LogMetrics"
  }
}
