from datetime import datetime, timezone
from typing import List, Optional
from pydantic import BaseModel, Field


class ChatMessage(BaseModel):
    role: str = Field(..., pattern="^(user|assisant)$")
    content: str
    timestamp: datetime = Field(default_factory=lambda: datetime.now(timezone.utc))


class ChatRequest(BaseModel):
    message: str = Field(..., min_length=1, max_length=2000)
    sessionId: str
    conversationHistory: Optional[List[ChatMessage]] = []


class ChatResponse(BaseModel):
    response: str
    sessionId: str
    timestamp: datetime
    intent: Optional[str] = None


class ChatHistoryResponse(BaseModel):
    sessionId: str
    messages: List[ChatMessage]
    createdAt: datetime
    lastMessageAt: datetime
