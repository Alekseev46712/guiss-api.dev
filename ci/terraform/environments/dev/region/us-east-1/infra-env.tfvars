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
handler                     = "<handler>"
memory_size                 = "512"
runtime                     = "dotnetcore3.1"
lambda_timeout              = "15"
publish                     = true
lambda_alias_current        = "current_version"
lambda_description          = "Automated deployment of Guiss API Lambda"
lambda_env_vars             = {
  "IsLoggingVerbose"             = "true"
}

# Parameters
param_path = "Guiss-API"
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
    { name = "Name", type = "S" }
  ]
db_global_secondary_indexes = [
    { name = "Name-index", hash_key = "Name", write_capacity = 10, read_capacity = 10, projection_type = "ALL" }
  ]
