resource "aws_ecs_task_definition" "prometheus" {
  family                   = "prometheus"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = "512"
  memory                   = "1024"

  execution_role_arn = aws_iam_role.prometheus_execution_role.arn
  task_role_arn      = aws_iam_role.prometheus_task_role.arn

  container_definitions = jsonencode([
    {
      name  = "prometheus"
      image = "prom/prometheus:latest"
      cpu   = 512
      memory = 1024
      portMappings = [{
        containerPort = 9090
        hostPort      = 9090
      }]
      command = [
        "--config.file=/etc/prometheus/prometheus.yml"
      ]
      mountPoints = [{
        sourceVolume = "prometheus-data"
        containerPath = "/prometheus"
      }]
    }
  ])
}

resource "aws_ecs_service" "prometheus_service" {
  name            = "prometheus-service"
  cluster         = aws_ecs_cluster.monitoring.id
  task_definition = aws_ecs_task_definition.prometheus.arn
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = aws_subnet.private[*].id
    security_groups  = [aws_security_group.prometheus_sg.id]
    assign_public_ip = false
  }
}
