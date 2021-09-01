module "dynamodb_global" {
  db_install               = var.db_install
  db_multiregion           = var.db_multiregion
  source                   = "../modules/dynamodb"
  name                     = "a${var.asset_id}-db-${var.lambda_name}-${var.name_suffix}"
  billing_mode             = var.db_billing_mode
  read_capacity            = var.db_read_capacity
  write_capacity           = var.db_write_capacity
  hash_key                 = var.db_hash_key
  range_key                = var.db_range_key
  server_side_encryption   = var.db_encryption
  attributes               = var.db_attributes
  global_secondary_indexes = var.db_global_secondary_indexes
  aws_profile              = var.aws_profile
  tags                     = local.tags
}


# Variables
variable "db_install" {
  type        = bool
}
variable "db_multiregion" {
  type        = bool
}
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
