

export interface MenuItemData {
  title?: string;
  slug?: string;
  _type?: string;
  current?: boolean
}

export interface Speaker {
  name?: string;
  position?: string;
  company?: string;
  bio?: string;
  photoUrl?: string;
  speakerSessions?: SpeakerSession[];
}

export interface SpeakerSession {
  sessionId?: string;
  title?: string;
  slug?: string;
  tags: string[];
  description?: string;
  startTime: Date;
  endTime: Date;
  room?: string;
  level?: SessionLevel;
  format?: SessionFormat;
  language: string;
}

export interface SpeakerSessionDetails {
  speakerSession: SpeakerSession;
  speakers: Speaker[];
}

export enum SessionLevel {
  Beginner = "Beginner",
  Intermediate = "Intermediate",
  Advanced = "Advanced",
}

export enum SessionFormat {
  Lecture = "Lecture",
  Workshop = "Workshop",
  Panel = "Panel",
  Keynote = "Keynote",
}