output "alarm_names" {
    description = "The names of the alarms"
    value   = [ for alarm in aws_cloudwatch_metric_alarm.bells: alarm.arn ]
}

output "metric_filter_names" {
    description = "The names of the metric filter"
    value   = [ for metric in aws_cloudwatch_log_metric_filter.records: metric.id ]
}