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

Nice 👍 You’re asking about **observability** (CloudWatch Logs) and **secure configuration management** (Secrets Manager & Parameter Store). These three services are central when deploying apps (like your .NET risk job API) on ECS. Let’s break it down.

---
# Describe how you would use CloudWatch Logs, AWS Secrets Manager, and Systems Manager Parameter Store for observability and secure configuration.

## 📊 CloudWatch Logs → Observability

* **Purpose**: Centralized logging for containers running in ECS/Fargate/EC2.
* **How it works**:

  * In your ECS **task definition**, configure a `logConfiguration` to send container stdout/stderr to CloudWatch.
  * Example snippet:

    ```json
    "logConfiguration": {
      "logDriver": "awslogs",
      "options": {
        "awslogs-group": "/ecs/riskjob-api",
        "awslogs-region": "us-east-1",
        "awslogs-stream-prefix": "ecs"
      }
    }
    ```
  * Logs appear under `/ecs/riskjob-api` in CloudWatch.
* **Benefits**:

  * Aggregates logs from all containers (instead of digging into instances).
  * You can create **metrics filters** (e.g., count “RiskJobFailed”).
  * Set up **CloudWatch Alarms** on log patterns.

👉 Example: Track how often the `/run-risk-job` endpoint fails and trigger an alert to SNS/Slack.

---

## 🔐 AWS Secrets Manager → Secure Secrets

* **Purpose**: Secure storage for **sensitive values** (DB passwords, API keys).
* **How it works**:

  * Store secret once (`/riskjob/db/password`).
  * ECS task definition can inject it into the container as an **environment variable**.
  * Example snippet:

    ```json
    "secrets": [
      {
        "name": "DB_PASSWORD",
        "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789:secret:riskjob/db/password-AbCdEf"
      }
    ]
    ```
  * At runtime, the container sees:

    ```csharp
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    ```
* **Benefits**:

  * Automatic **rotation** (e.g., rotate RDS credentials every 30 days).
  * Fine-grained IAM permissions (task can only read what it needs).

👉 Example: Store your Mongo/Postgres connection string password securely and let ECS inject it when running tasks.

---

## ⚙️ Systems Manager Parameter Store → Config Management

* **Purpose**: Secure, hierarchical key-value store for **non-secret configuration**.
* **How it works**:

  * Store parameters like `/riskjob/api/base-url`, `/riskjob/feature-flags/enableRiskJob`.
  * ECS can inject them as environment variables similar to Secrets Manager:

    ```json
    "secrets": [
      {
        "name": "RISK_JOB_FEATURE_FLAG",
        "valueFrom": "arn:aws:ssm:us-east-1:123456789:parameter/riskjob/feature-flags/enableRiskJob"
      }
    ]
    ```
  * In code:

    ```csharp
    var featureEnabled = Environment.GetEnvironmentVariable("RISK_JOB_FEATURE_FLAG");
    ```
* **Benefits**:

  * Centralized config for multiple environments (dev/test/prod).
  * Can use **String** (plain) or **SecureString** (encrypted with KMS).
  * Easier to change values without redeploying.

👉 Example: Store the Google API key for borrower geolocation lookups in **Parameter Store** and inject it dynamically.

---

## 🔑 How They Work Together

* **CloudWatch Logs** → Monitor and alert on app health and risk job execution.
* **Secrets Manager** → Store sensitive credentials (DB, API keys) securely, rotate automatically.
* **Parameter Store** → Store environment-specific configs (feature flags, URLs, limits).

> ECS tasks pull **runtime configuration and secrets** securely via IAM, and send **logs/metrics** out to CloudWatch for visibility.

---

# 🛠️ CI/CD with GitHub Actions

**High-level flow:**

* Trigger on push to `main`.
* Build Docker image.
* Push to Amazon ECR.
* Update ECS Service.

**Example Workflow: `.github/workflows/deploy.yml`**

```yaml
name: Deploy to ECS

on:
  push:
    branches: [ "main" ]

jobs:
  deploy:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout
      uses: actions/checkout@v4

    - name: Configure AWS credentials
      uses: aws-actions/configure-aws-credentials@v4
      with:
        aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
        aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
        aws-region: us-east-1

    - name: Login to Amazon ECR
      run: |
        aws ecr get-login-password --region us-east-1 | \
          docker login --username AWS --password-stdin ${{ secrets.AWS_ACCOUNT_ID }}.dkr.ecr.us-east-1.amazonaws.com

    - name: Build Docker image
      run: |
        IMAGE_REPO=${{ secrets.AWS_ACCOUNT_ID }}.dkr.ecr.us-east-1.amazonaws.com/riskjob-api
        IMAGE_TAG=${{ github.sha }}
        docker build -t $IMAGE_REPO:$IMAGE_TAG .
        docker push $IMAGE_REPO:$IMAGE_TAG
        echo "IMAGE_URI=$IMAGE_REPO:$IMAGE_TAG" >> $GITHUB_ENV

    - name: Deploy to ECS
      run: |
        aws ecs update-service \
          --cluster riskjob-cluster \
          --service riskjob-service \
          --force-new-deployment
```

* **Secrets in GitHub**:

  * `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, `AWS_ACCOUNT_ID` stored securely in GitHub Secrets.
* ECS service picks up the new image (referenced by digest in ECR).

✅ More flexibility, integrates with GitHub PR workflows, and cheaper if you already use GitHub.

---

# 🔑 Best Practices

* Use **multi-stage Docker builds** to keep images small.
* Tag images with both `git sha` and `latest` for traceability.
* Enable **health checks** in ECS service to ensure zero-downtime deployments.
* Add **CloudWatch Alarms** to rollback on failures.
* Keep secrets out of CI/CD – fetch them at runtime via **Secrets Manager/Parameter Store**.
