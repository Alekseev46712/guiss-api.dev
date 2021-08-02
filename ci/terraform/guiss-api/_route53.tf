module "route53_record" {
  source         = "../modules/route53"
  name           = var.route53_hostname
  domain         = var.route53_domain
  type           = "A"
  alias_name     = module.api_gateway.targeted_domain_name
  alias_zone_id  = module.api_gateway.custom_domain_id
}

# Variables
variable "route53_domain" {
  type = string
}
variable "route53_hostname" {
  type = string
}
