import logging
from dataclasses import dataclass
from typing import Any, Dict, List, Optional
from azure.cosmos import CosmosClient, PartitionKey, exceptions
from conferenti_agent.config import get_settings
from conferenti_agent.types.ai_chat import ChatMessage

logger = logging.getLogger(__name__)


class CosmosDbClient:
    """Cosmos Db client for Conferenti data operations"""

    def __init__(self):
        settings = get_settings()
        if not settings.cosmos_db_endpoint or not settings.cosmos_db_key:
            raise ValueError("Cosmos DB endpoint and key must be configured in .env")

        connection_kwargs = {}
        if settings.cosmos_db_use_local:
            connection_kwargs["connection_verify"] = False
            # Prevent SDK from discovering/resolving endpoints
            connection_kwargs["enable_endpoint_discovery"] = False

        self.client = CosmosClient(
            settings.cosmos_db_endpoint, settings.cosmos_db_key, **connection_kwargs
        )
        self.database_name = settings.cosmos_db_database_name

        self.database = self.client.get_database_client(self.database_name)
        self.speaker_container = self.database.get_container_client(
            settings.cosmos_db_speaker_container
        )
        self.session_container = self.database.get_container_client(
            settings.cosmos_db_session_container
        )

        self.chat_container = self.database.create_container_if_not_exists(
            id=settings.cosmos_db_chat_container,
            partition_key=PartitionKey(path="/sessionId"),
        )

    async def get_speaker_by_id(self, speaker_id: str) -> Optional[Dict[str, Any]]:
        """Get a speaker by ID."""
        try:
            item = self.speaker_container.read_item(
                item=speaker_id, partition_key=speaker_id
            )
            return item
        except exceptions.CosmosResourceNotFoundError:
            return None

    async def get_all_speakers(self, max_items: int = 100) -> List[Dict[str, Any]]:
        """Get all speakers."""
        query = "SELECT * FROM c"
        items = list(
            self.speaker_container.query_items(
                query=query, enable_cross_partition_query=True, max_item_count=max_items
            )
        )

        return items

    async def get_speakers_by_session(self, session_id: str) -> List[Dict[str, Any]]:
        """Get all speakers for a specific session."""
        query = "SELECT * FROM c WHERE ARRAY_CONTAINS(c.sessionIds, @session_id)"
        parameters = [{"name": "@session_id", "value": session_id}]

        items = list(
            self.speaker_container.query_items(
                query=query, parameters=parameters, enable_cross_partition_query=True
            )
        )
        return items

    async def search_speakers(self, search_term: str) -> List[Dict[str, Any]]:
        """Search speakers by name, title, or company (case-insensitive)."""
        query = """
        SELECT * FROM c
        WHERE CONTAINS(LOWER(c.name), @search_term)
           OR CONTAINS(LOWER(c.position), @search_term)
           OR CONTAINS(LOWER(c.company), @search_term)
        """
        parameters = [{"name": "@search_term", "value": search_term.lower()}]

        items = list(
            self.speaker_container.query_items(
                query=query, parameters=parameters, enable_cross_partition_query=True
            )
        )
        return items

    async def get_session_by_id(self, session_id: str) -> Optional[Dict[str, Any]]:
        """Get session details by ID."""
        try:
            item = self.session_container.read_item(
                item=session_id, partition_key=session_id
            )
            return item
        except exceptions.CosmosResourceNotFoundError:
            return None
        except Exception as e:
            print(f"Error fetching session: {e}")
            return None

    async def get_sessions_by_topic(self, topic: str) -> Dict[str, Any]:
        """
        Suggest sessions based on specific topic/technology

        Args:
            topic: Technology or topic to search for

        Returns:
            Dict with matching msessions and AI summary
        """
        try:
            query = """
                SELECT * FROM c
                WHERE CONTAINS(LOWER(c.title), LOWER(@topic))
                    OR CONTAINS(LOWER(c.description), LOWER(@topic))
                    OR ARRAY_CONTAINS(c.tags, @topic, true)
                ORDER BY c.startTime
            """
            parameters = [{"name": "@topic", "value": topic}]

            sessions = list(
                self.session_container.query_items(
                    query=query, parameters=parameters, enable_cross_partition_query=True
                )
            )

            return sessions
        except exceptions.CosmosResourceNotFoundError:
            return None
        except Exception as e:
            logger.error(f"Error in suggest_by_topic: {str(e)}")
            raise

    async def get_sessions_by_time(
        self, date: str = None, time_slot: str = None
    ) -> Dict[str, Any]:
        """Get sessions by date or time slot"""
        try:
            query_conditions = ["SELECT * FROM c WHERE 1=1"]
            parameters = []

            if date:
                query_conditions.append("AND STARTSWITH(c.startTime, @date)")
                parameters.append({"name": "@date", "value": date})

            if time_slot:
                time_ranges = {
                    "morning": ("08:00", "12:00"),
                    "afternoon": ("12:00", "17:00"),
                    "evening": ("17:00", "22:00"),
                }

                if time_slot.lower() in time_ranges:
                    start, end = time_ranges[time_slot.lower()]
                    query_conditions.append(
                        "AND c.startTime >= @start AND c.startTime < @end"
                    )
                    parameters.append({"name": "@start", "value": start})
                    parameters.append({"name": "@end", "value": end})

                query_conditions.append("ORDER BY c.startTime")
                query = " ".join(query_conditions)

                sessions = list(
                    self.session_container.query_items(
                        query=query, parameters=parameters, enable_cross_partition_query=True
                    )
                )

                return sessions
        except exceptions.CosmosResourceNotFoundError:
            return None
        except Exception as e:
            logger.error(f"Error in get_sessions_by_time: {str(e)}")
            raise

    async def suggest_session_by_speaker(self, speaker_id: str) -> List[Dict[str, Any]]:
        """Get sessions by a specific speaker"""
        try:
            query = """
                SELECT * FROM c
                WHERE ARRAY_CONTAINS(c.speakerIds, @speaker_id)
                ORDER BY c.startTime
            """
            parameters = [{"name": "@speaker_id", "value": speaker_id}]

            sessions = list(
                self.session_container.query_items(
                    query=query, parameters=parameters, enable_cross_partition_query=True
                )
            )

            return sessions
        except exceptions.CosmosResourceNotFoundError:
            return None
        except Exception as e:
            logger.error(f"Error in suggest_by_topic: {str(e)}")
            raise

    async def get_all_sessions(self, max_items: int = 5) -> List[Dict[str, Any]]:
        """Get all sessions."""
        query = "SELECT * FROM c"
        items = list(
            self.session_container.query_items(
                query=query, enable_cross_partition_query=True, max_item_count=max_items
            )
        )

        return items

    async def get_chats_from_session(self, session_id: str) -> List[ChatMessage]:
        """
        Load conversation history from Cosmos Db
        """

        query = "SELECT * FROM c WHERE c.sessionId=@session_id ORDER BY c.timestamp ASC"
        parameters = [{"name": "@session_id", "value": session_id}]

        items = list(
            self.chat_container.query_items(
                query=query, parameters=parameters, enable_cross_partition_query=True
            )
        )
        return items


_db_client: Optional[CosmosDbClient] = None


def get_db_client() -> CosmosDbClient:
    """Get or create Cosmos DB client singleton."""
    global _db_client
    if _db_client is None:
        _db_client = CosmosDbClient()
    return _db_client
