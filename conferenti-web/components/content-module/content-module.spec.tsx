import { render } from "@testing-library/react";
import { Content } from "./index";



describe('Content', () => {
  it('renders without crashing', () => { 
    render(<Content title="Test Title" description="Test Description" />);
    // Basic smoke test
    expect(true).toBe(true);
   });
})
