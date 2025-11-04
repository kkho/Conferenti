import { render, screen } from '@testing-library/react';
import SessionContent from '.';
import { Session, SessionFormat, SessionLevel } from '@/types';

const mockSession: Session = {
  id: '1',
  title: 'Test Session',
  slug: 'test-session',
  tags: ['test', 'demo'],
  description: 'This is a test session description',
  startTime: new Date('2025-11-03T10:00:00'),
  endTime: new Date('2025-11-03T11:00:00'),
  room: 'Room A',
  level: SessionLevel.Beginner,
  format: SessionFormat.Workshop,
  language: 'English',
  speakerIds: ['speaker-1']
};

describe('SessionContent', () => {
  const mockHandleSessionClick = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('renders without crashing', () => {
    render(
      <SessionContent
        session={mockSession}
        handleSessionClick={mockHandleSessionClick}
        imageSource="/test-image.png"
      />
    );

    // Verify the card renders with the session title
    expect(screen.getByText('Test Session')).toBeInTheDocument();
  });

  it('renders with proper image source', () => {
    const { container } = render(
      <SessionContent
        session={mockSession}
        handleSessionClick={mockHandleSessionClick}
        imageSource="/workshop.png"
      />
    );

    const image = container.querySelector('img');
    expect(image).toBeInTheDocument();
    expect(image).toHaveAttribute('src', '/workshop.png');
  });

  it('renders session description', () => {
    render(
      <SessionContent
        session={mockSession}
        handleSessionClick={mockHandleSessionClick}
        imageSource="/test-image.png"
      />
    );

    expect(
      screen.getByText('This is a test session description')
    ).toBeInTheDocument();
  });
});
