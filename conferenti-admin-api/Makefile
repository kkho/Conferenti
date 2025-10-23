CC := go
BINARY_NAME	:= conferentiadmin

GREEN  := $(shell tput -Txterm setaf 2)
YELLOW := $(shell tput -Txterm setaf 3)
WHITE  := $(shell tput -Txterm setaf 7)
CYAN   := $(shell tput -Txterm setaf 6)
RESET  := $(shell tput -Txterm sgr0)

.DEFAULT_GOAL := all
.PHONY: test test-unit test-coverage test-race clean build

all: build

# Run all tests
test: test-unit test-coverage clean build

# Run unit tests
test-unit:
	@echo "${GREEN}Running unit tests...${RESET}"
	$(CC) test -v ./...

# Run unit tests with coverage
test-coverage:
	@echo "${GREEN}Running tests with coverage...${RESET}"
	$(CC) test -v -coverprofile=coverage.out ./...
	$(CC) tool cover -html=coverage.out -o coverage.html
	@echo "${CYAN}Coverage report generated: coverage.html${RESET}"

# Run unit tests with race detection
test-race:
	@echo "${GREEN}Running tests with race detection...${RESET}"
	$(CC) test -v -race ./...

# Run integration tests
test-integration:
	@echo "${GREEN}Running integration tests...${RESET}"
	$(CC) test -v -tags=integration ./tests/integration/...

# Run specific package tests
test-repos:
	@echo "${GREEN}Running repository tests...${RESET}"
	$(CC) test -v ./repositories/...

test-services:
	@echo "${GREEN}Running service tests...${RESET}"
	$(CC) test -v ./services/...

test-handlers:
	@echo "${GREEN}Running handler tests...${RESET}"
	$(CC) test -v ./handlers/...

test-middleware:
	@echo "${GREEN}Running middleware tests...${RESET}"
	$(CC) test -v ./middleware/...

# Run benchmarks
benchmark:
	@echo "${GREEN}Running benchmarks...${RESET}"
	$(CC) test -v -bench=. -benchmem ./...

# Install test dependencies
test-deps:
	@echo "${GREEN}Installing test dependencies...${RESET}"
	$(CC) get github.com/stretchr/testify/assert
	$(CC) get github.com/stretchr/testify/mock
	$(CC) get github.com/stretchr/testify/suite
	$(CC) mod tidy

# Clean test artifacts
test-clean:
	@echo "${GREEN}Cleaning test artifacts...${RESET}"
	rm -f coverage.out coverage.html
	$(CC) clean -testcache

# Build the application
build:
	@echo "${GREEN}Building application...${RESET}"
	mkdir -p bin
	$(CC) build -o bin/main ./cmd/admin-api

# Clean build artifacts
clean:
	@echo "${GREEN}Cleaning build artifacts...${RESET}"
	rm -f $(BINARY_NAME)
	$(CC) clean

# Install dependencies
deps:
	@echo "${GREEN}Installing dependencies...${RESET}"
	$(CC) mod download
	$(CC) mod tidy

