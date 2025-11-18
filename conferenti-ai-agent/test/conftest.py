"""
Pytest configuration and shared fixtures.
"""

import os
import pytest
from typing import Generator


@pytest.fixture(scope="session", autouse=True)
def setup_test_environment():
    """Setup test environment before running tests."""
    # Set default test environment variables
    os.environ.setdefault("BYPASS_KEY_VAULT", "true")
    os.environ.setdefault("PROJECT_ENDPOINT", "http://localhost:11434")
    os.environ.setdefault("MODEL_DEPLOYMENT_NAME", "llama3.2")
    os.environ.setdefault("AUTH0_DOMAIN", "test.auth0.com")
    os.environ.setdefault("DISABLE_AUTH", "true")
    os.environ.setdefault("COSMOS_DB_USE_LOCAL", "true")
    os.environ.setdefault("LOG_LEVEL", "DEBUG")

    yield

    # Cleanup if needed


@pytest.fixture
def clean_environment() -> Generator:
    """Provide clean environment for each test."""
    # Store original environment
    original_env = os.environ.copy()

    yield

    # Restore original environment
    os.environ.clear()
    os.environ.update(original_env)


@pytest.fixture
def sample_speaker():
    """Provide sample speaker data for tests."""
    return {
        "id": "speaker-001",
        "name": "Dr. Jane Smith",
        "position": "Chief Technology Officer",
        "company": "Tech Innovations Inc.",
        "email": "jane.smith@techinnovations.com",
        "bio": "Dr. Jane Smith is a renowned expert in cloud computing with over 15 years of experience.",
        "expertise": ["Cloud Computing", "DevOps", "Kubernetes"],
        "linkedin": "https://linkedin.com/in/janesmith",
        "twitter": "@janesmith",
    }


@pytest.fixture
def sample_session():
    """Provide sample session data for tests."""
    return {
        "id": "session-001",
        "name": "Cloud Native Conference 2024",
        "theme": "Modern Cloud Architecture",
        "description": "Exploring the latest in cloud-native technologies",
        "target_audience": "Software Engineers and Architects",
        "topics": ["Kubernetes", "Microservices", "Service Mesh", "Cloud Security"],
        "duration_minutes": 45,
        "max_attendees": 200,
    }


@pytest.fixture
def sample_speakers_list():
    """Provide list of sample speakers for tests."""
    return [
        {
            "id": "speaker-001",
            "name": "John Doe",
            "position": "Senior Engineer",
            "company": "Cloud Corp",
        },
        {
            "id": "speaker-002",
            "name": "Jane Smith",
            "position": "Tech Lead",
            "company": "DevOps Inc",
        },
        {
            "id": "speaker-003",
            "name": "Bob Johnson",
            "position": "Architect",
            "company": "System Solutions",
        },
    ]


# Markers for different test categories
def pytest_configure(config):
    """Configure custom pytest markers."""
    config.addinivalue_line(
        "markers", "unit: Unit tests that don't require external services"
    )
    config.addinivalue_line(
        "markers", "integration: Integration tests that may require external services"
    )
    config.addinivalue_line("markers", "e2e: End-to-end tests requiring all services")
    config.addinivalue_line("markers", "slow: Tests that take a long time to run")
