output "template_lambda_name" {
    description = "Name of the Lambda function"
    value = "${module.template_api.lambda_name}"
}

output "template_api_base_url" {
    description = "Base URL to invoke the REST API"
    value = "${module.template_api_gateway.api_base_url}"
}

output "template_table_name" {
    description = "DynamoDB table name"
    value = "${module.template_dynamodb_table.table_name}"
}

output "template_lambda_hash" {
    description = "Lambda Hash"
    value = "${module.template_api.lambda_source_code_hash}"
}

output "template_lambda_alias_current_arn" {
    description = "The ARN identifying your Lambda function alias."
    value       = "${module.template_api.lambda_alias_current_arn}"
}

output "template_lambda_current_alias_current_invoke_arn" {
    description = "The ARN to be used for invoking Lambda Function from API Gateway - to be used in"
    value       = "${module.template_api.lambda_alias_current_invoke_arn}"
}

output "lambda_latest_version" {
    description = "Latest published version of your Lambda Function."
    value       = "${module.template_api.lambda_latest_version}"
}

output "alarm_names" {
    description = "The names of the alarms"
    value       = concat(module.template_alarms.alarm_names, module.dynamodb_alarms.alarm_names)
}

output "metric_filter_names" {
    description = "The names of the metric filter"
    value       = "${module.template_alarms.metric_filter_names}"
}