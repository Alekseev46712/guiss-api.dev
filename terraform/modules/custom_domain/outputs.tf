output "targeted_domain_name" {
    description = "Target Domain Name created by Cloudfront"
    value       = "${element(concat(aws_api_gateway_domain_name.gateway_domain_name.*.cloudfront_domain_name, list("")), 0)}"  
}

output "custom_domain_id" {
    description = "The internal id assigned to this domain name by API Gateway"
    value       = "${element(concat(aws_api_gateway_domain_name.gateway_domain_name.*.cloudfront_zone_id, list("")), 0)}"  
}