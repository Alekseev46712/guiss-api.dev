data "aws_security_groups" "memcached" {
  filter {
    name   = "tag:Name"
    values = var.ec_security_groups
  }
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.main.id]
  }
}

resource "aws_security_group" "memcached" {
  name        = "a${var.asset_id}-sg-memcached-${var.lambda_name}-${var.name_suffix}-${local.abbreviations}"
  description = "Security group for ${var.lambda_name} memcached"
  vpc_id      = data.aws_vpc.main.id

  ingress {
    from_port   = "11211"
    to_port     = "11211"
    protocol    = "tcp"
    cidr_blocks = ["10.0.0.0/8"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
  tags = local.tags
}

resource "aws_elasticache_cluster" "memcached" {
  cluster_id           = "a${var.asset_id}-memcached-${var.lambda_name}-${var.name_suffix}-${local.abbreviations}"
  engine               = "memcached"
  node_type            = var.ec_type
  num_cache_nodes      = 1
  parameter_group_name = "default.memcached1.6"
  security_group_ids   = concat(data.aws_security_groups.memcached.ids, [aws_security_group.memcached.id])
  subnet_group_name    = var.ec_subnet_group
  tags                 = local.tags
}


# Variables
variable "ec_type" {
  type        = string
}
variable "ec_subnet_group" {
  type        = string
}
variable "ec_security_groups" {
  type        = list(string)
}
