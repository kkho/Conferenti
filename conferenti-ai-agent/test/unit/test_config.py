"""
Unit tests for configuration management.
"""

import os
import pytest
from unittest.mock import patch, MagicMock
from conferenti_agent.config import Settings, get_settings


class TestSettings:
    """Test Settings class configuration loading."""

    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "AUTH0_DOMAIN": "test.auth0.com",
            "BYPASS_KEY_VAULT": "true",
            "COSMOS_DB_USE_LOCAL": "true",
        },
        clear=True,
    )
    def test_settings_load_from_env(self):
        """Test loading settings from environment variables."""
        settings = Settings()

        assert settings.project_endpoint == "http://localhost:11434"
        assert settings.model_deployment_name == "llama3.2"
        assert settings.auth0_domain == "test.auth0.com"
        assert settings.bypass_key_vault is True
        assert settings.cosmos_db_use_local is True

    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "AUTH0_DOMAIN": "test.auth0.com",
            "API_KEY": "test-api-key",
            "BYPASS_KEY_VAULT": "true",
        },
        clear=True,
    )
    def test_settings_with_api_key(self):
        """Test settings with API key provided."""
        settings = Settings()

        assert settings.api_key == "test-api-key"

    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "AUTH0_DOMAIN": "test.auth0.com",
            "BYPASS_KEY_VAULT": "false",
            "USE_KEY_VAULT_EMULATOR": "true",
            "KEY_VAULT_EMULATOR_ENDPOINT": "http://localhost:5000",
        },
        clear=True,
    )
    def test_settings_with_key_vault_emulator(self):
        """Test settings configured for Key Vault emulator."""
        settings = Settings()

        assert settings.bypass_key_vault is False
        assert settings.use_key_vault_emulator is True
        assert settings.key_vault_emulator_endpoint == "http://localhost:5000"

    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "AUTH0_DOMAIN": "test.auth0.com",
            "COSMOS_DB_ENDPOINT": "https://test.documents.azure.com:443/",
            "COSMOS_DB_DATABASE_NAME": "TestDatabase",
            "COSMOS_DB_SPEAKER_CONTAINER": "Speakers",
            "COSMOS_DB_SESSION_CONTAINER": "Sessions",
            "BYPASS_KEY_VAULT": "true",
        },
        clear=True,
    )
    def test_cosmos_db_configuration(self):
        """Test Cosmos DB configuration."""
        settings = Settings()

        assert settings.cosmos_db_endpoint == "https://test.documents.azure.com:443/"
        assert settings.cosmos_db_database_name == "TestDatabase"
        assert settings.cosmos_db_speaker_container == "Speakers"
        assert settings.cosmos_db_session_container == "Sessions"

    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "AUTH0_DOMAIN": "test.auth0.com",
            "DISABLE_AUTH": "true",
            "BYPASS_KEY_VAULT": "true",
        },
        clear=True,
    )
    def test_auth_disabled(self):
        """Test authentication disabled configuration."""
        settings = Settings()

        assert settings.disable_auth is True

    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "AUTH0_DOMAIN": "test.auth0.com",
            "LOG_LEVEL": "DEBUG",
            "BYPASS_KEY_VAULT": "true",
        },
        clear=True,
    )
    def test_log_level_configuration(self):
        """Test log level configuration."""
        settings = Settings()

        assert settings.log_level == "DEBUG"

    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "AUTH0_DOMAIN": "test.auth0.com",
            "CONFERENTI_API_URL": "https://api.conferenti.com/api",
            "BYPASS_KEY_VAULT": "true",
        },
        clear=True,
    )
    def test_conferenti_api_url(self):
        """Test Conferenti API URL configuration."""
        settings = Settings()

        assert settings.conferenti_api_url == "https://api.conferenti.com/api"
