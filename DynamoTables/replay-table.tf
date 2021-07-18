resource "aws_dynamodb_table" "replay-table" {
  name           = "a206104-aaa-replay-dynamodb"
  billing_mode   = "PROVISIONED"
  read_capacity  = 10
  write_capacity = 10
  hash_key       = "Guid"

  attribute {
    name = "Guid"
    type = "S"
  }

  attribute {
    name = "Date"
    type = "S"
  }

  attribute {
    name = "Time"
    type = "N"
  }

  ttl {
    attribute_name = "TTL"
    enabled        = true
  }

  global_secondary_index {
    name            = "Date-index"
    hash_key        = "Date"
    range_key       = "Time"
    write_capacity  = 10
    read_capacity   = 10
    projection_type = "ALL"
  }

  tags = {
    Name                              = "a206104-aaa-replay-dynamodb"
    "tr:application-asset-insight-id" = "206104"
    "tr:environment-name"             = "DEVELOPMENT"
    "tr:resource-owner"               = "AAADevelopmentNottingham2@thomson.com"
  }
}