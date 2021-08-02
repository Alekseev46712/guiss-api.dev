variable "asset_id" {
    description = "The asset ID"
    type        = "string"
}

variable "prefix" {
    description = "Prefix for Asset ID"
    type        = "string"
}

variable "name" {
    description = "Prefix for names the name of the API gateway"
    type        = "string"
}

variable "description" {
    description = "Description for the API gateway (displayed in AWS Console)"
    type        = "string"
    default     = ""
}

variable "deploy_name" {
  description = "Deployment name for the API gateway"
  type        = "string"
  default     = "main"
}

variable "tags" {
  description = "Tags to be applied to the Lambda and its IAM role"
  type        = "map"
}

#variable "gateway_policy" {
#  description = "Resource policy to restrict access to the API Gateway"
#  type = "string"
#  default = ""
#}
variable "gateway_whitelist" {
  description = "Resource policy to restrict access to the API Gateway"
  type = list(string)
}


variable "uri" {
  description = "The Lambda input URI"
  type = "string"
}

variable "function_name" {
  description = "The Lambda function name"
  type = "string"
}

variable "lambda_alias_name" {
  type = "string"
}

variable "api_gateway_custom_domain" {
    type        = "string"
}

variable "api_gateway_hostname" {
    type        = "string"
}

variable "api_gateway_domain" {
    type        = "string"
}

variable "xray_tracing_enabled" {
    type = bool
    default = false
}

variable "log_group_prefix" {
  description = "Prefix for the log group name. Defaults to /aws/lambda."
  type = string
  default = "/aws/api-gateway/"
}

variable "metrics_enabled" {
  type = bool
  default = true
}

variable "aws_region" {
   type = string
}

variable "log_group_arn" {
  type = string
}

variable "name_suffix" {
    type = string
}
