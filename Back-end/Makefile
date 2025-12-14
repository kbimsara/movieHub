.PHONY: help build test run clean docker-up docker-down k8s-deploy k8s-delete terraform-init terraform-apply

# Variables
SOLUTION = MovieHub.sln
DOCKER_COMPOSE = docker-compose.yml
TERRAFORM_DIR = terraform
K8S_DIR = k8s

help: ## Show this help message
	@echo 'Usage: make [target]'
	@echo ''
	@echo 'Available targets:'
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "  %-20s %s\n", $$1, $$2}' $(MAKEFILE_LIST)

# Development
restore: ## Restore NuGet packages
	dotnet restore $(SOLUTION)

build: restore ## Build the solution
	dotnet build $(SOLUTION) --configuration Release --no-restore

test: ## Run tests
	dotnet test $(SOLUTION) --configuration Release --no-build --verbosity normal

clean: ## Clean build artifacts
	dotnet clean $(SOLUTION)
	find . -type d -name "bin" -o -name "obj" | xargs rm -rf

run-auth: ## Run Auth Service
	cd src/Services/Auth/MovieHub.Services.Auth.API && dotnet run

run-user: ## Run User Service
	cd src/Services/User/MovieHub.Services.User.API && dotnet run

format: ## Format code
	dotnet format $(SOLUTION)

# Docker
docker-build: ## Build all Docker images
	docker-compose -f $(DOCKER_COMPOSE) build

docker-up: ## Start all services with Docker Compose
	docker-compose -f $(DOCKER_COMPOSE) up -d

docker-down: ## Stop all services
	docker-compose -f $(DOCKER_COMPOSE) down

docker-logs: ## View Docker logs
	docker-compose -f $(DOCKER_COMPOSE) logs -f

docker-clean: ## Remove all containers and volumes
	docker-compose -f $(DOCKER_COMPOSE) down -v
	docker system prune -f

# Database
db-migrate-auth: ## Run Auth Service migrations
	dotnet ef database update --project src/Services/Auth/MovieHub.Services.Auth.Infrastructure --startup-project src/Services/Auth/MovieHub.Services.Auth.API

db-migrate-all: ## Run all migrations
	@echo "Running migrations for all services..."
	$(MAKE) db-migrate-auth
	# Add other services...

# Kubernetes
k8s-create-namespace: ## Create Kubernetes namespace
	kubectl apply -f $(K8S_DIR)/namespace.yaml

k8s-apply-config: ## Apply ConfigMaps and Secrets
	kubectl apply -f $(K8S_DIR)/configmap.yaml
	kubectl apply -f $(K8S_DIR)/secrets.yaml

k8s-deploy: k8s-create-namespace k8s-apply-config ## Deploy to Kubernetes
	kubectl apply -f $(K8S_DIR)/services/
	kubectl apply -f $(K8S_DIR)/istio/

k8s-delete: ## Delete Kubernetes resources
	kubectl delete namespace moviehub

k8s-status: ## Check Kubernetes deployment status
	kubectl get all -n moviehub

k8s-logs-auth: ## View Auth Service logs
	kubectl logs -f -l app=auth-service -n moviehub

k8s-describe-auth: ## Describe Auth Service deployment
	kubectl describe deployment auth-service -n moviehub

# Terraform
terraform-init: ## Initialize Terraform
	cd $(TERRAFORM_DIR) && terraform init

terraform-plan: ## Plan Terraform changes
	cd $(TERRAFORM_DIR) && terraform plan -var="project_id=$(GCP_PROJECT_ID)"

terraform-apply: ## Apply Terraform changes
	cd $(TERRAFORM_DIR) && terraform apply -var="project_id=$(GCP_PROJECT_ID)" -auto-approve

terraform-destroy: ## Destroy Terraform infrastructure
	cd $(TERRAFORM_DIR) && terraform destroy -var="project_id=$(GCP_PROJECT_ID)"

terraform-output: ## Show Terraform outputs
	cd $(TERRAFORM_DIR) && terraform output

# Monitoring
prometheus: ## Open Prometheus UI
	@echo "Opening Prometheus at http://localhost:9090"
	open http://localhost:9090 || xdg-open http://localhost:9090

grafana: ## Open Grafana UI
	@echo "Opening Grafana at http://localhost:3000"
	@echo "Username: admin, Password: admin"
	open http://localhost:3000 || xdg-open http://localhost:3000

# CI/CD
ci-build: restore build test ## Run CI pipeline locally

ci-docker: docker-build ## Build Docker images for CI

# Local development
dev-setup: docker-up db-migrate-all ## Complete development setup
	@echo "Development environment ready!"
	@echo "Auth Service: http://localhost:5001/swagger"
	@echo "User Service: http://localhost:5002/swagger"
	@echo "Prometheus: http://localhost:9090"
	@echo "Grafana: http://localhost:3000"

# GCP
gcp-login: ## Login to GCP
	gcloud auth login

gcp-set-project: ## Set GCP project
	gcloud config set project $(GCP_PROJECT_ID)

gcp-get-credentials: ## Get GKE credentials
	gcloud container clusters get-credentials moviehub-gke-cluster --region us-central1

# Utilities
check-deps: ## Check for outdated dependencies
	dotnet list package --outdated

update-deps: ## Update NuGet packages
	dotnet list package --outdated | grep ">" | awk '{print $$2}' | xargs -I {} dotnet add package {}

swagger-auth: ## Open Auth Service Swagger
	open http://localhost:5001/swagger || xdg-open http://localhost:5001/swagger

health-check: ## Check health of all services
	@echo "Checking service health..."
	@curl -s http://localhost:5001/health | jq || echo "Auth Service: DOWN"
	@curl -s http://localhost:5002/health | jq || echo "User Service: DOWN"
	@curl -s http://localhost:5003/health | jq || echo "Movie Service: DOWN"
