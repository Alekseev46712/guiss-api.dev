variable "table_name" {
  description = "Name of DynamoDB table"
  type        = string
}

variable "is_global_table" {
  description = "Indicates whether this is a global table that requires monitoring of the pending replication count."
  type        = bool
}

variable "thresholds" {
  type        = object({ pending_replication = number, read_throttle = number, write_throttle = number, system_errors = number })
  description = "Threshold values that trigger an alarm on DynamoDB"
  default = {
    pending_replication = 300
    read_throttle       = 100
    write_throttle      = 100
    system_errors       = 10
  }
}

variable "tags" {
  type        = map
  description = "AWS Tags to be applied to the alarms"
}

variable "alarm_topic_name" {
  type        = string
  default     = "compass-alarm-notification"
  description = "Name of SNS topic to be used for alarms"
}
