# Containerized-.NET-Risk-Job
.NET 8 Web API deployed in AWS ECS (Fargate).

## Overview
This project demonstrates how to containerize a .NET 8 Web API application and deploy it using AWS ECS (Fargate). The application is designed to handle risk job processing, making it suitable for scenarios that require scalable and efficient risk management solutions.
## Features
- .NET 8 Web API for handling risk job requests.
- Containerized using Docker for easy deployment and scalability. So you can push it to any container registry like AWS ECR, Docker Hub, etc.


## 🚀 ECS Deployment Strategy Overview.
The deployment strategy for the .NET 8 Web API application on AWS ECS (Fargate) involves several key steps to ensure a smooth and efficient deployment process. Below is an overview of the deployment strategy:

Amazon ECS (Elastic Container Service) is a fully managed container orchestration service. At a high level, the flow looks like this:
1. **Build & push container**: Docker image pushed to Amazon ECR (Elastic Container Registry).
2. **Task Definition**: Defines how to run your container (image, CPU/memory, ports, environment).
3. **Service**: Manages long-running tasks, ensures desired count (e.g., 2 replicas).
4. **Networking**: Tied to a VPC, subnets, and security groups (can use ALB for external access).
5. **IAM Roles**: Grant permissions to ECS tasks and services to interact with AWS resources.
6. **Service Discovery**: Lets containers find each other by name instead of IP.

# 🚀 ECS Deployment Guide (Risk Job API)

This guide explains how to deploy the **Risk Job API** container to **Amazon ECS Fargate**.

---

## 📦 1. Build & Push Image
```bash
aws ecr get-login-password --region us-east-1 \
  | docker login --username AWS --password-stdin 123456789.dkr.ecr.us-east-1.amazonaws.com

docker build -t risk-job-api .
docker tag risk-job-api:latest 123456789.dkr.ecr.us-east-1.amazonaws.com/risk-job-api:latest
docker push 123456789.dkr.ecr.us-east-1.amazonaws.com/risk-job-api:latest
````

---

## 📄 2. Task Definition

Defines container settings (image, CPU, memory, ports, logs).

```json
{
  "family": "risk-job-api",
  "executionRoleArn": "arn:aws:iam::123456789:role/ecsTaskExecutionRole",
  "taskRoleArn": "arn:aws:iam::123456789:role/riskJobTaskRole",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "256",
  "memory": "512",
  "containerDefinitions": [
    {
      "name": "risk-job-api",
      "image": "123456789.dkr.ecr.us-east-1.amazonaws.com/risk-job-api:latest",
      "portMappings": [{ "containerPort": 8080 }],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/risk-job-api",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ]
}
```

---

## 🛠️ 3. Deploy ECS Service

Runs and maintains tasks behind a load balancer.

```bash
aws ecs create-service \
  --cluster risk-job-cluster \
  --service-name risk-job-service \
  --task-definition risk-job-api \
  --desired-count 2 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-abc,subnet-def],securityGroups=[sg-123],assignPublicIp=DISABLED}"
```

---

## 🔑 4. IAM Roles

* **Execution Role** → pull from ECR, send logs.
* **Task Role** → app permissions (S3, Secrets Manager, etc.).

---

## 🌐 5. Networking & Discovery

* Tasks run in **private subnets** with security groups.
* Expose via **ALB** or register with **Cloud Map / Route 53**.

---

## 📊 6. Scaling & Monitoring

* **Auto scaling** by CPU/memory.
* **Health checks** via ALB.
* **Logs/metrics** in CloudWatch.

---

✅ Risk Job API is now running on ECS Fargate and accessible via ALB or DNS.
