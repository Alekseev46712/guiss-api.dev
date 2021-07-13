output "api_base_url" {
    description = "Base URL to invoke the REST API"
    value       = "${aws_api_gateway_deployment.deployment.invoke_url}"
}

output "execution_arn" {
    description = "Execution arn for the API Gateway"
    value       = "${aws_api_gateway_rest_api.api_gateway.execution_arn}"
}

output "api_id" {
    description = "The ID of the REST API"
    value       = "${aws_api_gateway_rest_api.api_gateway.id}"
}

output "deployment_id" {
    description = "Execution arn for the API Gateway"
    value       = "${aws_api_gateway_deployment.deployment.id}"
}

output "targeted_domain_name" {
    description = "Target Domain Name created by Cloudfront"
    value       = "${element(concat(aws_api_gateway_domain_name.gateway_domain_name.*.cloudfront_domain_name, list("")), 0)}"  
}

output "custom_domain_id" {
    description = "The internal id assigned to this domain name by API Gateway"
    value       = "${element(concat(aws_api_gateway_domain_name.gateway_domain_name.*.cloudfront_zone_id, list("")), 0)}"  
}