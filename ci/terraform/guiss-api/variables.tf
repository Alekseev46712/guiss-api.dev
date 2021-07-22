# Provider
variable "aws_region" {
  type        = string
}

variable "aws_profile" {
  type        = string
}

# Shared
variable "asset_id" {
  type        = string
}

variable "name_suffix" {
  type        = string
}

variable "environment" {
  type        = string
  description = "Environment type for tagging purposes: PRODUCTION, PRE-PRODUCTION, QUALITY ASSURANCE, INTEGRATION TESTING, DEVELOPMENT or LAB."
}

variable "resource_owner" {
  type        = string
}

variable "service_name" {
  type        = string
}

variable "financial_id" {
  type        = string
}

# Lambda
variable "lambda_name" {
  type        = string
}

variable "filename" {
  type        = string
}

variable "handler" {
  type        = string
}

variable "memory_size" {
  type        = string
}

variable "runtime" {
  description = "Name of the runtime. Defaults to 'dotnetcore2.1'."
  type        = string
  default     = "dotnetcore2.1"
}

variable "lambda_timeout" {
  type        = string
  default     = "3"
}

variable "publish" {
  type        = bool
}

variable "lambda_alias_current" {
  type        = string
}

variable "lambda_description" {
  type        = string
}

variable "app_version_number" {
  type        = string
}

variable "lambda_env_vars" {
  type        = map
  default     = {}
}

variable "param_path" {
  type        = string
}

# Alarms
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

variable "log_target" {
  type        = string
  default     = "CloudWatch"
}

# Parameters
variable "params" {
  type = any
}

variable "secure_params" {
  type = any
}

# DB
variable "db_billing_mode" {
  type        = string
}
variable "db_read_capacity" {
  type        = string
}
variable "db_write_capacity" {
  type        = string
}
variable "db_hash_key" {
  type        = string
}
variable "db_range_key" {
  type        = string
}
variable "db_encryption" {
  type        = bool
}
variable "db_attributes" {
  type        = any
}
variable "db_global_secondary_indexes" {
  type        = any
}
