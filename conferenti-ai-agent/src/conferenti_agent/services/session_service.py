# session_service.py

from datetime import datetime, timezone
import logging
from typing import Any, Dict, List, Optional
import os
from conferenti_agent.prompts import (
    SUGGEST_SESSIONS_PROMPT,
    SUGGEST_SESSION_AGENDA_PROMPT,
    SUGGEST_SPEAKER_TOPICS_PROMPT,
)

from conferenti_agent.agent import create_agent_client
from conferenti_agent.services.database import get_db_client
from conferenti_agent.config import get_settings

logger = logging.getLogger(__name__)

instructions = "You are a helpful conference planning assistant."


class SessionService:
    """Service for session operations with AI and Cosmos DB."""

    def __init__(self):
        self.settings = get_settings()

        if not os.getenv("PROJECT_ENDPOINT"):
            os.environ["PROJECT_ENDPOINT"] = self.settings.project_endpoint
        if not os.getenv("MODEL_DEPLOYMENT_NAME"):
            os.environ["MODEL_DEPLOYMENT_NAME"] = self.settings.model_deployment_name
        if not os.getenv("API_KEY") and self.settings.api_key:
            os.environ["API_KEY"] = self.settings.api_key

        self.agent_client = create_agent_client()
        self.db = get_db_client()

    async def suggest_general(self, query: str, context: Optional[str] = None) -> str:
        """
        General session suggestion based on user query

        Args:
            query: User's question about sessions
            context: Optional conversation context

        Returns:
            Dict with suggestion and metadata
        """
        try:
            sessions = await self.db.get_all_sessions()

            if not sessions:
                return {
                    "suggestion": "I couldn't find any sessions at the moment.",
                    "sessions": [],
                    "query": query,
                }

            prompt = self._build_prompt(query, sessions, context)
            # Get AI response
            logger.info(f"Generating session suggestion for query: {query}")

            agent = self.agent_client.create_agent(
                name="session_suggester_general",
                instructions=instructions,
            )
            response = agent.run(prompt)

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

        except Exception as e:
            logger.error(f"Error in suggest_general: {str(e)}")
            raise

    async def suggest_by_topic(self, topic: str) -> Dict[str, Any]:
        """
        Suggest sessions based on specific topic/technology

        Args:
            topic: Topic or technology of interest

        Returns:
          Dict with matching sessions and AI summary
        """
        try:
            sessions = await self.db.get_sessions_by_topic(topic)

            if not sessions:
                return {
                    "suggestion": f"I couldn't find any sessions about {topic}.",
                    "sessions": [],
                    "topic": topic,
                }

            prompt = f"""You are a conference assistant. The user is interested in sessions about: {topic}

                Found sessions:
                {self._format_sessions_for_prompt(sessions)}

                Provide a helpful summary of these sessions, highlighting:
                1. Brief overview of each session
                2. Key topics covered
                3. Recommendation based on the topic interest
                4. Time slots and scheduling suggestions

                Be concise and friendly."""

            agent = self.agent_client.create_agent(
                name="session_suggester_topic",
                instructions=instructions,
            )

            response = agent.run(prompt)

            return {
                "suggestion": response,
                "sessions": sessions,
                "topic": topic,
                "count": len(sessions),
            }

        except Exception as e:
            logger.error(f"Error in suggest_by_topic: {str(e)}")
            raise

    async def suggest_by_speaker(self, speaker_id: str) -> Dict[str, Any]:
        """
        Find sessions by a specific speaker
        Args:
            speaker_id: Id of the speaker

        Returns:
            Dict with speakers's sessions
        """
        try:
            sessions = await self.db.suggest_session_by_speaker(speaker_id)

            if not sessions:
                return {
                    "suggestion": f"I couldn't find any sessions about {speaker_id}.",
                    "sessions": [],
                    "speaker_id": speaker_id,
                }

            prompt = f"""You are a conference assistant.
            
            The speaker has the following sessions:
            {self._format_sessions_for_prompt(sessions)}
            
            Provide a brief, engaging summary of these sessions, including:
            1. Session titles and topics
            2. What attendees can expect to learn
            3. Scheduling information
            
            Be concise and enthusiastic."""

            agent = self.agent_client.create_agent(
                name="session_suggester_speaker",
                instructions=instructions,
            )
            response = agent.run(prompt)

            return {
                "suggestion": response,
                "sessions": sessions,
                "speaker_id": speaker_id,
                "count": len(sessions),
            }

        except Exception as e:
            logger.error(f"Error in suggest_by_speaker: {str(e)}")
            raise

    async def suggest_by_time(
        self, date: str = None, time_slot: str = None
    ) -> Dict[str, Any]:
        """
        Suggest sessions by date or time slot

        Args:
          date: Date in ISO format (YYYY-MM-DD)
          time_slot: TIme slot like "morning", "afternoon", "evening"

        Returns:
            Dict with sessions in that time range
        """
        try:
            sessions = await self.db.get_sessions_by_time(date, time_slot)
            if not sessions:
                return {
                    "suggestion": f"No sessions found for {date or ''} {time_slot or ''}",
                    "sessions": [],
                    "date": date,
                    "time_slot": time_slot,
                }

            prompt = f"""You are a conference assistant.
          Sessions scheduled for {date or ''} {time_slot or ''}
          {self._format_sessions_for_prompt(sessions)}
          
          Provide a helpful schedule overview including:
          1. List of sessions with times
          2. Suggestions for which sessions to attend
          3. Any scheduling conflicts to be aware of
          
          Be organized and helpful.
          """
            agent = self.agent_client.create_agent(
                name="session_suggester_time",
                instructions=instructions,
            )
            response = agent.run(prompt)

            return {
                "suggestion": response,
                "sessions": sessions,
                "date": date,
                "time_slot": time_slot,
                "count": len(sessions),
            }

        except Exception as e:
            logger.error(f"Error in suggest_by_time: {str(e)}")
            raise

    def _format_sessions_for_prompt(self, sessions: List[Dict[str, Any]]) -> str:
        """Format session data for AI prompt"""
        formatted = []

        for session in sessions:
            session_info = f"""
Title: {session.get('title', 'N/A')}
Description: {session.get('description', 'N/A')}
Start Time: {session.get('startTime', 'N/A')}
End Time: {session.get('endTime', 'N/A')}
Room: {session.get('room', 'N/A')}
Track: {session.get('track', 'N/A')}
Level: {session.get('level', 'N/A')}
Tags: {', '.join(session.get('tags', []))}
---"""
            formatted.append(session_info)

        return "\n".join(formatted)

    def _build_prompt(
        self, query: str, sessions: List[Dict[str, Any]], context: Optional[str] = None
    ) -> str:
        """Build comprehensive prompt for AI agent"""

        base_prompt = f"""You are a helpful conference assistant for Conferenti 2025.

Available sessions:
{self._format_sessions_for_prompt(sessions)}

"""

        if context:
            base_prompt += f"""Previous conversation context:
{context}

"""

        base_prompt += f"""User question: {query}

Provide a helpful, accurate response about the sessions. Include:
1. Relevant session recommendations
2. Key details (time, room, speakers if applicable)
3. Why these sessions might be interesting
4. Any scheduling tips or suggestions

Be concise, friendly, and informative."""

        return base_prompt
