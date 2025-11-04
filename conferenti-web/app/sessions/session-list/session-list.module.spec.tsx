import { render, screen } from '@testing-library/react';
import { Session } from '@/types';
import SessionList from '.';

describe('SessionList', () => {
  it('renders without crashing', () => {
    render(
      <SessionList
        sessions={[]}
        selectedSession={null}
        error={null}
        handleCloseModal={function (): void {
          throw new Error('Function not implemented.');
        }}
        handleSessionClick={function (session: Session): void {
          throw new Error('Function not implemented.');
        }}
      />
    );

    expect(true).toBe(true);
  });

  it('renders navigation links with Sessions', () => {
    render(
      <SessionList
        sessions={[
          {
            id: '1',
            title: 'Session 1',
            description: 'Description for Session 1'
          } as Session
        ]}
        selectedSession={null}
        error={null}
        handleCloseModal={function (): void {
          throw new Error('Function not implemented.');
        }}
        handleSessionClick={function (session: Session): void {
          throw new Error('Function not implemented.');
        }}
      />
    );

    // Check if menu items are rendered
    expect(screen.getByText('Session 1')).toBeInTheDocument();
    expect(screen.getByText('Description for Session 1')).toBeInTheDocument();
  });

  it('renders without menuItems', () => {
    render(
      <SessionList
        sessions={[]}
        selectedSession={null}
        error={null}
        handleCloseModal={function (): void {
          throw new Error('Function not implemented.');
        }}
        handleSessionClick={function (session: Session): void {
          throw new Error('Function not implemented.');
        }}
      />
    );

    const sessionList = screen.getByRole('list');
    expect(sessionList).toBeInTheDocument();
  });
});
