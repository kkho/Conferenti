# Conferenti AI Agent Tests

This directory contains the test suite for the Conferenti AI Agent.

## Test Structure

```
test/
├── unit/                      # Unit tests (no external dependencies)
│   ├── test_config.py        # Configuration tests
│   ├── test_keyvault.py      # Key Vault tests
│   └── test_speaker_service.py  # Speaker service tests
├── integration/               # Integration tests (require services)
│   ├── test_api_endpoints.py # API endpoint tests
│   └── test_agent_integration.py  # AI agent integration tests
├── conftest.py               # Shared fixtures and configuration
└── README.md                 # This file
```

## Running Tests

### Run All Tests

```powershell
pytest
```

### Run Unit Tests Only

```powershell
pytest test/unit -m unit
```

### Run Integration Tests

```powershell
pytest test/integration -m integration
```

### Run Specific Test File

```powershell
pytest test/unit/test_config.py
```

### Run Specific Test Class

```powershell
pytest test/unit/test_config.py::TestSettings
```

### Run Specific Test Method

```powershell
pytest test/unit/test_config.py::TestSettings::test_settings_load_from_env
```

### Run with Verbose Output

```powershell
pytest -v
```

### Run with Coverage Report

```powershell
pytest --cov=conferenti_agent --cov-report=html
```

## Test Categories

### Unit Tests (`test/unit/`)

- **Fast execution** (< 1 second per test)
- **No external dependencies** (mocked services)
- **Test individual components** in isolation
- Run these frequently during development

### Integration Tests (`test/integration/`)

- **May require external services** (Ollama, Cosmos DB)
- **Test component interaction**
- **Slower execution** (may take several seconds)
- Run before commits or in CI/CD

### End-to-End Tests

- **Require all services running**
- **Test complete user flows**
- **Slowest execution**
- Run before releases

## Environment Variables for Testing

Set these variables to control test behavior:

```powershell
# Skip tests that require Ollama/Azure OpenAI
$env:SKIP_AGENT_TESTS="true"

# Skip tests that require Cosmos DB
$env:SKIP_DB_TESTS="true"

# Skip end-to-end tests
$env:SKIP_E2E_TESTS="true"

# Run all tests (default)
$env:SKIP_AGENT_TESTS="false"; $env:SKIP_DB_TESTS="false"; $env:SKIP_E2E_TESTS="false"
```

## Prerequisites for Integration Tests

### Ollama (for AI agent tests)

```powershell
# Start Ollama
ollama serve

# Pull required model
ollama pull llama3.2
```

### Cosmos DB Emulator (for database tests)

```powershell
# Install Cosmos DB Emulator
# Download from: https://aka.ms/cosmosdb-emulator

# Start the emulator
# It runs on https://localhost:8081 by default
```

## Writing New Tests

### Unit Test Template

```python
"""
Unit tests for <module_name>.
"""

import pytest
from unittest.mock import Mock, patch
from conferenti_agent.<module> import <ClassToTest>


class Test<ClassName>:
    """Test <ClassName> functionality."""

    def test_<method_name>_<scenario>(self):
        """Test <method_name> when <scenario>."""
        # Arrange
        instance = <ClassToTest>()

        # Act
        result = instance.<method_name>()

        # Assert
        assert result == expected_value
```

### Integration Test Template

```python
"""
Integration tests for <feature>.
"""

import pytest


@pytest.mark.integration
class Test<Feature>Integration:
    """Integration tests for <feature>."""

    @pytest.mark.asyncio
    async def test_<scenario>(self):
        """Test <scenario> with real services."""
        # Test implementation
        pass
```

## Mocking Guidelines

- **Mock external services** (Azure services, HTTP calls)
- **Mock AI agent** for faster unit tests
- **Use AsyncMock** for async methods
- **Patch at the usage point**, not the definition

Example:

```python
@patch('conferenti_agent.services.speaker_service.create_agent_client')
def test_with_mocked_agent(mock_agent_client):
    mock_agent_client.return_value = MagicMock()
    # Test implementation
```

## Continuous Integration

Tests run automatically on:

- **Pull requests** (unit + integration tests)
- **Commits to main** (all tests including E2E)
- **Nightly builds** (full test suite with coverage)

## Troubleshooting

### Tests Fail with Import Errors

```powershell
# Install package in development mode
pip install -e .
```

### Tests Fail with Missing Dependencies

```powershell
# Install test dependencies
pip install pytest pytest-asyncio pytest-mock pytest-cov
```

### Async Tests Not Running

Check that `pytest-asyncio` is installed and `asyncio_mode = auto` is in `pytest.ini`.

### Cosmos DB Emulator Connection Fails

- Ensure emulator is running
- Check SSL certificate is trusted
- Verify endpoint: `https://localhost:8081`

## Coverage Goals

- **Unit tests**: > 80% coverage
- **Critical paths**: 100% coverage
- **Integration tests**: Focus on key workflows

## Best Practices

1. **Keep tests independent** - Each test should run in isolation
2. **Use descriptive names** - `test_suggest_speakers_when_session_not_found`
3. **Follow AAA pattern** - Arrange, Act, Assert
4. **Mock external services** - Don't hit real APIs in unit tests
5. **Clean up after tests** - Use fixtures for setup/teardown
6. **Test edge cases** - Empty lists, None values, exceptions
7. **Keep tests fast** - Unit tests should be < 1 second
8. **Test behavior, not implementation** - Focus on public API
