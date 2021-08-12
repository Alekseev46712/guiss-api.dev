data "aws_kms_key" "parameter" {
  key_id = "alias/${var.kms_alias}"
}

resource "aws_ssm_parameter" "string" {
  for_each = toset(var.params)

  name     = "/a${var.asset_id}/${var.name_suffix}/${each.value}"
  valu     = "changeme"
  type     = "String"
  tags     = local.tags

  lifecycle {
    ignore_changes = [ value ]
  }
}

resource "aws_ssm_parameter" "secure_string" {
  for_each = toset(var.secure_params)

  name     = "/a${var.asset_id}/${var.name_suffix}/${each.value}"
  type     = "SecureString"
  value    = "changeme"
  key_id   = data.aws_kms_key.parameter.id
  tags     = local.tags

  lifecycle {
    ignore_changes = [ value ]
  }
}


# Variables
variable "kms_alias" {
  type        = string
  default     = "aws/ssm"
}
variable "params" {
  type        = any
}
variable "secure_params" {
  type        = any
}
