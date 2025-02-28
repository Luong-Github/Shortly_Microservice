resource "aws_s3_bucket" "alb_logs" {
  bucket = "alb-logs-bucket-${random_id.suffix.hex}"
  acl    = "private"
}

resource "aws_s3_bucket_policy" "alb_logs_policy" {
  bucket = aws_s3_bucket.alb_logs.id
  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "elasticloadbalancing.amazonaws.com"
      },
      "Action": "s3:PutObject",
      "Resource": "${aws_s3_bucket.alb_logs.arn}/*"
    }
  ]
}
POLICY
}
