import React from 'react';
import { render, screen } from '@testing-library/react';
import { Speaker } from '@/types';
import SpeakerList from '.';

describe('SpeakerList', () => {
  it('renders without crashing', () => {
    render(
      <SpeakerList
        speakers={[]}
        selectedSpeaker={null}
        error={null}
        handleCloseModal={function (): void {
          throw new Error('Function not implemented.');
        }}
        handleSpeakerClick={function (speaker: Speaker): void {
          throw new Error('Function not implemented.');
        }}
      />
    );

    expect(true).toBe(true);
  });

  it('renders navigation links with Speakers', () => {
    render(
      <SpeakerList
        speakers={[
          {
            id: '1',
            name: 'John Doe',
            position: 'Developer'
          } as Speaker
        ]}
        selectedSpeaker={null}
        error={null}
        handleCloseModal={function (): void {
          throw new Error('Function not implemented.');
        }}
        handleSpeakerClick={function (speaker: Speaker): void {
          throw new Error('Function not implemented.');
        }}
      />
    );

    // Check if menu items are rendered
    expect(screen.getByText('John Doe')).toBeInTheDocument();
    expect(screen.getByText('Developer')).toBeInTheDocument();
  });

  it('renders without menuItems', () => {
    render(
      <SpeakerList
        speakers={[]}
        selectedSpeaker={null}
        error={null}
        handleCloseModal={function (): void {
          throw new Error('Function not implemented.');
        }}
        handleSpeakerClick={function (speaker: Speaker): void {
          throw new Error('Function not implemented.');
        }}
      />
    );

    const speakerList = screen.getByRole('list');
    expect(speakerList).toBeInTheDocument();
  });
});
