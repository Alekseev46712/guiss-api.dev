output "fqdn" {
  description = "FQDN built using the zone domain and name."
  value = "${aws_route53_record.route53_record.fqdn}"
}
