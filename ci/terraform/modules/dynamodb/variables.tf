variable "name" {
  type        = string
}
variable "billing_mode" {
  type        = string
}
variable "read_capacity" {
  type        = string
}
variable "write_capacity" {
  type        = string
}
variable "hash_key" {
  type        = string
}
variable "range_key" {
  type        = string
}
variable "server_side_encryption" {
  type        = bool
}
variable "attributes" {
  type        = any
}
variable "global_secondary_indexes" {
  type        = any
}
variable "tags" {
    type      = map
}
