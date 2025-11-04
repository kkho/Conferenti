import { Session, SessionFormat } from '@/types';
import SessionContent from '../session-content';
import { resolveSessionFormatAsset } from '@/lib/constants';
import SessionModal from '../session-modal';

const SessionList = ({
  sessions,
  selectedSession,
  error,
  className,
  titleClass,
  descriptionClass,
  handleCloseModal,
  handleSessionClick,
  showButtons
}: {
  sessions: Session[];
  selectedSession: Session | null;
  error: Error | null;
  className?: string;
  titleClass?: string;
  descriptionClass?: string;
  handleCloseModal: () => void;
  handleSessionClick: (session: Session) => void;
  showButtons?: boolean;
}) => (
  <>
    <ul
      className={`grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-x-4 gap-y-4 sm:gap-y-16 mt-12 justify-items-stretch`}
    >
      {sessions && sessions.length > 0 ? (
        sessions.map((session) => (
          <SessionContent
            key={session.id}
            session={session}
            className={className}
            titleClass={titleClass}
            descriptionClass={descriptionClass}
            handleSessionClick={handleSessionClick}
            imageSource={resolveSessionFormatAsset(
              session.format || SessionFormat.Workshop
            )}
            showButtons={showButtons}
          />
        ))
      ) : (
        <p>No sessions available</p>
      )}
    </ul>
    {error && (
      <p className="mt-6 text-lg/8 text-gray-400" aria-live="polite">
        Error retrieving sessions
      </p>
    )}
    {selectedSession && (
      <SessionModal
        session={selectedSession}
        isOpen={!!selectedSession}
        onClose={handleCloseModal}
      />
    )}
  </>
);

export default SessionList;
