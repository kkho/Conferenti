import { ChatMessage, Role } from '@/types';
import { useCallback, useEffect, useState } from 'react';

export function useAiChat() {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [sessionId, setSessionId] = useState<string | null>(null);

  const threeDaysInSeconds = 259300;

  useEffect(() => {
    const initSession = async () => {
      try {
        const response = await fetch('/api/ai-chat/session', {
          credentials: 'include' // Important: send/receive cookies
        });
        const data = await response.json();
        setSessionId(data.sessionId);

        // Optional: Load chat history if session exists
        // if (data.sessionId) {
        //   await loadHistory(data.sessionId);
        // }
      } catch (err) {
        setError('failed to initialize chat session');
        console.error('Session init error:', err);
      }
    };

    initSession();
  }, []);

  const sendMessage = useCallback(
    async (userId: string, content: string) => {
      if (!content.trim() || isLoading) {
        return;
      }

      const localDate: Date = new Date();

      const userMessage: ChatMessage = {
        id: crypto.randomUUID(),
        sessionId: sessionId || '',
        userId,
        role: Role.User,
        content,
        timestamp: new Date(
          Date.UTC(
            localDate.getUTCFullYear(),
            localDate.getUTCMonth(),
            localDate.getUTCDate(),
            localDate.getUTCHours(),
            localDate.getUTCMinutes(),
            localDate.getUTCSeconds(),
            localDate.getUTCMilliseconds()
          )
        ),
        ttl: threeDaysInSeconds
      };

      setMessages((prev) => [...prev, userMessage]);
      setIsLoading(true);
      setError(null);

      try {
        // Backend reads sessionId from HTTP-Only cookie
        // If cookie doesn't exist, backend will create a new session
        const response = await fetch('/api/ai-chat/session', {
          method: 'POST',
          credentials: 'include',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ message: content })
        });

        if (!response.ok) {
          throw new Error('Failed to send message');
        }

        const data = await response.json();

        if (!sessionId && data.sessionId) {
          setSessionId(data.sessionId);
        }

        const assistantMessage: ChatMessage = {
          id: crypto.randomUUID(),
          sessionId: sessionId || data.sessionId || '',
          userId: 'Conferenti Bot',
          role: Role.Assistant,
          content: data.response,
          timestamp: new Date(data.timestamp),
          ttl: threeDaysInSeconds
        };

        setMessages((prev) => [...prev, assistantMessage]);
      } catch (err) {
        setError('Failed to send message. Please try again');
      } finally {
        setIsLoading(false); // Fixed: was true, should be false
      }
    },
    [isLoading, sessionId]
  );

  const startNewChat = useCallback(async () => {
    try {
      // Delete old session cookie
      await fetch('/api/ai-chat/session', {
        method: 'DELETE',
        credentials: 'include'
      });

      // Create new session with new cookie
      const response = await fetch('/api/ai-chat/session', {
        credentials: 'include'
      });
      const data = await response.json();
      setSessionId(data.sessionId);
      setMessages([]);
      setError(null);
    } catch (err) {
      setError('Failed to start a new chat');
      console.error('Start new chat error:', err);
    }
  }, []);

  return {
    messages,
    isLoading,
    error,
    sessionId,
    sendMessage,
    startNewChat
  };
}
