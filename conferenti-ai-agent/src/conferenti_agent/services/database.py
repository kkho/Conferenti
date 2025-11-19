from dataclasses import dataclass
from typing import Any, Dict, List, Optional
from azure.cosmos import CosmosClient, exceptions
from conferenti_agent.config import get_settings

# Suppress SSL warnings for Cosmos DB emulator


@dataclass
class CosmosConfig:
    endpoint: str
    key: str
    database_name: str
    speaker_container_name: str
    session_container_name: str


class CosmosDbClient:
    """Cosmos Db client for Conferenti data operations"""

    def __init__(self):
        settings = get_settings()
        if not settings.cosmos_db_endpoint or not settings.cosmos_db_key:
            raise ValueError("Cosmos DB endpoint and key must be configured in .env")

        connection_kwargs = {}
        if settings.cosmos_db_use_local:
            connection_kwargs["connection_verify"] = False

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


_db_client: Optional[CosmosDbClient] = None


def get_db_client() -> CosmosDbClient:
    """Get or create Cosmos DB client singleton."""
    global _db_client
    if _db_client is None:
        _db_client = CosmosDbClient()
    return _db_client
