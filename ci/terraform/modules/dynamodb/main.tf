resource "aws_dynamodb_table" "database" {
  name           = var.name
  billing_mode   = var.billing_mode
  read_capacity  = var.read_capacity
  write_capacity = var.write_capacity
  hash_key       = var.hash_key
  range_key      = var.range_key

  server_side_encryption {
    enabled = var.server_side_encryption
  }

  dynamic "attribute" {
    for_each = [for s in var.attributes: {
      name = s.name
      type = s.type
    }]

    content {
      name = attribute.value.name
      type = attribute.value.type
    }
  }

  dynamic "global_secondary_index" {
    for_each = [for s in var.global_secondary_indexes: {
      name            = s.name
      hash_key        = s.hash_key
      write_capacity  = s.write_capacity
      read_capacity   = s.read_capacity
      projection_type = s.projection_type
    }]

    content {
      name            = global_secondary_index.value.name
      hash_key        = global_secondary_index.value.hash_key
      write_capacity  = global_secondary_index.value.write_capacity
      read_capacity   = global_secondary_index.value.read_capacity
      projection_type = global_secondary_index.value.projection_type
    }
  }

  tags = var.tags
}
