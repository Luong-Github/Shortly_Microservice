resource "aws_iam_role" "alb_logs_lambda_role" {
  name = "alb-logs-lambda-role"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "lambda.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
EOF
}

resource "aws_iam_policy" "alb_logs_lambda_policy" {
  name        = "alb-logs-lambda-policy"
  description = "Policy for Lambda to write to CloudWatch"
  
  policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "logs:CreateLogStream",
        "logs:PutLogEvents"
      ],
      "Resource": "${aws_cloudwatch_log_group.alb_logs.arn}"
    },
    {
      "Effect": "Allow",
      "Action": [
        "s3:GetObject"
      ],
      "Resource": "${aws_s3_bucket.alb_logs.arn}/*"
    }
  ]
}
EOF
}

resource "aws_iam_role_policy_attachment" "alb_logs_lambda_attach" {
  role       = aws_iam_role.alb_logs_lambda_role.name
  policy_arn = aws_iam_policy.alb_logs_lambda_policy.arn
}
