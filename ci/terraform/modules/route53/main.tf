data "aws_route53_zone" "hosted_zone" {
  name         = var.domain
  private_zone = true
}

resource "aws_route53_record" "route53_record" {
  zone_id                  = data.aws_route53_zone.hosted_zone.id
  name                     = var.name
  type                     = var.type
  alias {
    name                   = var.alias_name
    zone_id                = var.alias_zone_id
    evaluate_target_health = var.alias_target_health
  }
}
