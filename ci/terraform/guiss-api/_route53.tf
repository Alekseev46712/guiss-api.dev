data "aws_route53_zone" "hosted_zone" {
  name         = var.route53_domain
  private_zone = true
}

resource "aws_route53_record" "route53_record" {
  zone_id                  = data.aws_route53_zone.hosted_zone.id
  name                     = var.route53_hostname
  type                     = "A"
  alias {
    name                   = module.api_gateway.regional_domain_name
    zone_id                = module.api_gateway.regional_zone_id
    evaluate_target_health = "true"
  }
  latency_routing_policy {
    region = var.aws_region
  }
  health_check_id          = aws_route53_health_check.api.id
  set_identifier           = var.aws_region
}

resource "aws_route53_health_check" "api" {
  fqdn              = "${module.api_gateway.api_id}.execute-api.${var.aws_region}.amazonaws.com"
  port              = 443
  type              = "HTTPS"
  resource_path     = "/main/api/healthcheck"
  failure_threshold = 3
  request_interval  = 30

  tags              = merge(local.tags, {
                        "Name" = "a${var.asset_id}-${var.lambda_name}-healthcheck-${var.name_suffix}-${local.abbreviations}",
                      })
}


# Variables
variable "route53_domain" {
  type = string
}
variable "route53_hostname" {
  type = string
}
