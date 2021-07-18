variable "prefix" {
    description = "Prefix for Asset ID"
    type        = string
}

variable "name" {
    description = "Table name"
    type = string
}

variable "billing_mode" {
    description = "PROVISIONED or PAY_PER_REQUEST"
    type = string
}

variable "read_capacity" {
    description = "Primary index request units per second"
    type = number
}

variable "secondary_read_capacity" {
    description = "Secondary index request units per second"
    type = number
}

variable "write_capacity" {
    description = "Primary index request units per second"
    type = number
}

variable "secondary_write_capacity" {
    description = "Secondary index request units per second"
    type = number
}

variable "tags" {
    description = "Tags to be applied to the table"
    type = map
}

variable "stream_enabled"{
    description ="Stream enabled feature"
    type = bool
}

variable "stream_view_type"{
    description ="Stream view type feature"
    type = string
}
