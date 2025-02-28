resource "aws_cloudwatch_metric_alarm" "alb_5xx_errors" {
  alarm_name          = "ALB-High-5XX-Errors"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = 2
  metric_name         = "HTTPCode_ELB_5XX_Count"
  namespace          = "AWS/ApplicationELB"
  period             = 60
  statistic          = "Sum"
  threshold          = 10
  alarm_description  = "Triggers if ALB 5XX errors exceed 10 in a minute."
  alarm_actions      = [aws_sns_topic.alb_alerts.arn]
}

resource "aws_sns_topic" "alb_alerts" {
  name = "alb-alerts"
}
