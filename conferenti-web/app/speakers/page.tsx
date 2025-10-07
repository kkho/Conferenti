'use client';

import { Content } from '@/components/content-module';
import { FilterBarContent } from '@/components/filter-bar-content';
import { Speaker } from '@/types';
import { useEffect, useState, Suspense } from 'react';
import Image from 'next/image';
import { useRouter, useSearchParams } from 'next/navigation';
import SpeakerModal from '@/app/speakers/speaker-modal/speaker-modal';

function SpeakersContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [speakers, setSpeakers] = useState<Speaker[]>([]);
  const [selectedSpeaker, setSelectedSpeaker] = useState<Speaker | null>(null);
  const [filterText, setFilterText] = useState<string>('');
  const [isError, setIsError] = useState<boolean>(false);

  // Check URL for speaker modal
  const speakerId = searchParams?.get('speaker');

  const filteredSpeakers = speakers.filter((speaker) =>
    speaker.name?.toLowerCase().includes(filterText.toLowerCase())
  );

  const filterChangeHandler = (typedText: string) => {
    setFilterText(typedText);
  };

  // Handle speaker click - open modal and update URL
  const handleSpeakerClick = (speaker: Speaker) => {
    setSelectedSpeaker(speaker);
    router.push(`/speakers?speaker=${speaker.id}`, { scroll: false });
  };

  // Handle modal close - clear URL param
  const handleCloseModal = () => {
    setSelectedSpeaker(null);
    router.push('/speakers', { scroll: false });
  };

  // Effect to handle URL changes and find speaker
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
    setIsError(false);
    fetch('data/speakers.json')
      .then((response) => response.json())
      .then((fetchedData) => setSpeakers(fetchedData))
      .catch((error) => {
        setIsError(true);
        console.error('Error fetching data:', error);
      });
  }, []);

  // TODO: Will use the uncommented code below to fetch from API when ready
  // const { data: apiSpeakers, isLoading, error } = useGetSpeakers(''); // Remove filter from API call
  // useEffect(() => {
  //   if (apiSpeakers) {
  //     setSpeakers(apiSpeakers);
  //   }
  // }, [apiSpeakers]);

  return (
    <>
      <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
        <Content
          title="Speakers"
          description="Meet our speakers for this years conference."
          imageUrl="https://storage.googleapis.com/cloudnext-assets/event-assets/25/speakers/hero/background.2025-01-21.webp"
        />
        <FilterBarContent
          placeHolder={'Filter by speakers...'}
          className="pt-24"
          handleFilterChange={filterChangeHandler}
          filterText={filterText}
        />

        <ul className="grid gap-x-8 gap-y-12 sm:grid-cols-2 sm:gap-y-16 xl:col-span-2 mt-12">
          {filteredSpeakers &&
            filteredSpeakers?.map((speaker) => (
              <li key={speaker.id || speaker.name}>
                <div
                  className="flex item-center gap-x-6 cursor-pointer p-4 rounded-lg hover:bg-gray-800/50 transition-colors"
                  onClick={() => handleSpeakerClick(speaker)}
                >
                  <Image
                    className="size-16 rounded-full outline-1 -outline-offset-1 outline-white/10"
                    src={speaker.photoUrl || '/empty-user.png'}
                    alt={speaker.name || 'Speaker'}
                    width={64}
                    height={64}
                  />
                  <div>
                    <h3 className="text-base/7 font-semibold tracking-tight text-white">
                      {speaker.name}
                    </h3>
                    <p className="text-sm/6 font-semibold text-gray-400">
                      {speaker.position}
                    </p>
                    {speaker.company && (
                      <p className="text-sm/6 text-gray-500">
                        {speaker.company}
                      </p>
                    )}
                  </div>
                </div>
              </li>
            ))}
        </ul>

        {isError && (
          <p className="mt-6 text-lg/8 text-gray-400" aria-live="polite">
            Error retrieving speakers
          </p>
        )}

        {selectedSpeaker && (
          <SpeakerModal
            speaker={selectedSpeaker}
            isOpen={!!selectedSpeaker}
            onClose={handleCloseModal}
          />
        )}
      </div>
    </>
  );
}

export default function SpeakersPage() {
  return (
    <Suspense
      fallback={
        <div className="flex justify-center items-center min-h-screen">
          Loading speakers...
        </div>
      }
    >
      <SpeakersContent />
    </Suspense>
  );
}
