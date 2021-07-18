resource "aws_dynamodb_table" "template-table" {
  name           = "${var.prefix}-${var.name}"
  billing_mode   = var.billing_mode
  read_capacity  = var.read_capacity
  write_capacity = var.write_capacity
  hash_key       = "Id"
  range_key      = "Kind"
  stream_enabled = var.stream_enabled
  stream_view_type = var.stream_view_type

  attribute {
    name = "Id"
    type = "S"
  }

  attribute {
    name = "Kind"
    type = "S"
  }

  attribute {
    name = "SearchName"
    type = "S"
  }

  global_secondary_index {
    name            = "Kind-index"
    hash_key        = "Kind"
    write_capacity  = var.secondary_write_capacity
    read_capacity   = var.secondary_read_capacity
    projection_type = "ALL"
  }

  global_secondary_index {
    name            = "Name-index"
    hash_key        = "SearchName"
    range_key       = "Kind"
    write_capacity  = var.secondary_write_capacity
    read_capacity   = var.secondary_read_capacity
    projection_type = "ALL"
  }

  tags = merge(var.tags, map("Name", var.name))
}
