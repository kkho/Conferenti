'use client';

import dynamic from 'next/dynamic';
import { Content } from '@/components/content-module';
import SpeakerList from '@/app/speakers/speaker-list';
import { useGetSpeakers } from '@/hooks/speakers';
import { Session, Speaker } from '@/types';
import { useState, useEffect } from 'react';
import { useGetSessions } from '@/hooks/sessions';
import SessionList from './sessions/session-list';

function HomeContent() {
  const [speakers, setSpeakers] = useState<Speaker[]>([]);
  const [selectedSpeaker, setSelectedSpeaker] = useState<Speaker | null>(null);
  const [selectedSession, setSelectedSession] = useState<Session | null>(null);
  const { data: speakerData, error: errorSpeakers } = useGetSpeakers();
  const { data: sessionData, error: errorSessions } = useGetSessions();

  const handleSpeakerClick = (speaker: Speaker): void => {
    setSelectedSpeaker(speaker);
  };

  const handleSpeakerCloseModal = (): void => {
    setSelectedSpeaker(null);
  };

  const handleSessionClick = (session: Session): void => {
    setSelectedSession(session);
  };

  const handleSessionCloseModal = (): void => {
    setSelectedSession(null);
  };

  useEffect(() => {
    if (speakerData) {
      setSpeakers(speakerData);
    }
  }, [speakerData]);

  return (
    <div className="w-full font-sans">
      <main
        id="main-content"
        className="flex flex-col gap-[32px] row-start-2 items-center sm:items-start"
      >
        <div className="banner"></div>
        <section className="w-full section pr-12 pl-12 item-center bg-[#f8f9fa]">
          <h1 className="text-3xl pt-8 font-semibold tracking-tight text-pretty sm:text-4xl text-center text-black">
            Welcome to Conferenti 2025
          </h1>
          <Content
            classes="justify-center flex-row-reverse"
            extraClasses="mt-12"
            textClasses="text-gray-600"
            title="Explore conferenti"
            description="Check out the Conferenti highlights and get knowledge about the conference speakers, schedule, and more.
          Join us to make the event successful!"
            imageUrl="/conferenti_explore.png"
            imageHeight={400}
            imageWidth={350}
            imageClasses="mr-24"
          />

          <h2 className="text-3xl pt-8 font-semibold tracking-tight text-pretty sm:text-4xl text-center text-black">
            Why Conferenti?
          </h2>
          <Content
            classes="justify-center flex-row"
            extraClasses="mt-12"
            textClasses="text-gray-600"
            title="Meet the speakers"
            description="Connect with professionals and industry leaders who will be sharing their insights and expertise at the conference."
            imageUrl="/speakers.png"
            imageHeight={400}
            imageWidth={350}
            imageClasses="ml-24"
          ></Content>
          <Content
            classes="justify-center flex-row-reverse"
            extraClasses="mt-12"
            textClasses="text-gray-600"
            title="Be part of innovation"
            description="Join us in embracing innovation and exploring the latest trends and technologies that are shaping the future of our industry."
            imageUrl="/conferenti_conference.png"
            imageHeight={400}
            imageWidth={350}
            imageClasses="mr-24"
          ></Content>

          <>
            <h2 className="text-3xl pt-8 font-semibold tracking-tight text-pretty sm:text-4xl text-center text-black">
              Meet the Speakers
            </h2>
            <SpeakerList
              classes="pr-80 pl-80"
              speakers={speakers ?? []}
              selectedSpeaker={selectedSpeaker}
              error={errorSpeakers}
              handleCloseModal={handleSpeakerCloseModal}
              handleSpeakerClick={handleSpeakerClick}
            />
          </>
          <>
            <h2 className="text-3xl pt-8 font-semibold tracking-tight text-pretty sm:text-4xl text-center text-black">
              Agenda
            </h2>
            <div className="pr-80 pl-80">
              <p className="mt-8 mb-8 text-lg/8 text-center text-gray-600">
                Get a glimpse of what is coming soon. Select what you want to
                attend and enjoy the conference!
              </p>

              <SessionList
                sessions={sessionData ?? []}
                selectedSession={selectedSession}
                error={errorSessions}
                handleCloseModal={handleSessionCloseModal}
                handleSessionClick={handleSessionClick}
                className="cardContent"
                titleClass="text-black"
                descriptionClass="text-gray-600"
                showButtons={true}
              />
            </div>
          </>
        </section>
      </main>
    </div>
  );
}

const DynamicHomeContent = dynamic(() => Promise.resolve(HomeContent), {
  ssr: false,
  loading: () => (
    <div className="flex justify-center items-center min-h-screen">
      Loading page...
    </div>
  )
});

export default function HomePage() {
  return <DynamicHomeContent />;
}
