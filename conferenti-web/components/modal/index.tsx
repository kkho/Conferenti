import useModalStore from '@/stores/modalStore';
import { useEffect, useRef } from 'react';
import { createPortal } from 'react-dom';
import { motion, AnimatePresence } from 'framer-motion';

const Modal = ({
  children,
  title = 'Session',
  isOpen: propIsOpen,
  onClose: propOnClose,
  maxWidth = 'max-w-lg'
}: {
  children: React.ReactNode;
  title: string;
  isOpen?: boolean;
  onClose?: () => void;
  maxWidth?: string;
}) => {
  const { isOpen: storeIsOpen, close: storeClose } = useModalStore();
  
  // Use props if provided, otherwise use store
  const isOpen = propIsOpen !== undefined ? propIsOpen : storeIsOpen;
  const close = propOnClose || storeClose;
  const containerRef = useRef<HTMLDivElement | null>(null);
  const previouslyFocused = useRef<Element | null>(null);

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    previouslyFocused.current = document.activeElement;
    const originalOverflow = document.body.style.overflow;
    document.documentElement.style.overflow = 'hidden';

    const timer = setTimeout(() => {
      const first = containerRef.current?.querySelector<HTMLElement>(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
      );

      first?.focus();
    }, 50);

    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        close();
      }

      if (e.key === 'Tab') {
        const focusables = Array.from(
          containerRef.current
            ? containerRef.current.querySelectorAll(
                'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
              )
            : []
        ).filter((el) => !el.hasAttribute('disabled'));

        if (focusables.length === 0) {
          return;
        }

        const idx = focusables.indexOf(document.activeElement!);

        if (e.shiftKey && idx === 0) {
          e.preventDefault();
          (focusables[focusables.length - 1] as HTMLElement).focus();
        }

        if (!e.shiftKey && idx === focusables.length - 1) {
          e.preventDefault();
          (focusables[0] as HTMLElement).focus();
        }
      }
    };

    document.addEventListener('keydown', onKey);

    return () => {
      clearTimeout(timer);
      document.removeEventListener('keydown', onKey);
      document.documentElement.style.overflow = originalOverflow;
      (previouslyFocused.current as HTMLElement | null)?.focus();
    };
  }, [isOpen, close]);

  if (typeof document === 'undefined') {
    return null;
  }

  return createPortal(
    <AnimatePresence>
      {isOpen && (
        <motion.div
          className="fixed inset-0 z-50 flex items-center justify-center"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          aria-modal="true"
          role="dialog"
          aria-label={title}
        >
          {/* Overlay */}
          <motion.div
            className="absolute inset-0 bg-black/40"
            onClick={close}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
          />

          {/* Modal panel */}
          <motion.div
            ref={containerRef}
            className={`relative z-10 w-full ${maxWidth} mx-4 bg-white dark:bg-gray-900 rounded-2xl shadow-2xl overflow-hidden`}
            initial={{ y: 30, opacity: 0, scale: 0.98 }}
            animate={{ y: 0, opacity: 1, scale: 1 }}
            exit={{ y: 20, opacity: 0, scale: 0.99 }}
            transition={{ type: 'spring', stiffness: 320, damping: 30 }}
          >
            <div className="flex items-start justify-between gap-4 p-6">
              <h3 className="text-lg font-semibold">{title}</h3>
              <button
                onClick={close}
                className="ml-auto rounded-md p-1 focus:outline-none focus:ring-2 focus:ring-offset-2"
                aria-label="Close modal"
              >
                ✕
              </button>
            </div>

            <div className="px-6 pb-6">{children}</div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>,
    document.body
  );
};

export default Modal;
