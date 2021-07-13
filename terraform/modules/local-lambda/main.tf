resource "aws_lambda_function" "lambda_function" {
  function_name    = "${var.prefix}-${var.name}"
  description      = "${var.description}"
  filename         = "${var.filename}"
  source_code_hash = filebase64sha256(var.filename)
  handler          = "${var.handler}"
  runtime          = "${var.runtime}"
  memory_size      = "${var.memory_size}"
  timeout          = "${var.timeout}"
  role             = "${var.role_arn}"
  publish          = "${var.publish}"

  # We must supply an 'environment' block if and only if the
  # environment_variables collection is not empty. It is an error to
  # include this block with an empty environment variable list.
  dynamic "environment" {
    for_each = length(var.environment_variables) > 0 ? [var.environment_variables] : []
    content {
      variables = environment.value
    }
  }

  vpc_config {
    subnet_ids         = "${var.subnet_ids}"
    security_group_ids = "${var.security_group_ids}"
  }
  tags = "${merge(var.tags, map("Name", "${var.prefix}-${var.name}"))}"

  depends_on = ["aws_cloudwatch_log_group.log_group"]
}

resource "aws_lambda_alias" "lambda_alias_current" {
  name              = "${var.lambda_alias_current}"
  function_name     = "${aws_lambda_function.lambda_function.arn}"
  function_version  = "${aws_lambda_function.lambda_function.version}"
}

resource "aws_cloudwatch_log_group" "log_group" {
  name              = "${var.log_group_prefix}${var.prefix}-${var.name}"
}

locals {
  timestamp = "${timestamp()}"
}