from datetime import datetime, timezone
from typing import List, Optional
from pydantic import BaseModel, Field


class ChatMessage(BaseModel):
    role: str = Field(..., pattern="^(USER|ASSISTANT)$")
    content: str
    timestamp: datetime = Field(default_factory=lambda: datetime.now(timezone.utc))


class ChatRequest(BaseModel):
    message: str = Field(..., min_length=1, max_length=2000)
    sessionId: str


class ChatResponse(BaseModel):
    response: str
    sessionId: str
    timestamp: datetime
    success: bool = False
    error: Optional[str] = None
    intent: Optional[str] = None
    topics: Optional[List[str]] = []


class ChatHistoryResponse(BaseModel):
    sessionId: str
    messages: List[ChatMessage]
    createdAt: datetime
    lastMessageAt: datetime
