# The Pending Replication alarm is only applicable to global tables, which synchronise data across regions.
locals {
  alarms = merge(
    {
      "ReadThrottleEvents" = {
        "threshold" = var.thresholds.read_throttle
        "missing"   = "notBreaching"
      }
      "WriteThrottleEvents" = {
        "threshold" = var.thresholds.write_throttle
        "missing"   = "notBreaching"
      }
      "SystemErrors" = {
        "threshold" = var.thresholds.system_errors
        "missing"   = "notBreaching"
      }
    },
    var.is_global_table ? {
      "PendingReplicationCount" = {
        "threshold" = var.thresholds.pending_replication
        "missing"   = "missing"
      }
  } : {})
}

data "aws_sns_topic" "compass" {
  name = var.alarm_topic_name
}

resource "aws_cloudwatch_metric_alarm" "dynamodb_alarm" {
  for_each = local.alarms

  alarm_name = "${var.table_name}-${each.key}-alarm"
  namespace  = "AWS/DynamoDB"
  dimensions = {
    TableName = var.table_name
  }
  metric_name         = each.key
  statistic           = "SampleCount"
  comparison_operator = "GreaterThanThreshold"
  threshold           = each.value.threshold
  period              = 60
  evaluation_periods  = 3
  treat_missing_data  = each.value.missing
  alarm_actions       = [data.aws_sns_topic.compass.arn]
  ok_actions          = [data.aws_sns_topic.compass.arn]
  tags                = var.tags
}
