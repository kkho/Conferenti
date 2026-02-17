import logging
from datetime import datetime, timezone
import os
from typing import List, Optional
import uuid
from conferenti_agent.types.ai_chat import ChatMessage, ChatRequest, ChatResponse
from conferenti_agent.types.ai_roles import Roles
from pydantic import BaseModel, Field
from fastapi import Depends, FastAPI, HTTPException, Request
from fastapi.responses import JSONResponse
from fastapi.exceptions import RequestValidationError
from conferenti_agent.services.speaker_service import get_speaker_service
from conferenti_agent.auth import verify_token, require_scope

logger = logging.getLogger(__name__)

app = FastAPI(
    title="Conferenti AI Agent Api",
    description="AI-powered agent for conference management tasks.",
    version="0.1.0",
    dependencies=[Depends(verify_token)],
)


@app.exception_handler(RequestValidationError)
async def validation_exception_handler(request: Request, exc: RequestValidationError):
    """Custom handler for validation errors to provide detailed feedback"""
    return JSONResponse(
        status_code=422,
        content={
            "detail": exc.errors(),
            "body": exc.body,
            "message": "Request validation failed. Check the 'detail' field for specific errors.",
        },
    )


class SuggestSpeakersRequest(BaseModel):
    """Request to suggest speakers for a session"""

    session_id: str = Field(..., description="Session ID to suggest speakers for")
    count: int = Field(5, ge=1, le=20, description="Number of speakers to suggest")


class SuggestSpeakersGeneralRequest(BaseModel):
    """Request to suggest speakers without session context"""

    query: str
    topics: list[str] = Field(..., description="List of topics for the conference")
    count: int = Field(5, ge=1, le=20, description="Number of speakers to suggest")


class GenerateBioRequest(BaseModel):
    """Request to generate speaker bio"""

    speaker_id: str = Field(..., description="Speaker ID")


class MatchSpeakerRequest(BaseModel):
    """Request to match speaker to sessions"""

    speaker_id: str = Field(..., description="Speaker ID to match")
    session_ids: List[str] = Field(
        ..., description="List of session IDs to match against"
    )


class SpeakerResponse(BaseModel):
    """Response model for speaker operations"""

    success: bool
    data: Optional[dict] = None
    message: str


class SuggestionResponse(BaseModel):
    """Response for AI suggestions"""

    success: bool
    suggestion: str
    message: str


@app.get("/")
async def root():
    """API root endpoint"""
    return {
        "service": "Conferenti AI Agent API",
        "version": "1.0.0",
        "endpoints": {
            "speakers": "/api/speakers",
            "suggestions": "/api/speakers/suggest",
            "docs": "/docs",
        },
    }


@app.post("/api/speakers/suggest-general", response_model=SuggestionResponse)
async def suggest_speakers_general(
    request: SuggestSpeakersGeneralRequest,
    token: dict = Depends(require_scope("ai:chat")),
):
    """
    AI generated general speaker suggestions without session context.
    Useful for initial conference planning.
    """
    try:
        speaker_service = get_speaker_service()
        suggestion = await speaker_service.suggest_speakers_general(
            session_theme=request.theme,
            topics=request.topics,
            count=request.count,
        )

        return SuggestionResponse(
            success=True,
            suggestion=suggestion,
            message="General speaker suggestions generated successfully",
        )

    except Exception as e:
        import traceback

        print(f"Error in suggest_speakers_general: {str(e)}")
        print(traceback.format_exc())
        raise HTTPException(status_code=500, detail=str(e))


@app.post("/api/speakers/{speaker_id}/generate-bio", response_model=SuggestionResponse)
async def generate_speaker_bio(speaker_id: str, request: MatchSpeakerRequest):
    """Generate AI-powered biography for a speaker"""
    try:
        speaker_service = get_speaker_service()
        matches = await speaker_service.match_speaker_to_sessions(
            speaker_id=speaker_id, available_sessions=request.session_ids
        )

        return SuggestionResponse(
            success=True,
            suggestion=matches,
            message="Speaker-to-session matches generated successfully",
        )
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@app.post("/api/ai/chat", response_model=ChatResponse)
async def handle_chat(request: ChatRequest):
    """
    Main chat endpoint - receives message from .NET Conferenti Api
    determines intent and returns AI response.
    """
    try:
        intent, topics = detect_intent(request.message)

        conversation_history = await load_messages_from_cosmos(
            session_id=request.sessionId
        )
        context = build_context(conversation_history)

        if intent == "speaker_search":
            response_text = await handle_speaker_query(request.message, context, topics)
        elif intent == "session_search":
            response_text = await handle_session_query(request.message, context)
        else:
            response_text = await handle_general_query(request.message, context)

        store_message(
            session_id=request.sessionId, role=Roles.USER.value, content=request.message
        )

        store_message(
            session_id=request.sessionId,
            role=Roles.ASSISTANT.value,
            content=response_text,
        )

        return ChatResponse(
            response=response_text,
            sessionId=request.sessionId,
            success=True,
            error=None,
            timestamp=datetime.now(timezone.utc),
            intent=intent,
            topics=topics,
        )

    except Exception as e:
        logger.error(f"Chat error: {str(e)}")
        raise HTTPException(status_code=500, detail="Failed to process message")


@app.get("/health", dependencies=[])
async def health_check():
    """Health check endpoint"""
    return {
        "status": "healthy",
        "service": "conferenti-ai-agent",
        "ai_agent": (
            "ollama"
            if "localhost" in os.getenv("PROJECT_ENDPOINT", "")
            else "azure-openai"
        ),
    }


def detect_intent(message: str) -> str:
    """
    Analyze message to determine user intent
    """
    message_lower = message.lower()
    topics = []

    tech_keywords = {
        "ai",
        "artificial intelligence",
        "machine learning",
        "ml",
        "deep learning",
        "python",
        "java",
        "golang",
        "javascript",
        "typescript",
        "c#",
        "rust",
        "azure",
        "aws",
        "gcp",
        "cloud",
        "kubernetes",
        "k8s",
        "docker",
        "react",
        "angular",
        "vue",
        "frontend",
        "backend",
        "data science",
        "analytics",
        "big data",
        "sql",
        "nosql",
        "devops",
        "ci/cd",
        "microservices",
        "api",
        "rest",
        "graphql",
        "security",
        "cybersecurity",
        "encryption",
        "authentication",
    }

    for tech in tech_keywords:
        if tech in message_lower:
            topics.append(tech)

    import re

    quoted = re.findall(r'"([^"]+)"', message)
    topics.extend(quoted)

    words = message.split()
    capitalized = [
        w.strip(".,!?") for w in words if w and w[0].isupper() and len(w) > 1
    ]
    common_words = {
        "I",
        "The",
        "A",
        "An",
        "In",
        "On",
        "At",
        "To",
        "For",
        "With",
        "Who",
        "Show",
        "Find",
    }
    proper_nouns = [w for w in capitalized if w not in common_words]
    topics.extend(proper_nouns)

    speaker_keywords = [
        "speaker",
        "presenter",
        "who is",
        "tell me about",
        "biography",
        "expert",
    ]
    if any(keyword in message_lower for keyword in speaker_keywords):
        return ("speaker_search", topics)

    session_keywords = [
        "session",
        "talk",
        "presentation",
        "workshop",
        "schedule",
        "agenda",
        "when",
        "time",
        "attending",
    ]
    if any(keyword in message_lower for keyword in session_keywords):
        return ("session_search", topics)

    return ("general", topics)


def build_context(history: List[ChatMessage]) -> str:
    """
    Build conversation context from history
    """

    if not history:
        return ""

    recent_messages = history[-5:] if len(history) > 5 else history

    context_parts = []
    for msg in recent_messages:
        role = Roles.USER if msg.role == Roles.USER else Roles.ASSISTANT
        context_parts.append(f"{role}: {msg.content}")

    return "\n".join(context_parts)


async def handle_speaker_query(message: str, context: str, topics: List[str]) -> str:
    """
    Handle speaker related queries using existing agent
    """
    from conferenti_agent.services.speaker_service import SpeakerService

    service = SpeakerService()
    prompt = f"""Previous conversation:
    {context}
    
    Current question: {message}
    
    Provide information about the requested speakers(s)."""

    result = await service.suggest_speakers_general(prompt, topics=topics)
    return result or "I couldn't find information about that speaker."


async def handle_session_query(message: str, context: str) -> str:
    """
    Handle session-related queries using existing agent
    """
    from conferenti_agent.services.session_service import SessionService

    service = SessionService()
    prompt = f"""Previous conversation: {context}
    
    Current question: {message}
    
    Provide information about the requested session(s).
    """

    result = await service.suggest_general(prompt)
    return result or "I couldn't find information about that sessions."


async def handle_general_query(message: str, context: str) -> str:
    """
    Handle general conference queries
    """
    from conferenti_agent.agent import AiAgent

    agent = AiAgent()
    prompt = f"""You are a helpful conference assistant for Conferenti
    Previous conversation: {context}
    
    User question: {message}
    
    Provide a helpful, concise response about the conference."""

    response = agent.run(prompt)
    return response


def store_message(session_id: str, role: str, content: str):
    """
    Store message in Cosmos Db with TTL
    """
    from conferenti_agent.services.database import get_db_client

    client = get_db_client()
    container = client.chat_container

    message = {
        "id": str(uuid.uuid4()),
        "sessionId": session_id,
        "role": str(role),
        "content": content,
        "timestamp": datetime.utcnow().isoformat(),
        "ttl": 259200,  # 3 days in seconds
    }

    container.upsert_item(message)


async def load_messages_from_cosmos(session_id: str) -> List[ChatMessage]:
    """
    Load conversation history from Cosmos Db
    """
    from conferenti_agent.services.database import get_db_client

    client = get_db_client()

    items = await client.get_chats_from_session(session_id=session_id)

    messages = []
    for item in items:
        messages.append(
            ChatMessage(
                role=item["role"],
                content=item["content"],
                timestamp=datetime.fromisoformat(item["timestamp"]),
            )
        )

    return messages


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(
        "conferenti_agent.services.api_client:app",
        host="0.0.0.0",
        port=8000,
        reload=True,
    )
