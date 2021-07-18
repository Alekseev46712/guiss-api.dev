variable "should_update_routing_table" {
    type        = bool
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
    description = "Id of the Users API record in the Routing API DynamoDb table."
    type        = string
}

variable "lambda_function_arn" {
    description = "ARN of the Users API Lambda."
    type        = string
}
