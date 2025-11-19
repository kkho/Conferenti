"""
Integration tests for AI agent functionality.
"""

import pytest
import os
from unittest.mock import patch, MagicMock


@pytest.mark.skipif(
    os.getenv("SKIP_AGENT_TESTS") == "true",
    reason="Agent tests require Ollama or Azure OpenAI to be running",
)
class TestAgentIntegration:
    """Integration tests for AI agent (requires actual AI service)."""

    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "BYPASS_KEY_VAULT": "true",
        },
    )
    def test_agent_client_creation(self):
        """Test creating an agent client."""
        from conferenti_agent.agent import create_agent_client

        client = create_agent_client()
        assert client is not None

    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "BYPASS_KEY_VAULT": "true",
        },
    )
    def test_agent_basic_run(self):
        """Test running a simple agent query."""
        from conferenti_agent.agent import create_agent_client

        client = create_agent_client()
        agent = client.create_agent(
            name="test_agent", instructions="You are a helpful assistant."
        )

        response = agent.run("What is 2+2?")

        assert response is not None
        assert "status" in response
        assert response["status"] == "completed"
        assert "content" in response
        assert len(response["content"]) > 0


@pytest.mark.asyncio
class TestDatabaseIntegration:
    """Integration tests for database operations."""

    @pytest.mark.skipif(
        os.getenv("SKIP_DB_TESTS") == "true",
        reason="Database tests require Cosmos DB emulator",
    )
    @patch.dict(
        os.environ,
        {
            "COSMOS_DB_USE_LOCAL": "true",
            "COSMOS_DB_ENDPOINT": "https://localhost:8081",
            "COSMOSDB_KEY": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            "BYPASS_KEY_VAULT": "true",
        },
    )
    async def test_database_connection(self):
        """Test database connection (requires Cosmos DB emulator)."""
        from conferenti_agent.services.database import get_db_client

        db = get_db_client()
        assert db is not None

    @pytest.mark.skipif(
        os.getenv("SKIP_DB_TESTS") == "true",
        reason="Database tests require Cosmos DB emulator",
    )
    @patch.dict(
        os.environ,
        {
            "COSMOS_DB_USE_LOCAL": "true",
            "COSMOS_DB_ENDPOINT": "https://localhost:8081",
            "COSMOSDB_KEY": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            "COSMOS_DB_DATABASE_NAME": "TestDatabase",
            "COSMOS_DB_SPEAKER_CONTAINER": "SpeakerContainer",
            "COSMOS_DB_SESSION_CONTAINER": "SessionContainer",
            "BYPASS_KEY_VAULT": "true",
        },
    )
    @patch("conferenti_agent.services.database.CosmosClient")
    async def test_get_all_speakers_empty(self, mock_cosmos_client):
        """Test getting speakers from empty database (mocked)."""
        # Mock the Cosmos DB client to avoid actual connection
        mock_db = MagicMock()
        mock_container = MagicMock()
        mock_container.query_items.return_value = []
        mock_db.get_container_client.return_value = mock_container
        mock_cosmos_client.return_value.get_database_client.return_value = mock_db
        
        from conferenti_agent.services.database import get_db_client, _db_client
        import conferenti_agent.services.database as db_module
        
        # Clear singleton
        db_module._db_client = None
        
        db = get_db_client()
        speakers = await db.get_all_speakers(max_items=10)

        assert isinstance(speakers, list)
        assert len(speakers) == 0


class TestEndToEndSpeakerSuggestion:
    """End-to-end test for speaker suggestion flow."""

    @pytest.mark.skipif(
        os.getenv("SKIP_E2E_TESTS") == "true",
        reason="E2E tests require all services running",
    )
    @patch.dict(
        os.environ,
        {
            "PROJECT_ENDPOINT": "http://localhost:11434",
            "MODEL_DEPLOYMENT_NAME": "llama3.2",
            "BYPASS_KEY_VAULT": "true",
            "COSMOS_DB_USE_LOCAL": "true",
        },
    )
    @pytest.mark.asyncio
    async def test_full_speaker_suggestion_flow(self):
        """Test complete flow from API to agent to response."""
        from conferenti_agent.services.speaker_service import get_speaker_service

        service = get_speaker_service()

        result = await service.suggest_speakers_general(
            session_theme="Cloud Computing",
            topics=["AWS", "Azure", "Kubernetes"],
            target_audience="DevOps Engineers",
            count=3,
        )

        assert result is not None
        assert len(result) > 0
        # Check that result contains relevant keywords
        result_lower = result.lower()
        assert any(word in result_lower for word in ["speaker", "expert", "suggest"])
