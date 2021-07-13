data "aws_iam_policy_document" "lambda_can_assume_role" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }
  }
}

data "aws_iam_policy_document" "template_dynamodb_permissions" {
  statement {
    effect = "Allow"
    actions = [
      "dynamodb:Query",
      "dynamodb:Scan"
    ]
    resources = [
      var.template_dynamodb_table_arn,
      "${var.template_dynamodb_table_arn}/index/Kind-index",
      "${var.template_dynamodb_table_arn}/index/Name-index",
      var.replay_dynamodb_table_arn
      ]
  }
  statement {
    effect = "Allow"
    actions = [
      "dynamodb:DescribeTable",
      "dynamodb:ListTables",
      "dynamodb:GetRecords",
      "dynamodb:BatchGetItem",
      "dynamodb:BatchWriteItem",
      "dynamodb:DeleteItem",
      "dynamodb:UpdateItem",
      "dynamodb:PutItem",
      "dynamodb:GetItem"
    ]
    resources = [var.template_dynamodb_table_arn,
                 var.replay_dynamodb_table_arn
                  ]
  }
   statement {
    effect = "Allow"
    actions = [
      "dynamodb:DescribeStream",
      "dynamodb:GetRecords",
      "dynamodb:GetShardIterator",
      "dynamodb:ListStreams"
    ]
    resources = ["*"]
  }
}

data "aws_iam_policy_document" "template_sqs_permissions" {
  statement {
    effect = "Allow"
    actions = [
      "sqs:GetQueueAttributes",
      "sqs:GetQueueUrl",
      "sqs:ListDeadLetterSourceQueues",
      "sqs:ListQueues",
      "sqs:SendMessage",
      "sqs:SendMessageBatch",
      "sqs:ReceiveMessage",
      "sqs:CreateQueue"
    ]
    resources = ["*"]
  }
}

data "aws_iam_policy_document" "template_ssm_permissions" {
  statement {
    effect = "Allow"
    actions = [
      "ssm:PutParameter",
      "ssm:GetParameter",
      "ssm:GetParameters",
      "ssm:GetParametersByPath"
    ]
    resources = ["*"]
  }
}

data "aws_iam_policy_document" "template_sns_permissions" {
  statement {
    effect = "Allow"
    actions = [
    "sns:*"
  ]
  resources = ["*"]
  }
}

resource "aws_iam_role" "template_api_role" {
  path = "/service-role/"
  name                  = var.iam_role_name
  assume_role_policy    = data.aws_iam_policy_document.lambda_can_assume_role.json
  force_detach_policies = true
}

resource "aws_iam_policy" "template_dynamodb_policy" {
  name   = var.policy_name #change this!
  policy = data.aws_iam_policy_document.template_dynamodb_permissions.json
}

resource "aws_iam_policy" "template_sqs_policy" {
  name   = var.sqs_policy_name
  policy = data.aws_iam_policy_document.template_sqs_permissions.json
}

resource "aws_iam_policy" "template_ssm_policy" {
  name   = var.ssm_policy_name
  policy = data.aws_iam_policy_document.template_ssm_permissions.json
}

resource "aws_iam_policy" "template_sns_policy" {
  name   = var.sns_policy_name 
  policy = data.aws_iam_policy_document.template_sns_permissions.json
}

resource "aws_iam_role_policy_attachment" "template_dynamodb_policy_attachment" {
  role       = aws_iam_role.template_api_role.name
  policy_arn = aws_iam_policy.template_dynamodb_policy.arn
}

resource "aws_iam_role_policy_attachment" "template_sqs_policy_attachment" {
  role       = aws_iam_role.template_api_role.name
  policy_arn = aws_iam_policy.template_sqs_policy.arn
}

resource "aws_iam_role_policy_attachment" "template_ssm_policy_attachment" {
  role       = aws_iam_role.template_api_role.name
  policy_arn = aws_iam_policy.template_ssm_policy.arn 
}

resource "aws_iam_role_policy_attachment" "template_sns_policy_attachment" {
  role       = aws_iam_role.template_api_role.name
  policy_arn = aws_iam_policy.template_sns_policy.arn 
}

#pre-existing policy by aws for logging
resource "aws_iam_role_policy_attachment" "logging_policy_attachment" {
  role       = aws_iam_role.template_api_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

# The following policy attachments are mandatory on all roles:
resource "aws_iam_role_policy_attachment" "routing_api_role_global_deny_1" {
  role       = aws_iam_role.template_api_role.name
  policy_arn = "arn:aws:iam::${var.aws_account}:policy/a204161-service-policy-global-deny-1"
}

resource "aws_iam_role_policy_attachment" "routing_api_role_global_deny_2" {
  role       = aws_iam_role.template_api_role.name
  policy_arn = "arn:aws:iam::${var.aws_account}:policy/a204161-service-policy-global-deny-2"
}