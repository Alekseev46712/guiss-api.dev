# Provider
variable "aws_region" {
  type = string
}

variable "aws_profile" {
  type = string
}

# Shared
variable "asset_id" {
  type = string
}

variable "environment" {
  type = string
}

variable "resource_owner" {
  type = string
}

variable "app_version_number" {
  type = string
}

variable "deploy_name" {
  type = string
}

variable "name_suffix" {
  type = string
}

# Lambda
variable "lambda_name" {
  type = string
}

variable "filename" {
  type = string
}

variable "handler" {
  type = string
}

variable "memory_size" {
  type = string
}

variable "log_level" {
  type        = string
  default     = "Warning"
}

variable "log_target" {
  type        = "string"
  default     = "CloudWatch"
}

variable "publish" {
  type = bool
}

variable "logging_identity_pool_id" {
  type = string
}

variable "lambda_alias_current" {
  type = string
}

variable "lambda_description" {
  type = string
}

# IAM Role
variable "iam_role_name" {
  type = string
}

variable "policy_name" {
    type = string
}

variable "sqs_policy_name" {
  type = string
}

variable "ssm_policy_name" {
  type = string
}

variable "sns_policy_name" {
  type = string
}

variable "replay_dynamodb_table_arn" {
  type = string
}

# API-Gateway
variable "apigateway_name" {
  type = string
}

variable "api_gateway_description" {
  type = string
}

variable "should_create_custom_domainname" {
  type = number
}

variable "domain_name" {
  type = string
}

variable "certificate_arn" {
  type = string
}

variable "production_stage_name" {
  type = string
}

variable "target_hosted_zone_name" {
  type = string
}

variable "type" {
  type = string
}

# DynamoDB
variable "dynamodb_name" {
  type = string
}

variable "billing_mode" {
  type = string
}

variable "read_capacity" {
  type = number
}

variable "write_capacity" {
  type = number
}

variable "secondary_read_capacity" {
  type = number
}

variable "secondary_write_capacity" {
  type = number
}

variable "audit_logger_lambda_arn" {
  type = string
}
variable "stream_enabled" {
  description = "Stream enabled feature"
  type        = bool
}

variable "stream_view_type" {
  description = "Stream view type feature"
  type        = string
}
# Route53
variable "should_create_record_set" {
  type = string
}

# Alarms and filters
variable "alarms" {
  description = "Configuration for multiple alarms"
  type        = any
  default     = {}
}

variable "filters" {
  description = "Configuration for the alarm filters"
  type        = any
  default     = {}
}

variable "alarm_topic_name" {
  type        = string
  default     = "compass-alarm-notification"
  description = "Name of SNS topic to be used for DynamoDB alarms"
}

variable "dynamodb_alarm_thresholds" {
  type        = object({ pending_replication = number, read_throttle = number, write_throttle = number, system_errors = number })
  description = "Threshold values that trigger an alarm on DynamoDB"
  default = {
    pending_replication = 300
    read_throttle       = 100
    write_throttle      = 100
    system_errors       = 10
  }
}

variable "routing_api_table_name" {
    description = "Name the Routing API DynamoDb table."
    type        = string
}

variable "routing_api_table_hash_key" {
    description = "Hash key of the Routing API DynamoDb table."
    type        = string
}

variable "routing_id" {
    description = "Id of the GuissAPI record in the Routing API DynamoDb table."
    type        = string
}

variable "should_update_routing_table" {
    type        = bool
}
