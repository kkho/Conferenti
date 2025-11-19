"""
Quick test script to verify Key Vault emulator setup.

Usage:
    # Test bypass mode (default)
    python test_keyvault_setup.py
    
    # Test emulator mode
    # First, set environment variables:
    $env:BYPASS_KEY_VAULT="false"; $env:USE_KEY_VAULT_EMULATOR="true"
    python test_keyvault_setup.py
    
    # Test production mode
    $env:BYPASS_KEY_VAULT="false"; $env:USE_KEY_VAULT_EMULATOR="false"; $env:AZURE_KEY_VAULT_NAME="my-vault"
    python test_keyvault_setup.py
"""

import os
from conferenti_agent.keyvault import get_keyvault_config

def test_keyvault_setup():
    print("=" * 60)
    print("Testing Key Vault Configuration")
    print("=" * 60)
    
    # Display current configuration
    print("\n📋 Current Configuration:")
    print(f"   BYPASS_KEY_VAULT: {os.getenv('BYPASS_KEY_VAULT', 'true')}")
    print(f"   USE_KEY_VAULT_EMULATOR: {os.getenv('USE_KEY_VAULT_EMULATOR', 'false')}")
    print(f"   AZURE_KEY_VAULT_NAME: {os.getenv('AZURE_KEY_VAULT_NAME', 'not set')}")
    print(f"   KEY_VAULT_EMULATOR_ENDPOINT: {os.getenv('KEY_VAULT_EMULATOR_ENDPOINT', 'http://localhost:5000')}")
    
    print("\n🔄 Initializing Key Vault...")
    kv = get_keyvault_config()
    
    print("\n📊 Key Vault Status:")
    print(f"   Bypass mode: {kv.bypass}")
    print(f"   Emulator mode: {kv.use_emulator}")
    print(f"   Client initialized: {kv.client is not None}")
    
    # Test secret retrieval
    print("\n🔑 Testing Secret Retrieval:")
    
    # Set some test environment variables
    os.environ["TEST_API_KEY"] = "test-api-key-from-env"
    os.environ["TEST_COSMOS_KEY"] = "test-cosmos-key-from-env"
    
    api_key = kv.get_secret("CONFERENTI-API-KEY", os.getenv("TEST_API_KEY"))
    cosmos_key = kv.get_secret("COSMOS-DB-KEY", os.getenv("TEST_COSMOS_KEY"))
    
    print(f"   API Key: {'✅ Retrieved' if api_key else '❌ Not found'}")
    print(f"   Cosmos Key: {'✅ Retrieved' if cosmos_key else '❌ Not found'}")
    
    if kv.bypass:
        print("\n💡 Bypass mode active - using environment variables")
        print("   This is the default for local development")
    elif kv.use_emulator:
        print("\n💡 Emulator mode active")
        print(f"   Endpoint: {kv.emulator_endpoint}")
        print("   To add secrets to emulator, use your emulator's API or CLI")
    else:
        print("\n💡 Production mode active")
        print(f"   Vault: {kv.vault_name}")
    
    print("\n" + "=" * 60)
    print("✅ Key Vault setup test complete!")
    print("=" * 60)

if __name__ == "__main__":
    test_keyvault_setup()
