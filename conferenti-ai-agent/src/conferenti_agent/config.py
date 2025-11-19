"""
Configuration management for Conferenti AI Agent.
"""

import os
from typing import Optional
from pydantic_settings import BaseSettings, SettingsConfigDict
from conferenti_agent.keyvault import get_keyvault_config


class Settings(BaseSettings):
    # Azure OpenAI / Ollama Configuration
    project_endpoint: str
    model_deployment_name: str
    api_key: str | None = None

    # Auth0 Configuration
    auth0_domain: str
    auth0_audience: str = "https://api.conferenti.com"
    disable_auth: bool = False

    # Azure Key Vault Configuration
    bypass_key_vault: bool = True
    use_key_vault_emulator: bool = False
    azure_key_vault_name: Optional[str] = None
    key_vault_emulator_endpoint: str = "http://localhost:5000"  # For emulator

    # Azure Authentication (for Key Vault Service Principal)
    azure_tenant_id: Optional[str] = None
    azure_client_id: Optional[str] = None
    azure_client_secret: Optional[str] = None

    # Cosmos DB Configuration
    cosmos_db_use_local: bool = True
    cosmos_db_use_docker: bool = False
    cosmos_db_endpoint: str = "https://localhost:8081"
    cosmos_db_key: Optional[str] = None
    cosmos_db_database_name: str = "ConferentiDatabase"
    cosmos_db_speaker_container: str = "SpeakerContainer"
    cosmos_db_session_container: str = "SessionContainer"

    # Conferenti API
    conferenti_api_url: str = "http://localhost:5000/api"
    conferenti_api_key: Optional[str] = None

    # Logging
    log_level: str = "INFO"

    model_config = SettingsConfigDict(
        env_file=".env",
        env_file_encoding="utf-8",
        case_sensitive=False,
        extra="ignore",
        protected_namespaces=(),
    )

    def __init__(self, **kwargs):
        """
        Initialize settings and fetch secrets from Azure Key Vault if configured.
        """
        super().__init__(**kwargs)

        self.cosmos_db_endpoint = (
            os.getenv("COSMOSDB_ENDPOINT") or self.cosmos_db_endpoint
        )

        kv = get_keyvault_config()

        if not self.api_key:
            self.api_key = (
                kv.get_secret("AI-API-KEY", self.api_key)
                if not kv.bypass
                else os.getenv("AI_API_KEY", "")
            )

        if not self.cosmos_db_key:
            self.cosmos_db_key = (
                kv.get_secret("COSMOS-DB-KEY", self.cosmos_db_key)
                if not kv.bypass
                else os.getenv("COSMOSDB_KEY", "")
            )

        if not self.conferenti_api_key:
            self.conferenti_api_key = (
                kv.get_secret("CONFERENTI-API-KEY", self.conferenti_api_key)
                if not kv.bypass
                else os.getenv("CONFERENTI_API_KEY", "")
            )


def get_settings() -> Settings:
    """Get application settings singleton."""
    return Settings()
