import { render } from '@testing-library/react';
import { FilterBarContent } from '.';

describe('FilterBarContent', () => {
  it('renders without crashing', () => {
    render(
      <FilterBarContent placeHolder={''} handleFilterChange={function (filter: string): void {
        throw new Error('Function not implemented.');
      } } ></FilterBarContent>
    );

    expect(true).toBe(true);
  });
});
