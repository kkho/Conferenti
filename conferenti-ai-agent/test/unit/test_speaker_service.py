"""
Unit tests for SpeakerService.
"""

import pytest
from unittest.mock import AsyncMock, MagicMock, patch
from conferenti_agent.services.speaker_service import (
    SpeakerService,
    get_speaker_service,
)


@pytest.fixture
def mock_settings():
    """Mock settings for testing."""
    with patch("conferenti_agent.services.speaker_service.get_settings") as mock:
        settings = MagicMock()
        settings.project_endpoint = "http://localhost:11434"
        settings.model_deployment_name = "llama3.2"
        settings.api_key = None
        mock.return_value = settings
        yield settings


@pytest.fixture
def mock_agent_client():
    """Mock agent client for testing."""
    with patch("conferenti_agent.services.speaker_service.create_agent_client") as mock:
        client = MagicMock()
        mock.return_value = client
        yield client


@pytest.fixture
def mock_db_client():
    """Mock database client for testing."""
    with patch("conferenti_agent.services.speaker_service.get_db_client") as mock:
        db = AsyncMock()
        mock.return_value = db
        yield db


@pytest.fixture
def speaker_service(mock_settings, mock_agent_client, mock_db_client):
    """Create SpeakerService instance with mocked dependencies."""
    return SpeakerService()


class TestSpeakerService:
    """Test SpeakerService methods."""

    @pytest.mark.asyncio
    async def test_get_speaker_success(self, speaker_service, mock_db_client):
        """Test getting a speaker by ID."""
        expected_speaker = {
            "id": "speaker-123",
            "name": "John Doe",
            "position": "CTO",
            "company": "Tech Corp",
        }
        mock_db_client.get_speaker_by_id.return_value = expected_speaker

        result = await speaker_service.get_speaker("speaker-123")

        assert result == expected_speaker
        mock_db_client.get_speaker_by_id.assert_called_once_with("speaker-123")

    @pytest.mark.asyncio
    async def test_get_speaker_not_found(self, speaker_service, mock_db_client):
        """Test getting a non-existent speaker."""
        mock_db_client.get_speaker_by_id.return_value = None

        result = await speaker_service.get_speaker("non-existent")

        assert result is None

    @pytest.mark.asyncio
    async def test_list_all_speakers(self, speaker_service, mock_db_client):
        """Test listing all speakers."""
        expected_speakers = [
            {"id": "1", "name": "Speaker 1"},
            {"id": "2", "name": "Speaker 2"},
        ]
        mock_db_client.get_all_speakers.return_value = expected_speakers

        result = await speaker_service.list_all_speakers(limit=10)

        assert result == expected_speakers
        mock_db_client.get_all_speakers.assert_called_once_with(max_items=10)

    @pytest.mark.asyncio
    async def test_search_speakers(self, speaker_service, mock_db_client):
        """Test searching speakers."""
        expected_results = [{"id": "1", "name": "John Doe", "company": "Tech Corp"}]
        mock_db_client.search_speakers.return_value = expected_results

        result = await speaker_service.search_speakers("John")

        assert result == expected_results
        mock_db_client.search_speakers.assert_called_once_with("John")

    @pytest.mark.asyncio
    async def test_suggest_speakers_general(self, speaker_service, mock_agent_client):
        """Test general speaker suggestions."""
        mock_agent = MagicMock()
        mock_agent.run.return_value = {
            "status": "completed",
            "content": "Here are 3 suggested speakers for cloud computing...",
        }
        mock_agent_client.create_agent.return_value = mock_agent

        result = await speaker_service.suggest_speakers_general(
            session_theme="Cloud Computing", topics=["AWS", "Azure", "DevOps"], count=3
        )

        assert "suggested speakers" in result.lower()
        mock_agent_client.create_agent.assert_called_once()
        mock_agent.run.assert_called_once()

    @pytest.mark.asyncio
    async def test_suggest_speakers_for_session(
        self, speaker_service, mock_db_client, mock_agent_client
    ):
        """Test speaker suggestions for specific session."""
        mock_session = {
            "id": "session-123",
            "name": "AI Conference 2024",
            "theme": "Machine Learning",
            "target_audience": "Data Scientists",
            "topics": ["Deep Learning", "NLP"],
        }
        mock_db_client.get_session_by_id.return_value = mock_session

        mock_agent = MagicMock()
        mock_agent.run.return_value = {
            "status": "completed",
            "content": "Suggested speakers for ML session...",
        }
        mock_agent_client.create_agent.return_value = mock_agent

        result = await speaker_service.suggest_speakers_for_session(
            "session-123", count=5
        )

        assert "speakers" in result.lower()
        mock_db_client.get_session_by_id.assert_called_once_with("session-123")

    @pytest.mark.asyncio
    async def test_suggest_speakers_for_session_not_found(
        self, speaker_service, mock_db_client
    ):
        """Test speaker suggestions for non-existent session."""
        mock_db_client.get_session_by_id.return_value = None

        with pytest.raises(ValueError, match="Session .* not found"):
            await speaker_service.suggest_speakers_for_session("missing-session")

    @pytest.mark.asyncio
    async def test_generate_speaker_bio(
        self, speaker_service, mock_db_client, mock_agent_client
    ):
        """Test generating speaker bio."""
        mock_speaker = {
            "id": "speaker-123",
            "name": "Jane Smith",
            "position": "Senior Engineer",
            "company": "Tech Innovations",
            "email": "jane@example.com",
            "bio": "Experienced developer",
        }
        mock_db_client.get_speaker_by_id.return_value = mock_speaker

        # Mock get_sessions_for_speaker method
        with patch.object(
            speaker_service, "get_sessions_for_speaker", new_callable=AsyncMock
        ) as mock_sessions:
            mock_sessions.return_value = [
                {"title": "Intro to Python"},
                {"title": "Advanced FastAPI"},
            ]

            mock_agent = MagicMock()
            mock_agent.run.return_value = {
                "status": "completed",
                "content": "Jane Smith is a Senior Engineer with extensive experience...",
            }
            mock_agent_client.create_agent.return_value = mock_agent

            result = await speaker_service.generate_speaker_bio("speaker-123")

            assert "Jane Smith" in result
            assert "Senior Engineer" in result

    @pytest.mark.asyncio
    async def test_generate_speaker_bio_not_found(
        self, speaker_service, mock_db_client
    ):
        """Test generating bio for non-existent speaker."""
        mock_db_client.get_speaker_by_id.return_value = None

        with pytest.raises(ValueError, match="Speaker .* not found"):
            await speaker_service.generate_speaker_bio("missing-speaker")


class TestSpeakerServiceSingleton:
    """Test speaker service singleton pattern."""

    def test_singleton_returns_same_instance(
        self, mock_settings, mock_agent_client, mock_db_client
    ):
        """Test get_speaker_service returns singleton."""
        # Clear singleton
        import conferenti_agent.services.speaker_service as service_module

        service_module._speaker_service = None

        service1 = get_speaker_service()
        service2 = get_speaker_service()

        assert service1 is service2
