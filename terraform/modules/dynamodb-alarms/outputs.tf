output "alarm_names" {
    description = "The names of the alarms"
    value   = [ for alarm in aws_cloudwatch_metric_alarm.dynamodb_alarm: alarm.arn ]
}
