import os
from typing import Optional
from azure.keyvault.secrets import SecretClient
from azure.identity import DefaultAzureCredential, ClientSecretCredential


class KeyVaultConfig:
    def __init__(self):
        self.bypass = os.getenv("BYPASS_KEY_VAULT", "true").lower() == "true"
        self.use_emulator = (
            os.getenv("USE_KEY_VAULT_EMULATOR", "false").lower() == "true"
        )
        self.vault_name = os.getenv("AZURE_KEY_VAULT_NAME")
        self.emulator_endpoint = os.getenv(
            "KEY_VAULT_EMULATOR_ENDPOINT", "http://localhost:5000"
        )

        self.client: Optional[SecretClient] = None

        if self.bypass:
            print("Key Vault bypassed - using environment variables only")
            return

        if self.use_emulator:
            self._initialize_emulator_client()
        elif self.vault_name:
            self._initialize_azure_client()

    def _initialize_emulator_client(self):
        try:
            credential = DefaultAzureCredential()
            self.client = SecretClient(
                vault_url=self.emulator_endpoint, credential=credential
            )
            print(f"Connected to Key Vault emulator: {self.emulator_endpoint}")
        except Exception as e:
            print(f"Failed to connect to Key Vault emulator: {e}")
            print("Falling back to bypass mode (using .env only)")
            self.bypass = True
            self.client = None

    def _initialize_azure_client(self):
        try:
            vault_url = f"https://{self.vault_name}.vault.azure.net"

            tenant_id = os.getenv("AZURE_TENANT_ID")
            client_id = os.getenv("AZURE_CLIENT_ID")
            client_secret = os.getenv("AZURE_CLIENT_SECRET")

            if all([tenant_id, client_id, client_secret]):
                # Use Service Principal authentication
                credential = ClientSecretCredential(
                    tenant_id=tenant_id,
                    client_id=client_id,
                    client_secret=client_secret,
                )
                print(f"Using Service Principal for Key Vault: {self.vault_name}")
            else:
                # Use Managed Identity (DefaultAzureCredential)
                credential = DefaultAzureCredential()
                print(f"Using Managed Identity for Key Vault: {self.vault_name}")
            self.client = SecretClient(vault_url=vault_url, credential=credential)

        except Exception as e:
            print(f"Failed to connect to Key Vault: {e}")
            self.client = None

    def get_secret(
        self, secret_name: str, default: Optional[str] = None
    ) -> Optional[str]:
        # In bypass mode, always return default (from .env)
        if self.bypass or not self.client:
            return default

        try:
            secret = self.client.get_secret(secret_name)
            print(f"Retrieved secret: {secret_name}")
            return secret.value
        except Exception as e:
            print(f"Failed to get secret '{secret_name}': {e}")
            print(f"Using default value from environment")
            return default


# Singleton instance
_keyvault_config: Optional[KeyVaultConfig] = None


def get_keyvault_config() -> KeyVaultConfig:
    global _keyvault_config
    if _keyvault_config is None:
        _keyvault_config = KeyVaultConfig()
    return _keyvault_config
