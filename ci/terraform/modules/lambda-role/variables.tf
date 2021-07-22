variable "iam_role_name" {
  type = string
}

variable "aws_account" {
  type = string
}

variable "tags" {
    description = "Tags to be applied to the Lambda and its IAM role"
    type        = "map"
}