Core Functionalities Implemented
1️⃣ URL Shortening Service
✅ Shorten URLs (for both anonymous & registered users)
✅ Store URLs in the database
✅ Apply CQRS with MediatR
✅ Implement Redis caching for fast URL resolution
✅ Expire old URLs with a background job (Quartz.NET)
✅ URL analytics tracking

2️⃣ API Gateway & Load Balancing
✅ Set up AWS API Gateway for routing
✅ Implemented Application Load Balancer (ALB)
✅ Integrated ALB logs with CloudWatch for monitoring

3️⃣ Identity & Authentication Service
✅ JWT Authentication
✅ Role-Based Access Control (RBAC) for Cognito users
✅ Implemented event-driven user authentication
✅ RabbitMQ Consumer for login tracking
✅ Single Sign-On (SSO) with Microsoft Authentication App
✅ Audit Logging for admin actions

4️⃣ Analytics & Monitoring
✅ URL click tracking stored in Redis for fast queries
✅ RabbitMQ event-driven logging
✅ Analytics API to fetch login & URL tracking data
✅ Dashboard UI for real-time analytics (Angular)
✅ WebSockets for instant analytics updates
✅ Role-Based Access Control (RBAC) for analytics
✅ Rate Limiting & Logging
✅ Real-time analytics with SignalR + Redis Pub/Sub
✅ Customizable real-time charts & graphs
✅ Export analytics to CSV, PDF, Excel

5️⃣ Notifications & Alerts
✅ Admin Notification System (In-App, Email, SMS)
✅ SMS Alerts via Twilio/Slack/Telegram API
✅ Push Notifications for mobile/web apps
✅ Graphical analytics for notifications (charts & graphs)

6️⃣ Multi-Tenancy & Billing
✅ Multi-Tenant Support for Enterprise Users
✅ Subscription & Billing (Plans, Usage-Based Pricing)
✅ Affiliate & Referral System
✅ API Monetization (Charging for Extra API Calls)

🚀 AWS Infrastructure & Deployment
7️⃣ AWS Services & Deployment
✅ Deployed ECS Fargate microservices
✅ Infrastructure as Code with Terraform
✅ Automate CI/CD with GitHub Actions
✅ Implemented Rollback on failure
✅ Multi-Environment Deployment (Dev, Staging, Prod)
✅ AWS Secrets Manager for auto-rotating database credentials
✅ AWS Parameter Store for dynamic configurations

8️⃣ Monitoring & Logging
✅ CloudWatch, Prometheus, and Grafana for monitoring
✅ Integrated Prometheus AlertManager for automatic notifications
✅ Connected ALB logs with CloudWatch

9️⃣ Performance Optimization
✅ Load Testing with k6 & JMeter
✅ Database Optimization (Indexes, Partitioning, Query Optimization)
✅ Implemented Reverse Proxy
✅ RabbitMQ Consumers for async processing

🔟 Terraform & Jenkins Integration
✅ Automating Terraform with Jenkins
✅ Created Jenkinsfile for CI/CD pipeline
✅ Extended to multi-environment deployment