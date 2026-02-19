import { ChatMessage, Role } from '@/types';
import { useCallback, useEffect, useState } from 'react';

export function useAiChat() {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const threeDaysInSeconds = 259300;

  useEffect(() => {
    const initSession = async () => {
      try {
        // Initialize session on server - sessionId managed via HTTP-Only cookie
        await fetch('/api/ai-chat/session', {
          credentials: 'include' // Important: send/receive cookies
        });

        // Optional: Load chat history if session exists
        // const data = await response.json();
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
        sessionId: '', // Will be populated with server-provided sessionId
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

        // Update user message with server-provided sessionId
        setMessages((prev) =>
          prev.map((msg) =>
            msg.id === userMessage.id
              ? { ...msg, sessionId: data.sessionId }
              : msg
          )
        );

        const assistantMessage: ChatMessage = {
          id: crypto.randomUUID(),
          sessionId: data.sessionId, // Use server-provided sessionId
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
        setIsLoading(false);
      }
    },
    [isLoading, threeDaysInSeconds]
  );

  const startNewChat = useCallback(async () => {
    try {
      // Delete old session cookie
      await fetch('/api/ai-chat/session', {
        method: 'DELETE',
        credentials: 'include'
      });

      // Create new session with new cookie
      await fetch('/api/ai-chat/session', {
        credentials: 'include'
      });

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
    sendMessage,
    startNewChat
  };
}
