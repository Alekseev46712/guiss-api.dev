resource "aws_dynamodb_table_item" "routing_details" {
  count      = var.should_update_routing_table ? 1 : 0
  table_name = "${var.routing_api_table_name}"
  hash_key   = "${var.routing_api_table_hash_key}"

  item = <<ITEM
  {
    "${var.routing_api_table_hash_key}": {"S": "${var.routing_id}"},
    "RouteArn": {"S": "${var.lambda_function_arn}"},
    "Version": {"N": "0"},
    "LastModifiedDateTime": {"S": "${timestamp()}"}
  }
  ITEM
}
