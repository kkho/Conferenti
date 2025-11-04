import { render } from '@testing-library/react';
import { SpeakerImage } from '.';

describe('SpeakerImage', () => {
  it('renders without crashing', () => {
    render(
      <SpeakerImage
        src="/test-image.png"
        alt="Test Speaker"
        width={100}
        height={100}
      />
    );

    expect(true).toBe(true);
  });
});
