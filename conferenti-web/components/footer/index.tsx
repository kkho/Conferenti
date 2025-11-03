import Image from 'next/image';

const FooterComponent = () => {
  return (
    <footer role="contentinfo" className="bg-white">
      <div className="m-auto max-w-1440px pl-18 pr-18">
        <>
          <div className="flex justify-between items-center py-6">
            <Image
              src="/conferenti_footer_logo.png"
              alt="Conferenti Logo"
              width={120}
              height={40}
            />
            <p className="text-sm text-gray-500">
              © {new Date().getFullYear()} Conferenti. All rights reserved.
            </p>
          </div>
        </>
      </div>
    </footer>
  );
};

export default FooterComponent;
