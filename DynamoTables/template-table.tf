provider "aws" {
  access_key                  = "dummy key"
  secret_key                  = "dummy secret"
  region                      = "eu-west-1"
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true

  endpoints {
    dynamodb = "http://dynamodb:8000"
  }
}

resource "aws_dynamodb_table" "templates-table" {
  name           = "a206104_Guiss"
  billing_mode   = "PROVISIONED"
  read_capacity  = 10
  write_capacity = 10
  hash_key       = "Id"
  range_key      = "Kind"

  attribute {
    name = "Id"
    type = "S"
  }

  attribute {
    name = "Kind"
    type = "S"
  }

  ttl {
    attribute_name = "TimeToExist"
    enabled        = false
  }

  global_secondary_index {
    name            = "Kind-index"
    hash_key        = "Kind"
    write_capacity  = 10
    read_capacity   = 10
    projection_type = "ALL"
  }

  tags = {
    Name                              = "a201868_Guiss"
    "tr:application-asset-insight-id" = "206104"
    "tr:environment-name"             = "DEVELOPMENT"
    "tr:resource-owner"               = "AAADevelopmentNottingham2@thomson.com"
  }
}
