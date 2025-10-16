import { getByText, render } from "@testing-library/react";
import { Content } from "./index";



describe('Content', () => {
  it('renders without crashing', () => { 
    render(<Content imageUrl="https://example.com/image.jpg" title="Test Title" description="Test Description" />);
    // Basic smoke test
    expect(true).toBe(true);
  });

  it('renders the title correctly', () => {
    const { getByText } = render(
      <Content imageUrl="https://example.com/image.jpg" title="Test Title" description="test description" />
    );
    expect(getByText("Test Title")).toBeInTheDocument();
  })


})
