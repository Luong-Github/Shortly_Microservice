data "aws_ssm_parameter" "db_username" {
  name = "/${terraform.workspace}/db_username"
}

data "aws_ssm_parameter" "db_password" {
  name = "/${terraform.workspace}/db_password"
  with_decryption = true
}
