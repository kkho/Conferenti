import React from 'react';
import { render, screen } from '@testing-library/react';
import Navbar from '.';
import { MenuItemData } from '@/types';

// Mock the route resolver
jest.mock('../../lib/route.links', () => ({
  resolveHref: (type: string, _slug: string) => {
    if (type === 'sessions') return '/sessions';
    if (type === 'speakers') return '/speakers';
    if (type === 'faq') return '/faq';
    return '#';
  },
}));

describe('NavBar', () => {
  const menuItems: MenuItemData[] = [
    { title: 'Sessions', slug: '', _type: 'sessions', current: false },
    { title: 'Speakers', slug: '', _type: 'speakers', current: false },
    { title: 'FAQ', slug: '', _type: 'faq', current: false }
  ];

  it('renders without crashing', () => {
    render(<Navbar menuItems={[]} />);
    // Basic smoke test
    expect(true).toBe(true);
  });

  it('renders navigation links with menuItems', () => {
    render(<Navbar menuItems={menuItems} />);
    
    // Check if menu items are rendered
    expect(screen.getByText('Sessions')).toBeInTheDocument();
    expect(screen.getByText('Speakers')).toBeInTheDocument();
    expect(screen.getByText('FAQ')).toBeInTheDocument();
  });

  it('renders without menuItems', () => {
    render(<Navbar menuItems={[]} />);
    
    // Should still render the navbar structure
    const navbar = screen.getByRole('navigation');
    expect(navbar).toBeInTheDocument();
  });
});
