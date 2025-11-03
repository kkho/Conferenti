import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import FooterComponent from './index';

describe('FooterComponent', () => {
  it('renders without crashing', () => {
    render(<FooterComponent />);
    expect(screen.getByRole('contentinfo')).toBeInTheDocument();
  });

  it('displays the Conferenti logo', () => {
    render(<FooterComponent />);
    const logo = screen.getByAltText('Conferenti Logo');
    expect(logo).toBeInTheDocument();
    expect(logo).toHaveAttribute('src', '/conferenti_footer_logo.png');
  });

  it('displays copyright text with current year', () => {
    render(<FooterComponent />);
    const currentYear = new Date().getFullYear();
    const copyrightText = screen.getByText(
      `© ${currentYear} Conferenti. All rights reserved.`
    );
    expect(copyrightText).toBeInTheDocument();
  });

  it('has correct footer styling classes', () => {
    render(<FooterComponent />);
    const footer = screen.getByRole('contentinfo');
    expect(footer).toHaveClass('bg-white');
  });

  it('renders logo with correct dimensions', () => {
    render(<FooterComponent />);
    const logo = screen.getByAltText('Conferenti Logo');
    expect(logo).toHaveAttribute('width', '120');
    expect(logo).toHaveAttribute('height', '40');
  });
});
