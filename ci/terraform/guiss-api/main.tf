locals {
  prefix                = "a${var.asset_id}"
  abbreviations         = lookup({"eu-west-1"="euw1", "us-east-1"="use1"}, var.aws_region, var.aws_region)
  lambda_name_suffix    = "${var.lambda_name}-${var.name_suffix}-${local.abbreviations}"
  logging_service_name  = "${local.prefix}-lambda-logging-${var.name_suffix}-${local.abbreviations}"
  tags = {
    "tr:application-asset-insight-id" = var.asset_id
    "tr:resource-owner"               = var.resource_owner
    "tr:environment-type"             = var.environment
    "tr:service-name"                 = var.service_name
    "tr:financial-identifier"         = var.financial_id
  }
}

terraform {
  backend "s3" {
  }
}

provider "aws" {
  region  = var.aws_region
  profile = var.aws_profile
  version = "~> 3.16.0"
}

data "aws_caller_identity" "current" {}
