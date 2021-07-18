output "name" {
    description = "iam role name"
    value = "${aws_iam_role.template_api_role.name}"
}

output "arn" {
    description = "iam role arn"
    value = "${aws_iam_role.template_api_role.arn}"
}
