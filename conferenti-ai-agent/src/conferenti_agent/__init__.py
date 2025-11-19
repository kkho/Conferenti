"""Conferenti AI Agent for conference management."""

from .agent import create_agent_client
from .config import Settings, get_settings

__version__ = "0.1.0"
__all__ = ["create_agent_client", "Settings", "get_settings"]
