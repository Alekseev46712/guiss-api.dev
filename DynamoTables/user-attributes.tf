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
  
  attribute {
    name = "Namespace"
    type = "S"
  }

  ttl {
    attribute_name = "TimeToExist"
    enabled        = false
  }

  global_secondary_index {
    name            = "Name-index"
    hash_key        = "Name"
    write_capacity  = 10
    read_capacity   = 10
    projection_type = "ALL"
  }
  
  global_secondary_index {
    name            = "Namespace-index"
    hash_key        = "Namespace"
    write_capacity  = 10
    read_capacity   = 10
    projection_type = "ALL"
  }
  
  tags = {
    Name                              = "a250395_UserAttributes"
    "tr:application-asset-insight-id" = "250395"
    "tr:environment-name"             = "DEVELOPMENT"
    "tr:resource-owner"               = "AAADevelopmentNottingham2@thomson.com"
  }
}
