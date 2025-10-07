import './globals.css';
import { Navbar } from '@/components/nav-bar';
import { MenuItemData } from '@/types';
import { ReactQueryClientProvider } from '@/lib/react-query-client-provider';
import { Metadata } from 'next';
import Image from 'next/image';

const menuItems: MenuItemData[] = [
  { title: 'Home', slug: '', _type: '', current: false },
  { title: 'Sessions', slug: '', _type: 'sessions', current: false },
  { title: 'Speakers', slug: '', _type: 'speakers', current: false },
  { title: 'FAQ', slug: '', _type: 'faq', current: false }
];

export const metadata: Metadata = {
  title: 'Conferenti',
  description: 'Your conference app'
};

export default function RootLayout({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className="h-screen bg-gray-900" suppressHydrationWarning>
      <body className={`h-auto antialiased`} suppressHydrationWarning>
        <Navbar menuItems={menuItems} />
        <ReactQueryClientProvider>{children}</ReactQueryClientProvider>
        <footer className="row-start-3 flex gap-[24px] flex-wrap items-center justify-center">
          <a
            className="flex items-center gap-2 hover:underline hover:underline-offset-4"
            href="https://nextjs.org/learn?utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
            target="_blank"
            rel="noopener noreferrer"
          >
            <Image
              aria-hidden
              src="/file.svg"
              alt="File icon"
              width={16}
              height={16}
            />
            Learn
          </a>
          <a
            className="flex items-center gap-2 hover:underline hover:underline-offset-4"
            href="https://vercel.com/templates?framework=next.js&utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
            target="_blank"
            rel="noopener noreferrer"
          >
            <Image
              aria-hidden
              src="/window.svg"
              alt="Window icon"
              width={16}
              height={16}
            />
            Examples
          </a>
          <a
            className="flex items-center gap-2 hover:underline hover:underline-offset-4"
            href="https://nextjs.org?utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
            target="_blank"
            rel="noopener noreferrer"
          >
            <Image
              aria-hidden
              src="/globe.svg"
              alt="Globe icon"
              width={16}
              height={16}
            />
            Go to nextjs.org →
          </a>
        </footer>
      </body>
    </html>
  );
}
