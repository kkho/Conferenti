SUGGEST_SPEAKERS_PROMPT = """
You are a conference organizer AI.

Input Variables:
- {session_theme}
- {target_audience}
- {industry}
- {topics}
- {number_of_speakers}

Task: suggest speakers for this session

Output Format:
For each speaker:
  - Full Name
  - Title & Company
  - Expertise Areas (3-5)
  - Why Good Fit (2-3 sentences)
  - Notable Achievements
  - Suggested Topics (2-3)
  
Guidelines:
- Focus on real expertise
- Consider diversity
- Include thought leaders + emerging voices
"""

MATCH_SPEAKER_TO_SESSIONS_PROMPT = """
You are matching speakers to session topics.

Input Variables:
- {speaker_name}
- {speaker_expertise}
- {speaker_background}
- {previous_topics}
- {session_topics}
- {session_name}

Task: find top 3 session matches

Output Format:
For each match:
  - Session Topic
  - Match Score (1-10)
  - Reasoning
  - Suggested Angle
  - Audience Value
  
Guidelines:
- Consider expertise depth
- Match audience level
- Look for unique perspectives
"""

SUGGEST_SPEAKER_TOPICS_PROMPT = """
You are generating session topics for a speaker.

Input Variables:
- {speaker_name}
- {speaker_expertise}
- {speaker_background}
- {recent_work}
- {session_name}
- {target_audience}
- {session_duration}
- {focus_areas}
- {number_of_topics}

Task: Suggest session topics

Output Format:
For each topic:
  - Session Title (catchy, benefit-driven)
  - Session Type (keynote/workshop/panel/deep-dive)
  - Description (2-3 sentences)
  - Key Takeaways (3-4 bullets)
  - Target Level (beginner/intermediate/advanced)
  - Why this Speaker

Guidelines:
- Action-oriented titles
- Timely & relevant
- Mix practical + strategic
- Fix time duration
"""

GENERATE_SPEAKER_BIO_PROMPT = """
You are writing a professional speaker biography.

Input Variables:
- {speaker_name}
- {position}
- {company}
- {email}
- {current_bio}
- {session_titles}

Task: Write a compelling 150-200 word professional biography

Output Format:
A single paragraph biography in third person

Guidelines:
- Write in third person (use "he/she/they" or speaker's name)
- Lead with most impressive credential or current role
- Highlight 2-3 notable achievements or expertise areas
- Mention current position and company
- Reference their session topics if relevant
- Professional but engaging tone
- Focus on what makes them valuable to attendees
- Keep it concise (150-200 words)
- Avoid jargon unless industry-standard
- End with a note about what attendees will gain

Return only the biography text, no additional commentary.
"""
