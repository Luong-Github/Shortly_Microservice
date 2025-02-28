resource "aws_wafv2_web_acl" "rate_limit" {
  name        = "rate-limit-acl"
  scope       = "REGIONAL"

  default_action {
    allow {}
  }

  visibility_config {
    sampled_requests_enabled = true
    cloudwatch_metrics_enabled = true
    metric_name = "RateLimitMetric"
  }

  rule {
    name     = "RateLimit"
    priority = 1

    action {
      block {}
    }

    statement {
      rate_based_statement {
        limit              = 10000  # Max 10,000 requests per 5 minutes
        aggregate_key_type = "IP"
      }
    }

    visibility_config {
      sampled_requests_enabled = true
      cloudwatch_metrics_enabled = true
      metric_name = "RateLimitMetric"
    }
  }
  
}
