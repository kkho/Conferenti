from dataclasses import dataclass
from datetime import datetime
from enum import Enum
from typing import List


class SessionLevel(str, Enum):
    BEGINNER = "Beginner"
    INTERMEDIATE = "Intermediate"
    ADVANCED = "Advanced"


class SessionFormat(str, Enum):
    LECTURE = "Lecture"
    WORKSHOP = "Workshop"
    PANEL = "Panel"
    KEYNOTE = "Keynote"
    PRESENTATION = "Presentation"


@dataclass
class Session:
    id: str
    title: str
    slug: str
    tags: List[str]
    description: str
    startTime: datetime
    endTime: datetime
    room: str
    level: SessionLevel
    format: SessionFormat
    language: str
    speakerIds: List[str]

    def to_cosmos_dict(self) -> dict:
        return {
            "id": self.id,
            "title": self.title,
            "slug": self.slug,
            "tags": self.tags,
            "description": self.description,
            "startTime": self.startTime.isoformat(),
            "endTime": self.endTime.isoformat(),
            "room": self.room,
            "level": self.level.value,
            "format": self.format.value,
            "language": self.language,
            "speakerIds": self.speakerIds,
        }

    @classmethod
    def from_cosmos_dict(cls, data: dict) -> "Session":
        return cls(
            id=data["id"],
            title=data["title"],
            slug=data["slug"],
            tags=data["tags"],
            description=data["description"],
            startTime=datetime.fromisoformat(data["startTime"]),
            endTime=datetime.fromisoformat(data["endTime"]),
            room=data["room"],
            level=SessionLevel(data["level"]),
            format=SessionFormat(data["format"]),
            language=data["language"],
            speakerIds=data["speakerIds"],
        )


@dataclass
class SessionCreateRequest:
    """Request model for creating sessions via AI"""

    id: str
    session_name: str
    target_audience: str
    industry: str
    focus_areas: List[str]
    audience_level: SessionLevel
    number_of_sessions: int = 10
    session_duration: int = 45  # minutes
    language: str = "English"
