resource "aws_cloudwatch_metric_alarm" "bells" {
  for_each = var.alarms

  alarm_name                = "a${var.asset_id}-${each.key}-alarm-${var.name_suffix}"
  comparison_operator       = each.value.comparison_operator
  evaluation_periods        = each.value.evaluation_periods
  metric_name               = each.value.metric
  namespace                 = each.value.namespace
  period                    = each.value.period
  statistic                 = each.value.statistic
  threshold                 = each.value.threshold
  alarm_description         = each.value.description
  alarm_actions             = each.value.alarm_actions
  ok_actions                = each.value.ok_actions
  insufficient_data_actions = each.value.insufficient_data_actions
  treat_missing_data        = each.value.treat_missing_data
  tags                      = merge(var.tags, each.value.tags)
  dimensions                = each.value.dimensions
}

resource "aws_cloudwatch_log_metric_filter" "records" {
  for_each       = var.filters

  name           = "a${var.asset_id}-${each.key}-filter-${var.name_suffix}"
  pattern        = each.value.pattern
  log_group_name = var.group_name

  metric_transformation {
    name      = "${each.key}-${var.name_suffix}"
    namespace = each.value.namespace
    value     = each.value.value
  }
}
