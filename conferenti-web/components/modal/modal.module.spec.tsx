import { render } from '@testing-library/react';
import Modal from '.';

describe('Modal Module', () => {
  it('renders without crashing', () => {
    render(
      <Modal isOpen={true} onClose={() => {}} title="Test Modal">
        <div>Modal Content</div>
      </Modal>
    );

    expect(true).toBe(true);
  });
});
