resource "aws_dynamodb_table" "user-attributes" {
  name           = "a250395_UserAttributes"
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
    name = "Name"
    type = "S"
  }

  ttl {
    attribute_name = "TimeToExist"
    enabled        = false
  }

  tags = {
    Name                              = "a250395_UserAttributes"
    "tr:application-asset-insight-id" = "250395"
    "tr:environment-name"             = "DEVELOPMENT"
    "tr:resource-owner"               = "AAADevelopmentNottingham2@thomson.com"
  }
}