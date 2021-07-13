resource "aws_api_gateway_domain_name" "gateway_domain_name" {
    count                           = "${var.should_create_custom_domainname ? 1 : 0}"
    domain_name                     = "${var.domain_name}"
    certificate_arn                 = "${var.certificate_arn}"
    //regional_certificate_arn        = "${var.certificate_arn}"
    //endpoint_configuration           {
    //    types = ["REGIONAL"]
    //}
    //depends_on                      = [
    //    "aws_api_gateway_stage.production_api_gateway_stage"
    //] 
}

resource "aws_api_gateway_base_path_mapping" "gateway_base_path_mapping" {
    count                           = "${var.should_create_custom_domainname ? 1 : 0}"
    api_id                          = "${var.api_id}"
    stage_name                      = "${var.production_stage_name}"
    //domain_name                     = "${var.domain_name}"
    domain_name                     = "${aws_api_gateway_domain_name.gateway_domain_name[0].domain_name}"
    depends_on                      = [
        "aws_api_gateway_domain_name.gateway_domain_name"
    ]
}
