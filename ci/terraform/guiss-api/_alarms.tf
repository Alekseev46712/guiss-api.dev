module "cloudwatch_alarms" {
  source      = "../modules/metric-alarm"
  alarms      = var.alarms
  filters     = var.filters
  asset_id    = var.asset_id
  name_suffix = "${var.name_suffix}-${local.abbreviations}"
  tags        = local.tags
  group_name  = "${module.lambda.log_group_name}"
}
