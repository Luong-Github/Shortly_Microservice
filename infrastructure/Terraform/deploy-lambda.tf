resource "aws_lambda_function" "alb_logs_lambda" {
  function_name = "alb-logs-processor"
  runtime       = "python3.8"
  role          = aws_iam_role.alb_logs_lambda_role.arn
  handler       = "index.lambda_handler"
  filename      = "alb_logs_processor.zip"
  timeout       = 30
}

resource "aws_lambda_permission" "allow_s3" {
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.alb_logs_lambda.function_name
  principal     = "s3.amazonaws.com"
  source_arn    = aws_s3_bucket.alb_logs.arn
}

resource "aws_s3_bucket_notification" "alb_logs_notification" {
  bucket = aws_s3_bucket.alb_logs.id

  lambda_function {
    lambda_function_arn = aws_lambda_function.alb_logs_lambda.arn
    events              = ["s3:ObjectCreated:*"]
  }
}
