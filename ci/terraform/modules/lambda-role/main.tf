data "aws_iam_policy_document" "lambda_can_assume_role" {
  statement {
    actions = ["sts:AssumeRole"]
    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "lambda_role" {
  name                  = var.iam_role_name
  assume_role_policy    = data.aws_iam_policy_document.lambda_can_assume_role.json
  force_detach_policies = true
  tags = var.tags
}

resource "aws_iam_role_policy_attachment" "mtemplate_lambda_logging_policy_attachment" {
  role       = aws_iam_role.lambda_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

# The following policy attachments are mandatory on all roles:
resource "aws_iam_role_policy_attachment" "routing_api_role_global_deny_1" {
  role       = aws_iam_role.lambda_role.name
  policy_arn = "arn:aws:iam::${var.aws_account}:policy/a204161-service-policy-global-deny-1"
}

resource "aws_iam_role_policy_attachment" "routing_api_role_global_deny_2" {
  role       = aws_iam_role.lambda_role.name
  policy_arn = "arn:aws:iam::${var.aws_account}:policy/a204161-service-policy-global-deny-2"
}
