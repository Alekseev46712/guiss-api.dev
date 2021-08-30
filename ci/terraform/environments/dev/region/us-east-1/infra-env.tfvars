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
  "AppSettings__DynamoDb__UserAttributeTableName"            = "a250395-db-guiss-api-dev",
  "AppSettings__DynamoDb__DefaultQueryLimit"                 = "50",
  "AppSettings__Services__UserApi"                           = "https://aaa-users-dev.aaa-preprod.aws-int.thomsonreuters.com/develop",
  "AppSettings__Elasticache__Hostname"                       = "a250395-memcached-guiss-api-dev-use1.xazhrl.cfg.use1.cache.amazonaws.com",
  "AppSettings__Elasticache__Port"                           = "11211",
  "Logging__IdentityPoolId"                                  = "us-east-1:5e737732-975a-4c30-8b4b-1b31da87044d",
  "Logging__Target"                                          = "CloudWatch",
  "Logging__LogLevel__Default"                               = "Debug",
  "ParameterStore__UserAttributeApiConfigParameterStoreName" = "UserAttributeApiConfig",
  "Swagger__Endpoint"                                        = "/swagger/v1/swagger.json"
}

# Parameters
param_path = "Refinitiv.Aaa.GuissApi"
params = [
  "UserAttributeApiConfig"
]
secure_params = []

# DynamoDB
db_install        = true
db_multiregion    = true
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

# Elasticache
ec_type            = "cache.t3.micro"
ec_subnet_group    = "els-subnetgroup-aaa-sdlc-preprod-us-east-1"
ec_security_groups = ["fromBastionMemCached"]

# Route53
route53_hostname = "aaa-guiss-dev"
route53_domain   = "aaa-preprod.aws-int.thomsonreuters.com"

# API Gateway
api_gateway_custom_domain = 1
api_gateway_description   = "Automated deployment of Guiss API-Gateway"

# The list of IP ranges below is copied from the "WebCorp" security group in tr-fr-preprod.
# 34.234.230.251, 3.214.233.172, 3.214.140.7 - nat gateway ips for aaa-sdlc-preprod in us-east-1
# 34.251.88.0 54.76.122.132 52.48.210.115 - nat gateway ips of concourse ci/cd
# 15.177.0.0/18 ...  - route53 healthcheck ips
api_gateway_whitelist = ["10.0.0.0/8","159.220.0.0/16","159.42.0.0/16","163.231.0.0/16","164.57.0.0/16",
"167.68.0.0/16","192.165.208.0/20","198.179.137.0/24","198.80.128.0/18","199.224.128.0/17",
"203.191.132.0/24","206.197.182.88/32","84.18.160.0/19","34.250.63.0/24","52.31.174.229",
"34.234.230.251","3.214.233.172","3.214.140.7",
"34.251.88.0","54.76.122.132","52.48.210.115",
"15.177.0.0/18","52.80.197.0/25","52.80.197.128/25","52.80.198.0/25","52.83.34.128/25","52.83.35.0/25","52.83.35.128/25",
"54.248.220.0/26","54.250.253.192/26","54.251.31.128/26","54.255.254.192/26","54.252.254.192/26","54.252.79.128/26",
"176.34.159.192/26","54.228.16.0/26","177.71.207.128/26","54.232.40.64/26","107.23.255.0/26","54.243.31.192/26",
"54.183.255.128/26","54.241.32.64/26","54.244.52.192/26","54.245.168.0/26"]

# Alarms
alarms = {
  "guiss-api-critical" = {
    "namespace"           = "LogMetrics"
    "metric"              = "guiss-api-critical-dev"
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
  },
  "db-guiss-api-read-throttle" = {
    "namespace"           = "AWS/DynamoDB"
    "metric"              = "ReadThrottleEvents"
    "description"         = "DynamoDB Table Read Throttle Events"
    "comparison_operator" = "GreaterThanThreshold"
    "datapoints_to_alarm" = "0"
    "evaluation_periods"  = "3"
    "period"              = "60"
    "statistic"           = "SampleCount"
    "threshold"           = "100"
    "alarm_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "ok_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "insufficient_data_actions" = []
    "treat_missing_data"        = "notBreaching"
    "dimensions"                = {
      "TableName" = "a250395-db-guiss-api-dev"
    }
    "tags" = {}
  },
  "db-guiss-api-write-throttle" = {
    "namespace"           = "AWS/DynamoDB"
    "metric"              = "WriteThrottleEvents"
    "description"         = "DynamoDB Table Write Throttle Events"
    "comparison_operator" = "GreaterThanThreshold"
    "datapoints_to_alarm" = "0"
    "evaluation_periods"  = "3"
    "period"              = "60"
    "statistic"           = "SampleCount"
    "threshold"           = "100"
    "alarm_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "ok_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "insufficient_data_actions" = []
    "treat_missing_data"        = "notBreaching"
    "dimensions"                = {
      "TableName" = "a250395-db-guiss-api-dev"
    }
    "tags" = {}
  },
  "db-guiss-api-system-errors" = {
    "namespace"           = "AWS/DynamoDB"
    "metric"              = "SystemErrors"
    "description"         = "DynamoDB Table System Errors"
    "comparison_operator" = "GreaterThanThreshold"
    "datapoints_to_alarm" = "0"
    "evaluation_periods"  = "3"
    "period"              = "60"
    "statistic"           = "SampleCount"
    "threshold"           = "10"
    "alarm_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "ok_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "insufficient_data_actions" = []
    "treat_missing_data"        = "notBreaching"
    "dimensions"                = {
      "TableName" = "a250395-db-guiss-api-dev"
    }
    "tags" = {}
  },
  "db-guiss-api-replication" = {
    "namespace"           = "AWS/DynamoDB"
    "metric"              = "PendingReplicationCount"
    "description"         = "DynamoDB Table Replication Errors"
    "comparison_operator" = "GreaterThanThreshold"
    "datapoints_to_alarm" = "0"
    "evaluation_periods"  = "3"
    "period"              = "60"
    "statistic"           = "SampleCount"
    "threshold"           = "300"
    "alarm_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "ok_actions" = [
      "arn:aws:sns:us-east-1:653551970210:compass-alarm-notification"
    ]
    "insufficient_data_actions" = []
    "treat_missing_data"        = "notBreaching"
    "dimensions"                = {
      "TableName" = "a250395-db-guiss-api-dev"
    }
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
