output "lambda_name" {
    description = "Name of the Lambda function"
    value       = "${aws_lambda_function.lambda_function.function_name}"
}

output "lambda_arn" {
    description = "Amazon Resource Name of the Lambda function"
    value       = "${aws_lambda_function.lambda_function.arn}"
}

output "lambda_invoke_arn" {
    description = "Amazon Resource Name of the Lambda function"
    value       = "${aws_lambda_function.lambda_function.invoke_arn}"
}

output "lambda_source_code_hash" {
    description = "Base64-encoded representation of raw SHA-256 sum of the zip file"
    value       = "${aws_lambda_function.lambda_function.source_code_hash}"
}

output "lambda_alias_current_arn" {
    description = "The ARN identifying your Lambda function alias."
    value       = "${aws_lambda_alias.lambda_alias_current.arn}"
}

output "lambda_alias_current_invoke_arn" {
    description = "The ARN to be used for invoking Lambda Function from API Gateway - to be used in"
    value       = "${aws_lambda_alias.lambda_alias_current.invoke_arn}"
}

output "lambda_latest_version" {
    description = "Latest published version of your Lambda Function."
    value       = "${aws_lambda_function.lambda_function.version}"
}

output "log_group_name" {
    description = "Name of the log group that messages are output to"
    value = "${var.log_group_prefix}${aws_lambda_function.lambda_function.function_name}"
}