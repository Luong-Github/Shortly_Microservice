provider "aws" {
  region = "us-east-1"
}

# 🚀 Create a VPC
resource "aws_vpc" "main_vpc" {
  cidr_block = "10.0.0.0/16"
}

# 🌐 Create a Public Subnet
resource "aws_subnet" "public_subnet" {
  vpc_id                  = aws_vpc.main_vpc.id
  cidr_block              = "10.0.1.0/24"
  map_public_ip_on_launch = true
}

# 🔒 Security Group for NGINX
resource "aws_security_group" "nginx_sg" {
  vpc_id = aws_vpc.main_vpc.id

  # Allow HTTP and HTTPS
  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # Allow SSH (Optional - for debugging)
  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # Allow all outbound traffic
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# 📀 Launch EC2 Instance for NGINX
resource "aws_instance" "nginx" {
  ami           = "ami-0c55b159cbfafe1f0" # Ubuntu AMI (Change as needed)
  instance_type = "t3.micro"
  subnet_id     = aws_subnet.public_subnet.id
  security_groups = [aws_security_group.nginx_sg.name]

  user_data = <<-EOF
              #!/bin/bash
              apt update -y
              apt install -y nginx
              systemctl start nginx
              systemctl enable nginx
              EOF

  tags = {
    Name = "nginx-reverse-proxy"
  }
}

# 🌍 Create an Application Load Balancer (ALB)
resource "aws_lb" "nginx_alb" {
  name               = "nginx-alb"
  internal           = false
  load_balancer_type = "application"
  security_groups    = [aws_security_group.nginx_sg.id]
  subnets           = [aws_subnet.public_subnet.id]
}

# 🏹 Create a Target Group
resource "aws_lb_target_group" "nginx_tg" {
  name     = "nginx-tg"
  port     = 80
  protocol = "HTTP"
  vpc_id   = aws_vpc.main_vpc.id
}

# 🔀 Attach the EC2 instance to the ALB Target Group
resource "aws_lb_target_group_attachment" "nginx_attach" {
  target_group_arn = aws_lb_target_group.nginx_tg.arn
  target_id        = aws_instance.nginx.id
  port             = 80
}

# 🔀 Create a Listener for HTTP Traffic
resource "aws_lb_listener" "nginx_http_listener" {
  load_balancer_arn = aws_lb.nginx_alb.arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.nginx_tg.arn
  }
}

# 📊 Enable CloudWatch Logging
resource "aws_cloudwatch_log_group" "nginx_logs" {
  name = "/aws/ec2/nginx-logs"
}

# 🏗 Output Load Balancer DNS
output "alb_dns_name" {
  value = aws_lb.nginx_alb.dns_name
}
