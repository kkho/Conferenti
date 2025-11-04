'use client';

import { Content } from '@/components/content-module';
import { FilterBarContent } from '@/components/filter-bar-content';
import { Speaker } from '@/types';
import { useEffect, useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import dynamic from 'next/dynamic';
// TODO: Uncomment when Auth0 is configured
// import { useUser } from '@auth0/nextjs-auth0/client';
import { useGetSpeakers } from '@/hooks/speakers';
import SpeakerList from '@/app/speakers/speaker-list';

function SpeakersContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [speakers, setSpeakers] = useState<Speaker[]>([]);
  const [selectedSpeaker, setSelectedSpeaker] = useState<Speaker | null>(null);
  const [filterText, setFilterText] = useState<string>('');

  const speakerId = searchParams?.get('speaker');

  const { data: publicData, error } = useGetSpeakers();

  const filteredSpeakers: Speaker[] = Array.isArray(speakers)
    ? speakers.filter((speaker) =>
        speaker.name?.toLowerCase().includes(filterText.toLowerCase())
      )
    : [];

  const filterChangeHandler = (typedText: string) => {
    setFilterText(typedText);
  };

  const handleSpeakerClick = (speaker: Speaker): void => {
    setSelectedSpeaker(speaker);
    router.push(`/speakers?speaker=${speaker.id}`, { scroll: false });
  };

  const handleCloseModal = () => {
    setSelectedSpeaker(null);
    router.push('/speakers', { scroll: false });
  };

  useEffect(() => {
    if (speakerId && speakers.length > 0) {
      const speaker = speakers.find((s) => s.id === speakerId);
      if (speaker) {
        setSelectedSpeaker(speaker);
      }
    } else {
      setSelectedSpeaker(null);
    }
  }, [speakerId, speakers]);

  useEffect(() => {
    if (publicData) {
      setSpeakers(publicData);
    }
  }, [publicData]);

  return (
    <>
      <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
        <Content
          classes="justify-between flex-row"
          extraClasses="mt-24"
          textClasses="text-white"
          title="Speakers"
          description="Meet our speakers for this years conference."
          imageUrl="Peer_Hexagon_Lys.svg"
        />
        <FilterBarContent
          placeHolder={'Filter by speakers...'}
          className="pt-24"
          handleFilterChange={filterChangeHandler}
          filterText={filterText}
        />

        <SpeakerList
          speakers={filteredSpeakers ?? []}
          selectedSpeaker={selectedSpeaker}
          error={error}
          handleCloseModal={handleCloseModal}
          handleSpeakerClick={handleSpeakerClick}
        />
      </div>
    </>
  );
}

const DynamicSpeakersContent = dynamic(() => Promise.resolve(SpeakersContent), {
  ssr: false,
  loading: () => (
    <div className="flex justify-center items-center min-h-screen">
      Loading speakers...
    </div>
  )
});

export default function SpeakersPage() {
  return <DynamicSpeakersContent />;
}
