# speaker_service.py

from typing import Dict, List, Optional
import os
from conferenti_agent.prompts import (
    SUGGEST_SPEAKERS_PROMPT,
    MATCH_SPEAKER_TO_SESSIONS_PROMPT,
    SUGGEST_SPEAKER_TOPICS_PROMPT,
    GENERATE_SPEAKER_BIO_PROMPT,
)
from conferenti_agent.agent import create_agent_client
from conferenti_agent.services.database import get_db_client
from conferenti_agent.config import get_settings


class SpeakerService:
    """Service for speaker operations with AI and Cosmos DB."""

    def __init__(self):
        # Load settings first to ensure environment variables are available
        self.settings = get_settings()

        # Ensure environment variables are set for agent client
        if not os.getenv("PROJECT_ENDPOINT"):
            os.environ["PROJECT_ENDPOINT"] = self.settings.project_endpoint
        if not os.getenv("MODEL_DEPLOYMENT_NAME"):
            os.environ["MODEL_DEPLOYMENT_NAME"] = self.settings.model_deployment_name
        if not os.getenv("API_KEY") and self.settings.api_key:
            os.environ["API_KEY"] = self.settings.api_key

        self.agent_client = create_agent_client()
        self.db = get_db_client()

    async def get_speaker(self, speaker_id: str) -> Optional[Dict]:
        """Get a speaker by ID from Cosmos DB."""
        return await self.db.get_speaker_by_id(speaker_id)

    async def list_all_speakers(self, limit: int = 100) -> List[Dict]:
        """Get all speakers from Cosmos DB."""
        return await self.db.get_all_speakers(max_items=limit)

    async def get_speakers_for_session(self, session_id: str) -> List[Dict]:
        """Get all speakers for a specific session"""
        return await self.db.get_speakers_by_session(session_id)

    async def search_speakers(self, query: str) -> List[Dict]:
        """Search speakers by name, title or company"""
        return await self.db.search_speakers(query)

    async def get_sessions_for_speaker(self, speaker_id: str) -> List[Dict]:
        """Get all sessions for a specific speaker."""
        return []

    async def suggest_speakers_for_session(
        self, session_id: str, count: int = 5
    ) -> str:
        """
        AI generates speaker suggestions in conversational format.
        Returns: Formatted text for chatbot UI display.
        """
        session = await self.db.get_session_by_id(session_id)
        if not session:
            raise ValueError(f"Session {session_id} not found")

        prompt = SUGGEST_SPEAKERS_PROMPT.format(
            session_theme=session.get("theme", "General Technology"),
            target_audience=session.get("target_audience", "Professionals"),
            industry="Technology",
            topics=", ".join(session.get("topics", []) or ["General Topics"]),
            number_of_speakers=count,
        )

        agent = self.agent_client.create_agent(
            name="speaker_suggester",
            instructions="You are a helpful conference planning assistant. Provide clear, actionable speaker suggestions.",
        )
        response = agent.run(prompt, stream=False)

        # Handle response format
        if isinstance(response, dict):
            if "content" in response:
                return response["content"]
            elif "status" in response and response["status"] == "failed":
                raise Exception(
                    f"Agent failed: {response.get('error', 'Unknown error')}"
                )

        raise Exception(f"Unexpected response format: {response}")

    async def suggest_speakers_general(
        self,
        session_theme: str,
        topics: List[str],
        target_audience: str = "Professionals",
        count: int = 5,
    ) -> str:
        """
        AI generates general speaker suggestions without session context.
        Useful for initial planning.

        Returns: Formatted text for chatbot UI display.
        """
        prompt = SUGGEST_SPEAKERS_PROMPT.format(
            session_theme=session_theme,
            target_audience=target_audience,
            industry="Technology",
            topics=", ".join(topics),
            number_of_speakers=count,
        )

        agent = self.agent_client.create_agent(
            name="speaker_suggester_general",
            instructions="You are a helpful conference planning assistant.",
        )
        response = agent.run(prompt, stream=False)

        # Debug logging
        print(f"Agent response: {response}")
        print(
            f"Response keys: {response.keys() if isinstance(response, dict) else 'Not a dict'}"
        )

        # Handle different response formats
        if isinstance(response, dict):
            if "content" in response:
                return response["content"]
            elif "status" in response and response["status"] == "failed":
                raise Exception(
                    f"Agent failed: {response.get('error', 'Unknown error')}"
                )
            else:
                raise Exception(f"Unexpected response format: {response}")
        else:
            raise Exception(f"Response is not a dictionary: {type(response)}")

    async def generate_speaker_bio(self, speaker_id: str) -> str:
        """
        Generate AI bio for a speaker using their existing data.
        No conference context needed - uses speakers's sessions as context.

        Returns: Bio text for chatbot UI display
        """

        speaker = await self.db.get_speaker_by_id(speaker_id)
        if not speaker:
            raise ValueError(f"Speaker {speaker_id} not found")

        sessions = await self.get_sessions_for_speaker(speaker_id)
        session_titles = [s.get("title", "") for s in sessions]

        prompt = GENERATE_SPEAKER_BIO_PROMPT.format(
            speaker_name=speaker.get("name", ""),
            position=speaker.get("position", ""),
            company=speaker.get("company", ""),
            email=speaker.get("email", ""),
            current_bio=speaker.get("bio", "None provided"),
            session_titles=session_titles or "None yet",
        )

        agent = self.agent_client.create_agent(
            name="bio_generator",
            instructions="You are a professional biography writer.",
        )
        response = agent.run(prompt)

        return response["content"]

    async def suggest_speaker_topics(
        self, speaker_id: str, session_id: str, count: int = 3
    ) -> str:
        """
        Suggest session topics for a speaker based on their expertise and background.
        Returns: Formatted topic suggestions for chatbot UI.
        """

        speaker = await self.db.get_speaker_by_id(speaker_id)
        if not speaker:
            raise ValueError(f"Speaker {speaker_id} not found")

        session = await self.db.get_session_by_id(session_id)
        if not session:
            raise ValueError(f"Session {session_id} not found")

        prompt = SUGGEST_SPEAKER_TOPICS_PROMPT.format(
            speaker_name=speaker.get("name", ""),
            speaker_expertise=speaker.get("position", ""),
            speaker_background=speaker.get("bio", ""),
            recent_work=speaker.get("company", ""),
            session_name=session.get("name", ""),
            session_theme=session.get("theme", ""),
            target_audience=session.get("audience", ""),
            session_duration=45,
            focus_areas=", ".join(session.get("topics", [])),
            number_of_topics=count,
        )

        agent = self.agent_client.create_agent(model="llama3.2")
        response = agent.run(prompt)

        return response["content"]

    async def match_speaker_to_sessions(
        self, speaker_id: str, available_sessions: List[str]
    ) -> str:
        """
        Match a speaker to available sessions.

        Args:
            speaker_id: Speaker to match
            available_sessions: List of session Ids to match against

        Returns: Formatted matching suggestions for chatbot UI.
        """

        speaker = await self.db.get_speaker_by_id(speaker_id)
        if not speaker:
            raise ValueError(f"Speaker {speaker_id} not found")

        sessions_details = []
        for session_id in available_sessions:
            session = await self.db.get_session_by_id(session_id)
            if session:
                sessions_details.append(session)

        previous_sessions = await self.get_sessions_for_speaker(speaker_id)

        session_topics_text = "\n".join(
            [
                f" - {s.get('title', '')}: ({s.get('description', '')[:100]}...)"
                for s in sessions_details
            ]
        )

        previous_topics = ", ".join([s.get("title", "") for s in previous_sessions])

        prompt = MATCH_SPEAKER_TO_SESSIONS_PROMPT.format(
            speaker_name=speaker.get("name", ""),
            speaker_expertise=speaker.get("position", ""),
            speaker_background=speaker.get("bio", ""),
            previous_topics=previous_topics or "None",
            session_topics=session_topics_text,
            session_name=session.get("name", ""),
        )

        agent = self.agent_client.create_agent(
            name="speaker_suggester_match_sessions",
            instructions="You are a helpful conference planning assistant. Match speakers to sessions effectively.",
        )

        response = agent.run(prompt)
        return response["content"]


# Singleton instance
_speaker_service: Optional[SpeakerService] = None


def get_speaker_service() -> SpeakerService:
    global _speaker_service
    if _speaker_service is None:
        _speaker_service = SpeakerService()
    return _speaker_service
