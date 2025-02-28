resource "aws_globalaccelerator_accelerator" "global_accelerator" {
  name = "my-global-accelerator"
  enabled = true
}

resource "aws_globalaccelerator_listener" "listener" {
  accelerator_arn = aws_globalaccelerator_accelerator.global_accelerator.arn
  protocol        = "TCP"
  port_range {
    from_port = 80
    to_port   = 80
  }
}

resource "aws_globalaccelerator_endpoint_group" "endpoint_group" {
  listener_arn = aws_globalaccelerator_listener.listener.arn
  endpoint_configuration {
    endpoint_id = aws_lb.ecs_alb.arn
    weight      = 100
  }
}
