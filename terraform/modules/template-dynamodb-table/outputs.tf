output "table_name" {
    description = "DynamoDB table name"
    value = "${aws_dynamodb_table.template-table.name}"
}

output "stream_arn" {
    description = "stream value"
    value = "${aws_dynamodb_table.template-table.stream_arn}"
}

output "table_arn" {
    value = aws_dynamodb_table.template-table.arn
}