import { Geist, Geist_Mono } from 'next/font/google';
import './globals.css';
import { Navbar } from '@/lib/nav-bar';
import { MenuItemData } from '@/types';
import { ReactQueryClientProvider } from '@/lib/react-query-client-provider';

const menuItems: MenuItemData[] = [
  { title: 'Home', slug: '', _type: '', current: false },
  { title: 'Sessions', slug: '', _type: 'sessions', current: false },
  { title: 'Speakers', slug: '', _type: 'speakers', current: false },
  { title: 'FAQ', slug: '', _type: 'faq', current: false }
];

const geistSans = Geist({
  variable: '--font-geist-sans',
  subsets: ['latin']
});

const geistMono = Geist_Mono({
  variable: '--font-geist-mono',
  subsets: ['latin']
});

export default function RootLayout({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className="h-full bg-gray-900" suppressHydrationWarning>
      <body className={`h-full antialiased`} suppressHydrationWarning>
        <Navbar menuItems={menuItems} />
        <ReactQueryClientProvider>{children}</ReactQueryClientProvider>
      </body>
    </html>
  );
}
