import os
from typing import List, Optional
from dotenv import find_dotenv, load_dotenv
from pydantic import BaseModel, Field
from fastapi import Depends, FastAPI, HTTPException, Request
from fastapi.responses import JSONResponse
from fastapi.exceptions import RequestValidationError
from conferenti_agent.services.speaker_service import get_speaker_service
from conferenti_agent.auth import verify_token, require_scope

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

    theme: str = Field(..., description="Theme of the conference")
    topics: list[str] = Field(..., description="List of topics for the conference")
    target_audience: str = Field(
        "Professionals", description="Target audience for the speakers"
    )
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


@app.post("/api/speakers/suggest", response_model=SuggestionResponse)
async def suggest_speakers_for_session(request: SuggestSpeakersRequest):
    """
    AI generates speaker suggestions for a specific session.
    Returns conversational text suitable for chatbot UI
    """
    try:
        speaker_service = get_speaker_service()
        suggestion = await speaker_service.suggest_speakers_for_session(
            request.session_id, request.count
        )

        return SuggestionResponse(
            success=True,
            suggestion=suggestion,
            message="Speaker suggestions generated successfully",
        )

    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


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
            target_audience=request.target_audience,
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


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(
        "conferenti_agent.services.api_client:app",
        host="0.0.0.0",
        port=8000,
        reload=True,
    )
