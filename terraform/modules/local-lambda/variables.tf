variable "prefix" {
    description = "Prefix for Asset ID"
    type        = string
}

variable "asset_id" {
    description = "Asset ID"
    type        = string
}

variable "name" {
    description = "Prefix for the name of the Lambda"
    type        = string
}

variable "description" {
    description = "Description for the Lambda (displayed in AWS Console)"
    type        = string
    default     = ""
}

variable "filename" {
    description = "The path to the function's deployment package within the local filesystem."
    type        = string
}

variable "handler" {
    description = "Name of the handler function in the code"
    type        = string
}

variable "runtime" {
    description = "Name of the runtime. Defaults to 'dotnetcore2.1'."
    type        = string
    default     = "dotnetcore2.1"
}

variable "memory_size" {
    description = "Memory allocation for the Lambda in Megabytes. Note: .NET Core needs minimum 256."
    type        = number
    default     = 256
}

variable "timeout" {
    description = "Timeout period in seconds"
    type        = number
    default     = 15
}

variable "role_arn" {
    description = "ARN of the role under which the Lambda should execute"
    type        = string
}

variable "environment_variables" {
    description = "Environment variables for the Lambda. Use to supply configuration information."
    type        = "map"
    default     = {}
}

variable "tags" {
    description = "Tags to be applied to the Lambda and its IAM role"
    type        = "map"
}

variable "subnet_ids" {
    description = "A list of subnet IDs associated with the Lambda function"
    type        = "list"
    default     = []
}

variable "security_group_ids" {
    description = "A list of security group IDs associated with the Lambda function"
    type        = "list"
    default     = []
}

variable "publish" {
    description = "Whether to publish creation/change as new Lambda Function Version."
    type        = bool
    default     = false
}

variable "lambda_alias_current" {
    description = "Name for the alias you are creating."
    type        = string
}

variable "log_group_prefix" {
  description = "Prefix for the log group name. Defaults to /aws/lambda."
  type = string
  default = "/aws/lambda/"
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

