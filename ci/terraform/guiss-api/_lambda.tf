data "aws_vpc" "main" {
  tags = {
    Name = var.aws_vpc
  }
}

data "aws_subnet_ids" "config" {
  vpc_id = data.aws_vpc.main.id
  filter {
    name   = "tag:Name"
    values = var.lambda_subnets
  }
}

data "aws_security_groups" "config" {
  filter {
    name   = "tag:Name"
    values = var.lambda_security_groups
  }
}

module "lambda_role" {
  source              = "../modules/lambda-role"
  iam_role_name       = "a${var.asset_id}-role-${var.lambda_name}-${var.name_suffix}-${local.abbreviations}"
  aws_account         = data.aws_caller_identity.current.account_id
  tags                = local.tags
}

module "lambda" {
  source                          = "../modules/lambda"
  prefix                          = local.prefix
  asset_id                        = var.asset_id
  name                            = local.lambda_name_suffix
  role_arn                        = module.lambda_role.arn
  description                     = var.lambda_description
  filename                        = "${var.filename}${var.app_version_number}.zip"
  runtime                         = var.runtime
  handler                         = var.handler
  memory_size                     = var.memory_size
  timeout                         = var.lambda_timeout
  publish                         = var.publish
  lambda_alias_current            = var.lambda_alias_current
  subnet_ids                      = data.aws_subnet_ids.config.ids
  security_group_ids              = data.aws_security_groups.config.ids

  environment_variables           = merge(var.lambda_env_vars, {
     "AWS__ParameterStorePath"                       = "/${local.prefix}/${var.name_suffix}/${var.param_path}",
     "AppSettings__DynamoDb__UserAttributeTableName" = module.dynamodb.name,
     "Version"                                       = var.app_version_number,
                                    })

  tags                            = local.tags
}

# Parameter store access
resource "aws_iam_role_policy" "parameter_store_access" {
  name = "parameter-store-access"
  role = module.lambda_role.name
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = [
          "ssm:DescribeParameters",
        ]
        Effect   = "Allow"
        Resource = "*"
      },
      {
        Action = [
          "ssm:PutParameter",
          "ssm:GetParametersByPath",
        ]
        Effect   = "Allow"
#        Resource = "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/${local.prefix}/${var.name_suffix}/${var.param_path}/*"
        Resource = "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/${var.param_path}/*"
      },
    ]
  })
}

# DynamoDb access
resource "aws_iam_role_policy" "dynabodb_access" {
  name = "dynabodb_access"
  role = module.lambda_role.name
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = [
          "dynamodb:DescribeTable",
          "dynamodb:ListTables",
          "dynamodb:GetRecords",
          "dynamodb:BatchGetItem",
          "dynamodb:DeleteItem",
          "dynamodb:UpdateItem",
          "dynamodb:PutItem",
          "dynamodb:GetItem",
        ]
        Effect   = "Allow"
        Resource = module.dynamodb.arn
      },
      {
        Action = [
          "dynamodb:Query",
          "dynamodb:Scan",
        ]
        Effect   = "Allow"
        Resource = [
          module.dynamodb.arn,
          "${module.dynamodb.arn}/index/*",
        ]
      },
    ]
  })
}

# Access to configure VPC
resource "aws_iam_role_policy_attachment" "lambda_vpc" {
  role       = module.lambda_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
}
