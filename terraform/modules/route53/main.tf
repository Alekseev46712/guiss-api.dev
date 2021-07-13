resource "aws_route53_record" "route53_record" {
  count           = "${var.should_create_record_set ? 1 : 0}"
  zone_id         = "${var.target_hosted_zone_id}"
  name            = "${var.domain_name}"
  type            = "${var.type}"
  
  alias {
    name                   = "${var.alias_regional_domain_name}"
    zone_id                = "${var.alias_regional_zone_id}"
    evaluate_target_health = "${var.alias_target_health}"
  }
}