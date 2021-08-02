output "name" {
    description = "DynamoDB table Name"
    value       = "${aws_dynamodb_table.database.name}"
}

output "arn" {
    description = "DynamoDB table ARN"
    value       = "${aws_dynamodb_table.database.arn}"
}
