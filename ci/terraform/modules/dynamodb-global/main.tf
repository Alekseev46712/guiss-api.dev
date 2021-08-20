provider "aws" {
  alias   = "use1"
  region  = "us-east-1"
  profile = var.aws_profile
  version = "~> 3.16.0"
}

provider "aws" {
  alias   = "euw1"
  region  = "eu-west-1"
  profile = var.aws_profile
  version = "~> 3.16.0"
}

resource "aws_dynamodb_table" "database_us" {
  count          = var.db_install ? 1 : 0

  provider       = aws.use1

  name           = var.name
  billing_mode   = var.billing_mode
  stream_enabled   = true
  stream_view_type = "NEW_AND_OLD_IMAGES"
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

resource "aws_dynamodb_table" "database_eu" {
  count          = var.db_install && var.db_multiregion ? 1 : 0

  provider       = aws.euw1

  name           = var.name
  billing_mode   = var.billing_mode
  stream_enabled   = true
  stream_view_type = "NEW_AND_OLD_IMAGES"
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

resource "aws_dynamodb_global_table" "database_global" {
  count          = var.db_install && var.db_multiregion ? 1 : 0

  provider = aws.use1

  name = var.name
  replica {
    region_name = "us-east-1"
  }

  replica {
    region_name = "eu-west-1"
  }
  depends_on = [
    aws_dynamodb_table.database_us,
    aws_dynamodb_table.database_eu,
  ]
}
