'use client';

import { FilterBarContent } from '@/components/filter-bar-content';
import { useGetSessions } from '@/hooks/sessions';
import { Session } from '@/types';
import dynamic from 'next/dynamic';
import { useRouter, useSearchParams } from 'next/navigation';
import { useEffect, useState } from 'react';
import SessionList from './session-list';

function SessionsContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [sessions, setSessions] = useState<Session[]>([]);
  const [selectedSession, setSelectedSession] = useState<Session | null>(null);
  const [filterText, setFilterText] = useState<string>('');

  const sessionId = searchParams?.get('session');
  const { data: publicData, error } = useGetSessions();

  const filteredSessions: Session[] = Array.isArray(sessions)
    ? sessions.filter((session) =>
        session.title?.toLowerCase().includes(filterText.toLowerCase())
      )
    : [];

  const filterChangeHandler = (typedText: string) => {
    setFilterText(typedText);
  };

  const handleSessionClick = (session: Session): void => {
    setSelectedSession(session);
    router.push(`/sessions?session=${session.id}`, { scroll: false });
  };

  const handleCloseModal = () => {
    setSelectedSession(null);
    router.push('/sessions', { scroll: false });
  };

  useEffect(() => {
    if (sessionId && sessions.length > 0) {
      const session = sessions.find((s) => s.id === sessionId);
      if (session) {
        setSelectedSession(session);
      }
    } else {
      setSelectedSession(null);
    }
  }, [sessionId, sessions]);

  useEffect(() => {
    if (publicData) {
      setSessions(publicData);
    }
  }, [publicData]);

  return (
    <>
      <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
        <FilterBarContent
          placeHolder={'Filter by sessions...'}
          className="pt-24"
          handleFilterChange={filterChangeHandler}
          filterText={filterText}
        />

        <SessionList
          sessions={filteredSessions}
          selectedSession={selectedSession}
          error={error}
          handleCloseModal={handleCloseModal}
          handleSessionClick={handleSessionClick}
          showButtons={true}
        />
      </div>
    </>
  );
}

const DynamicSessionContent = dynamic(() => Promise.resolve(SessionsContent), {
  ssr: false,
  loading: () => (
    <div className="flex justify-center items-center min-h-screen">
      Loading sessions...
    </div>
  )
});

export default function SessionsPage() {
  return <DynamicSessionContent />;
}
