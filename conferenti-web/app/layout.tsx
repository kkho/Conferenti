import './globals.css';
import { Metadata } from 'next';
import { Providers } from './provider';
import FooterComponent from '@/components/footer';

export const metadata: Metadata = {
  title: 'Conferenti',
  description: 'Your conference app',
  icons: {
    icon: '/favicon.ico',
    shortcut: '/favicon.ico',
    apple: '/favicon.ico'
  }
};

export default function RootLayout({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className="h-screen bg-gray-900" suppressHydrationWarning>
      <body className={`h-auto antialiased`} suppressHydrationWarning>
        <a href="#main-content" className="skip-link">
          Skip to main content
        </a>
        <Providers>
          {children}
          <FooterComponent />
        </Providers>
      </body>
    </html>
  );
}
