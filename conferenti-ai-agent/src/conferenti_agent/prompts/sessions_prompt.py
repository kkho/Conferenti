SUGGEST_SESSIONS_PROMPT = """
You are curating sessions.

Input Variables:
- {session_name}
- {session_duration}
- {target_audience}
- {industry}
- {focus_areas}
- {audience_level}
- {number_of_sessions}
- {session_duration}

Task: Suggest diverse session topics

Output Format:
For each session:
  - Title (compelling & clear)
  - Type (keynote/workshop/panel/etcv)
  - Duration
  - Description (3-4 sentences)
  - Target Level
  - Learning Objectives (3-5)
  - Suggested Speaker Profile
  - Track/Theme
  
Guidelines:
- Diverse topics & formats
- Mix strategic + practical
- Current trends
- Different learning styles
- Actionable content
"""

SUGGEST_SESSION_AGENDA_PROMPT = """
You are creating session agendas.

Input Variables:
- {session_title}
- {session_type}
- {duration}
- {speaker_name}
- {session_topic}
- {audience_level}
- {learning_objectives}

Task: Create minute-by-minute agenda

Output Format:
For each time block:
  - Time (minutes)
  - Activity/Topic
  - Format (presentation/demo/Q&A/execise)
  - Key Points
  - Materials Needed
  
Structure:
- Opening (5-10%)
- Main Content (70-80%)
- Interactive Elements (10-15%)
- Closing (5-10%)

Guidelines:
- Segments 5-15 min
- Interaction every 15-20 min
- Simple to complex
- Buffer for questions
- Vary pace & format
"""

MATCH_SESSIONS_TO_TRACK_PROMPT = """
You are organizing sessions into tracks.

Input Variables: 
- {session_tracks}
- {session_list}
- {session_name}
- {session_description}
- {target_audience}

Task: Assign sessions to tracks + create flow

Output Format:
For each track:
  - Track Name
  - Description (2-3 sentences)
  - Assigned Sessions (in order)
  - Sequence Rationale
  - Estimated Audience Size
  - Key Themes
  
For each assignment:
  - Why this track
  - Connection to other sessions
  - Value for attendees
  - Suggested time slot
  
Guidelines:
- Balance track sizes
- Create narrative flow
- Avoid conflicts
- Consier audience journey
- Start foundational, build complexity
"""
