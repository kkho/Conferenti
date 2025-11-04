import CardComponent from '@/components/card-component';
import { Session } from '@/types';

interface SessionContentProps {
  session?: Session | null;
  handleSessionClick?: (session: Session) => void;
  imageSource: string;
  titleClass?: string;
  descriptionClass?: string;
  className?: string;
  showButtons?: boolean;
}

const SessionContent = ({
  session,
  handleSessionClick,
  imageSource,
  titleClass,
  descriptionClass,
  className,
  showButtons
}: SessionContentProps) => {
  return (
    <section>
      <CardComponent
        title={session?.title || ''}
        description={session?.description || ''}
        imageSource={imageSource}
        titleClass={titleClass}
        descriptionClass={descriptionClass}
        className={className}
        altText={session?.title || ''}
        handleClick={() =>
          handleSessionClick && session && handleSessionClick(session)
        }
        showButtons={showButtons}
      />
    </section>
  );
};

export default SessionContent;
