provider "aws" {
  region = "us-east-1"
}

# VPC
resource "aws_vpc" "main" {
  cidr_block = "10.0.0.0/16"
}

# Subnets
resource "aws_subnet" "public" {
  vpc_id                  = aws_vpc.main.id
  cidr_block              = "10.0.1.0/24"
  map_public_ip_on_launch = true
}

resource "aws_subnet" "private" {
  vpc_id     = aws_vpc.main.id
  cidr_block = "10.0.2.0/24"
}

# API Gateway
resource "aws_api_gateway_rest_api" "api" {
  name        = "URLShortenerAPI"
  description = "API Gateway for URL Shortener Service"
}

# CloudFront Distribution
resource "aws_cloudfront_distribution" "cdn" {
  origin {
    domain_name = aws_api_gateway_rest_api.api.execution_arn
    origin_id   = "APIGatewayOrigin"
  }
  enabled = true
  default_cache_behavior {
    target_origin_id       = "APIGatewayOrigin"
    viewer_protocol_policy = "redirect-to-https"
    allowed_methods        = ["GET", "HEAD"]
    cached_methods         = ["GET", "HEAD"]
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    cloudfront_default_certificate = true
  }
}

# ElastiCache Redis
resource "aws_elasticache_cluster" "redis" {
  cluster_id           = "redis-cache"
  engine              = "redis"
  node_type           = "cache.t3.micro"
  num_cache_nodes     = 1
  parameter_group_name = "default.redis6.x"
}

# Aurora Database (Multi-Master)
resource "aws_rds_cluster" "aurora" {
  cluster_identifier      = "aurora-cluster"
  engine                 = "aurora-mysql"
  database_name          = "shortenerDB"
  master_username        = "admin"
  master_password        = "password123"
  apply_immediately      = true
  backup_retention_period = 7
}

resource "aws_rds_cluster_instance" "aurora_writer" {
  cluster_identifier = aws_rds_cluster.aurora.id
  instance_class     = "db.t3.medium"
  engine            = "aurora-mysql"
}

resource "aws_rds_cluster_instance" "aurora_reader" {
  count              = 2
  cluster_identifier = aws_rds_cluster.aurora.id
  instance_class     = "db.t3.medium"
  engine            = "aurora-mysql"
}

# DynamoDB Table
resource "aws_dynamodb_table" "clicks" {
  name           = "ClickAnalytics"
  billing_mode   = "PAY_PER_REQUEST"
  hash_key       = "short_url"

  attribute {
    name = "short_url"
    type = "S"
  }
}

# Lambda for Async Processing
resource "aws_lambda_function" "process_clicks" {
  function_name    = "ProcessClicks"
  runtime         = "python3.8"
  role            = aws_iam_role.lambda_exec.arn
  handler         = "lambda_function.lambda_handler"
  filename        = "lambda.zip"
}

# DynamoDB Stream Trigger
resource "aws_dynamodb_table" "clicks_stream" {
  stream_enabled   = true
  stream_view_type = "NEW_AND_OLD_IMAGES"
  name = "clicks_stream"
}

resource "aws_lambda_event_source_mapping" "dynamodb_trigger" {
  event_source_arn = aws_dynamodb_table.clicks.stream_arn
  function_name    = aws_lambda_function.process_clicks.arn
  starting_position = "LATEST"
}
