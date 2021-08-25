variable "alarms" {
    description = "Alarm configuration"
    type = any
}

variable "filters" {
    description = "Filter configuration"
    type = any
}

variable "asset_id" {
    description = "The asset ID to prefix to the name"
    type = "string"
}

variable "name_suffix" {
    description = "Suffix to add to the alarm names"
    type = "string"
}

variable "abbreviations" {
    description = "Region abbreviation to add to the alarm name"
    type = "string"
}

variable "tags" {
    description = "Tags to add to the alarms"
    type = "map"
}

variable "group_name" {
    description = "Name of the associated Lambda function"
    type = "string"
}
