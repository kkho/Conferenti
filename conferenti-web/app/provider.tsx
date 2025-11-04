'use client';
import { PropsWithChildren } from 'react';
import {
  FluentProvider,
  teamsDarkV21Theme,
  webLightTheme
} from '@fluentui/react-components';
import { ReactQueryClientProvider } from '@/lib/react-query-client-provider';
import { MenuItemData } from '@/types';
import { Navbar } from '@/components/nav-bar';

const menuItems: MenuItemData[] = [
  { title: 'Home', slug: '', _type: '', current: false },
  { title: 'Sessions', slug: '', _type: 'sessions', current: false },
  { title: 'Speakers', slug: '', _type: 'speakers', current: false },
  { title: 'FAQ', slug: '', _type: 'faq', current: false }
];

export function Providers({ children }: PropsWithChildren) {
  return (
    <FluentProvider>
      <Navbar menuItems={menuItems} />
      <ReactQueryClientProvider>{children}</ReactQueryClientProvider>
    </FluentProvider>
  );
}
