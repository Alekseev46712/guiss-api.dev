# Parameter store
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

# DynamoDb
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
