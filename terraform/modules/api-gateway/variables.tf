variable "asset_id" {
    description = "The asset ID"
    type        = string
}

variable "prefix" {
    description = "Prefix for Asset ID"
    type        = string
}

variable "name" {
    description = "Prefix for names the name of the API gateway"
    type        = string
}

variable "description" {
    description = "Description for the API gateway (displayed in AWS Console)"
    type        = string
    default     = ""
}

variable "deploy_name" {
  description = "Deployment name for the API gateway"
  type        = string
}

variable "tags" {
  description = "Tags to be applied to the Lambda and its IAM role"
  type        = map
}

variable "gateway_policy" {
  description = "Resource policy to restrict access to the API Gateway"
  type = string
  default = ""
}

variable "uri" {
  description = "The Lambda input URI"
  type = string
}

variable "function_name" {
  description = "The Lambda function name"
  type = string
}

variable "lambda_alias_name" {
  type = string
}

variable "should_create_custom_domainname" {
    type        = string
}

variable "domain_name" {
    type        = string
}

variable "certificate_arn" {
    type        = string
}

variable "production_stage_name" {
    type        = string
}