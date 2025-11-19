"""
Unit tests for Key Vault configuration.
"""

import os
import pytest
from unittest.mock import patch, MagicMock
from conferenti_agent.keyvault import KeyVaultConfig, get_keyvault_config


class TestKeyVaultConfig:
    """Test KeyVaultConfig class."""

    @patch.dict(os.environ, {"BYPASS_KEY_VAULT": "true"}, clear=True)
    def test_bypass_mode(self):
        """Test Key Vault in bypass mode."""
        kv = KeyVaultConfig()

        assert kv.bypass is True
        assert kv.client is None

    @patch.dict(
        os.environ,
        {
            "BYPASS_KEY_VAULT": "false",
            "USE_KEY_VAULT_EMULATOR": "true",
            "KEY_VAULT_EMULATOR_ENDPOINT": "http://localhost:5000",
        },
        clear=True,
    )
    @patch("conferenti_agent.keyvault.SecretClient")
    @patch("conferenti_agent.keyvault.DefaultAzureCredential")
    def test_emulator_mode_success(self, mock_credential, mock_secret_client):
        """Test Key Vault emulator mode with successful connection."""
        mock_client_instance = MagicMock()
        mock_secret_client.return_value = mock_client_instance

        kv = KeyVaultConfig()

        assert kv.bypass is False
        assert kv.use_emulator is True
        assert kv.emulator_endpoint == "http://localhost:5000"
        assert kv.client is not None

    @patch.dict(
        os.environ,
        {
            "BYPASS_KEY_VAULT": "false",
            "USE_KEY_VAULT_EMULATOR": "true",
        },
        clear=True,
    )
    @patch("conferenti_agent.keyvault.SecretClient")
    @patch("conferenti_agent.keyvault.DefaultAzureCredential")
    def test_emulator_mode_failure_fallback(self, mock_credential, mock_secret_client):
        """Test emulator mode falls back to bypass on failure."""
        mock_secret_client.side_effect = Exception("Connection failed")

        kv = KeyVaultConfig()

        assert kv.bypass is True  # Should fallback
        assert kv.client is None

    @patch.dict(
        os.environ,
        {
            "BYPASS_KEY_VAULT": "false",
            "AZURE_KEY_VAULT_NAME": "test-vault",
        },
        clear=True,
    )
    @patch("conferenti_agent.keyvault.SecretClient")
    @patch("conferenti_agent.keyvault.DefaultAzureCredential")
    def test_production_mode_with_managed_identity(
        self, mock_credential, mock_secret_client
    ):
        """Test production mode using Managed Identity."""
        mock_client_instance = MagicMock()
        mock_secret_client.return_value = mock_client_instance
        mock_credential_instance = MagicMock()
        mock_credential.return_value = mock_credential_instance

        kv = KeyVaultConfig()

        assert kv.bypass is False
        assert kv.vault_name == "test-vault"
        # Verify DefaultAzureCredential was called
        mock_credential.assert_called()

    @patch.dict(
        os.environ,
        {
            "BYPASS_KEY_VAULT": "false",
            "AZURE_KEY_VAULT_NAME": "test-vault",
            "AZURE_TENANT_ID": "tenant-123",
            "AZURE_CLIENT_ID": "client-456",
            "AZURE_CLIENT_SECRET": "secret-789",
        },
        clear=True,
    )
    @patch("conferenti_agent.keyvault.SecretClient")
    @patch("conferenti_agent.keyvault.ClientSecretCredential")
    def test_production_mode_with_service_principal(
        self, mock_credential, mock_secret_client
    ):
        """Test production mode using Service Principal."""
        mock_client_instance = MagicMock()
        mock_secret_client.return_value = mock_client_instance
        mock_credential_instance = MagicMock()
        mock_credential.return_value = mock_credential_instance

        kv = KeyVaultConfig()

        assert kv.bypass is False
        assert kv.vault_name == "test-vault"
        mock_credential.assert_called_once_with(
            tenant_id="tenant-123", client_id="client-456", client_secret="secret-789"
        )

    @patch.dict(os.environ, {"BYPASS_KEY_VAULT": "true"}, clear=True)
    def test_get_secret_in_bypass_mode(self):
        """Test get_secret returns default in bypass mode."""
        kv = KeyVaultConfig()

        secret = kv.get_secret("TEST_SECRET", "default-value")

        assert secret == "default-value"

    @patch.dict(
        os.environ,
        {
            "BYPASS_KEY_VAULT": "false",
            "USE_KEY_VAULT_EMULATOR": "true",
        },
        clear=True,
    )
    @patch("conferenti_agent.keyvault.SecretClient")
    @patch("conferenti_agent.keyvault.DefaultAzureCredential")
    def test_get_secret_from_vault(self, mock_credential, mock_secret_client):
        """Test get_secret retrieves from Key Vault."""
        mock_client = MagicMock()
        mock_secret = MagicMock()
        mock_secret.value = "vault-secret-value"
        mock_client.get_secret.return_value = mock_secret
        mock_secret_client.return_value = mock_client

        kv = KeyVaultConfig()
        secret = kv.get_secret("TEST_SECRET", "default-value")

        assert secret == "vault-secret-value"
        mock_client.get_secret.assert_called_once_with("TEST_SECRET")

    @patch.dict(
        os.environ,
        {
            "BYPASS_KEY_VAULT": "false",
            "USE_KEY_VAULT_EMULATOR": "true",
        },
        clear=True,
    )
    @patch("conferenti_agent.keyvault.SecretClient")
    @patch("conferenti_agent.keyvault.DefaultAzureCredential")
    def test_get_secret_fallback_on_error(self, mock_credential, mock_secret_client):
        """Test get_secret falls back to default on error."""
        mock_client = MagicMock()
        mock_client.get_secret.side_effect = Exception("Secret not found")
        mock_secret_client.return_value = mock_client

        kv = KeyVaultConfig()
        secret = kv.get_secret("MISSING_SECRET", "fallback-value")

        assert secret == "fallback-value"


class TestKeyVaultSingleton:
    """Test Key Vault singleton pattern."""

    @patch.dict(os.environ, {"BYPASS_KEY_VAULT": "true"}, clear=True)
    def test_singleton_returns_same_instance(self):
        """Test get_keyvault_config returns singleton."""
        # Clear the singleton first
        import conferenti_agent.keyvault as kv_module

        kv_module._keyvault_config = None

        kv1 = get_keyvault_config()
        kv2 = get_keyvault_config()

        assert kv1 is kv2
