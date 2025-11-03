'use client';

import { Session, SessionFormat } from '@/types';
import Modal from '@/components/modal';
import SessionContent from '../session-content';
import { resolveSessionFormatAsset } from '@/lib/constants';

interface SessionModalProps {
  session: Session | null;
  className?: string;
  titleClass?: string;
  descriptionClass?: string;
  isOpen: boolean;
  onClose: () => void;
}

export const SessionModal = ({
  session,
  className,
  titleClass,
  descriptionClass,
  isOpen,
  onClose
}: SessionModalProps) => {
  if (!session) return null;

  return (
    <Modal title={''} isOpen={isOpen} onClose={onClose} maxWidth="max-w-2xl">
      <div className="max-h-[70vh] overflow-y-auto">
        <SessionContent
          key={session.id}
          session={session}
          className={className}
          titleClass={titleClass}
          descriptionClass={descriptionClass}
          imageSource={resolveSessionFormatAsset(
            session.format || SessionFormat.Workshop
          )}
        />
      </div>
    </Modal>
  );
};

export default SessionModal;
