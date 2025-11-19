SHELL=bin/bash

ifneq (,$(wildcard ./.env))
		include .env
		export
endif

makefile_dir := $(dir $(abspath $(lastword $(MAKEFILE_LIST))))

help: ## Show this help
	@grep -E '[a-zA-Z_-]+:.*?## .*$$' $(firstword $(MAKEFILE_LIST)) \
	| awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-32s\033[0m %s\n", $$1, $$2}'

install: ## Install dependencies
		pip install -r requirements.txt

install-dev: ## Install development dependencies
		pip install -e ".[dev]"

install-requirements:
		pip install -r aoai-api-simulator/requirements.txt

run-simulated-api: ## Run the simulated API (Windows compatible)
ifeq ($(OS),Windows_NT)
		powershell -ExecutionPolicy Bypass -File run-simulator.ps1
else
		cd aoai-api-simulator && PYTHONPATH=src gunicorn \
				aoai_api_simulator.main:app \
				--worker-class uvicorn.workers.UvicornWorker \
				--workers 1 \
				--bind 0.0.0.0:8000 \
				--timeout 3600
endif

test: ## Run tests
		pytest ./test -vv

test-unit: ## Run unit tests only
		pytest ./test/unit -vv

test-integration: ## Run integration tests only
		pytest ./test/integration -vv

test-coverage: ## Run tests with coverage report
		pytest ./test --cov=conferenti_agent --cov-report=html --cov-report=term

test-watch:
		ptw --clear ./tests

lint: ## Run linter
		pylint ./src/conferenti_agent

format: ## Format code with black
		black ./src/conferenti_agent ./test

format-check: ## Check code formatting
		black --check ./src/conferenti_agent ./test

run-dev: ## Run API in development mode
		uvicorn conferenti_agent.services.api_client:app --reload --host 0.0.0.0 --port 8000

run: ## Run API in production mode
		uvicorn conferenti_agent.services.api_client:app --host 0.0.0.0 --port 8000

docker-build-simulated-api:
		docker build -t aoai-api-simulator -f aoai-api-simulator/Dockerfile .

docker-run-simulated-api: ## Run the AOAI Simulated API docker container
	echo "makefile_dir: ${makefile_dir}"
	echo "makefile_path: ${makefile_path}"
	docker run --rm -i -t \
		-p 8000:8000 \
		-v "${makefile_dir}.recording":/mnt/recording \
		-e RECORDING_DIR=/mnt/recording \
		-e SIMULATOR_MODE \
		-e SIMULATOR_API_KEY \
		-e AZURE_OPENAI_ENDPOINT \
		-e AZURE_OPENAI_KEY \
		-e AZURE_OPENAI_DEPLOYMENT \
		aoai-api-simulator

erase-recording: ## Erase all *.recording files
	rm -rf "${makefile_dir}.recording"

docker-build: ## Build the AI agent Docker image
		docker build -t conferenti-ai-agent:latest .

docker-run: ## Run the AI agent Docker container
		docker run --rm -i -t \
			-p 8000:8000 \
			--env-file .env \
			conferenti-ai-agent:latest

clean: ## Clean up Python cache files
		find . -type d -name "__pycache__" -exec rm -rf {} + 2>/dev/null || true
		find . -type f -name "*.pyc" -delete
		find . -type f -name "*.pyo" -delete
		find . -type d -name "*.egg-info" -exec rm -rf {} + 2>/dev/null || true
		rm -rf .pytest_cache
		rm -rf htmlcov
		rm -rf .coverage

.PHONY: help install install-dev install-requirements run-simulated-api test test-unit test-integration test-coverage test-watch lint format format-check run-dev run docker-build-simulated-api docker-run-simulated-api erase-recording docker-build docker-run clean