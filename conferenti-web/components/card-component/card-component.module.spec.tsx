import { render, screen, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom';
import CardComponent from './index';

describe('CardComponent', () => {
  const mockProps = {
    title: 'Test Card Title',
    description: 'This is a test description for the card component.',
    imageSource: '/test-image.png',
    altText: 'Test Image',
    handleClick: jest.fn(),
    showButtons: true
  };

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('should render the card with title and description', () => {
    render(<CardComponent {...mockProps} />);

    expect(screen.getByText('Test Card Title')).toBeInTheDocument();
    expect(
      screen.getByText('This is a test description for the card component.')
    ).toBeInTheDocument();
  });

  it('should render the image with correct src and alt text', () => {
    render(<CardComponent {...mockProps} />);

    const image = screen.getByAltText('Test Image');
    expect(image).toBeInTheDocument();
    expect(image).toHaveAttribute('src', '/test-image.png');
  });

  it('should render Open button', () => {
    render(<CardComponent {...mockProps} />);

    const openButton = screen.getByRole('button', { name: /open/i });
    expect(openButton).toBeInTheDocument();
  });

  it('should call handleClick when Open button is clicked', () => {
    render(<CardComponent {...mockProps} />);
    const openButton = screen.getByRole('button', { name: /open/i });
    fireEvent.click(openButton);

    expect(mockProps.handleClick).toHaveBeenCalledTimes(1);
  });

  it('should truncate long description with line-clamp', () => {
    const longDescription =
      'This is a very long description that should be truncated after three lines. It contains a lot of text to test the line-clamp functionality and ensure that ellipsis appears correctly when the text exceeds the maximum allowed lines.';

    render(<CardComponent {...mockProps} description={longDescription} />);

    const descriptionElement = screen.getByText(longDescription);
    expect(descriptionElement).toBeInTheDocument();
    expect(descriptionElement).toHaveClass('line-clamp-3');
  });

  it('should apply correct CSS classes', () => {
    const { container } = render(<CardComponent {...mockProps} />);

    // Check if card has the correct class
    const card = container.querySelector('[class*="card"]');
    expect(card).toBeInTheDocument();
  });
});
