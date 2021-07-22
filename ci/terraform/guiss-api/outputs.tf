output "create_audit_lambda_name" {
  description = "Name of the Lambda function"
  value       = "${module.lambda.lambda_name}"  
}

output "create_audit_lambda_arn" {
  description = "Amazon Resource Name of the Lambda function"
  value       = "${module.lambda.lambda_arn}"
}
