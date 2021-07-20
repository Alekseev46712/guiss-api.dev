provider "aws" {
  access_key                  = "dummy key"
  secret_key                  = "dummy secret"
  region                      = "eu-west-1"
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true

  endpoints {
    dynamodb = "http://dynamodb:8000"
  }
}