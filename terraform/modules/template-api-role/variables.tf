variable "iam_role_name" {
    type = string
}

variable "policy_name" {
    type = string
}

variable "template_dynamodb_table_arn" {
    type = string
}

variable "replay_dynamodb_table_arn" {
    type = string
}

variable "sqs_policy_name" {
    type = string
}

variable "sqs_name" {
    type = string
}

variable "ssm_policy_name" {
    type = string
}

variable "sns_policy_name" {
  type = string
}

variable "sns_topics_list" {
  type = list(string)
}

variable "aws_account" {
    type = string
}
