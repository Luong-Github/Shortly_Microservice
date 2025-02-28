resource "aws_route53_record" "primary" {
  zone_id = aws_route53_zone.main.zone_id
  name    = "api.myproject.com"
  type    = "A"

  set_identifier = "primary-region"
  failover_routing_policy {
    type = "PRIMARY"
  }
  alias {
    name                   = aws_lb.ecs_alb.dns_name
    zone_id                = aws_lb.ecs_alb.zone_id
    evaluate_target_health = true
  }
}

resource "aws_route53_record" "secondary" {
  zone_id = aws_route53_zone.main.zone_id
  name    = "api.myproject.com"
  type    = "A"

  set_identifier = "secondary-region"
  failover_routing_policy {
    type = "SECONDARY"
  }
  alias {
    name                   = aws_lb.secondary_alb.dns_name
    zone_id                = aws_lb.secondary_alb.zone_id
    evaluate_target_health = true
  }
}
