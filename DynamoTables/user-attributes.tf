resource "aws_dynamodb_table" "user-attributes" {
  name           = "a206104_UserAttributes"
  billing_mode   = "PROVISIONED"
  read_capacity  = 10
  write_capacity = 10
  hash_key       = "UserUuid"
  range_key      = "Name"

  attribute {
    name = "UserUuid"
    type = "S"
  }
  
  attribute {
    name = "SearchUserUuid"
    type = "S"
  }

  attribute {
    name = "Name"
    type = "S"
  }

  ttl {
    attribute_name = "TimeToExist"
    enabled        = false
  }
  
  global_secondary_index {
    name            = "UserUuid-index"
    hash_key        = "SearchUserUuid"
    write_capacity  = 10
    read_capacity   = 10
    projection_type = "ALL"
  }

  tags = {
    Name                              = "a206104_UserAttributes"
    "tr:application-asset-insight-id" = "206104"
    "tr:environment-name"             = "DEVELOPMENT"
    "tr:resource-owner"               = "AAADevelopmentNottingham2@thomson.com"
  }
}