variable "target_hosted_zone_id" {
    description = "The ID of the hosted zone to contain this record."
    type        = string
}

variable "domain_name" {
  description = "The name of the record."
  type        = string
}

variable "type" {
    description = "The record type. Valid values are A, AAAA, CAA, CNAME, MX, NAPTR, NS, PTR, SOA, SPF, SRV and TXT."
    type        = string
}

variable "alias_regional_domain_name" {
  description = "DNS domain name for a CloudFront distribution."
  type        = string
}

variable "alias_regional_zone_id" {
    description = "Hosted zone ID for a CloudFront distribution."
    type        = string
}

variable "alias_target_health" {
  description = "Checking the health of the resource record set."
  type        = string
  default     = false
}

variable "should_create_record_set" {
    type        = string
}