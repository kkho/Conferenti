import SpeakerModal from '@/app/speakers/speaker-modal/speaker-modal';
import { Speaker } from '@/types';
import { SpeakerImage } from '../../../components/speaker-image';

export const SpeakerList = ({
  speakers,
  selectedSpeaker,
  error,
  classes,
  handleCloseModal,
  handleSpeakerClick
}: {
  speakers: Speaker[];
  selectedSpeaker: Speaker | null;
  error: Error | null;
  classes?: string;
  handleCloseModal: () => void;
  handleSpeakerClick: (speaker: Speaker) => void;
}) => {
  return (
    <>
      <ul
        className={`${classes} grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-x-4 gap-y-4 sm:gap-y-16 mt-12 justify-items-stretch`}
      >
        {speakers && speakers.length > 0 ? (
          speakers.map((speaker) => (
            <li key={speaker.id || speaker.name}>
              <div
                className="flex items-center flex-col gap-x-6 cursor-pointer p-4 rounded-lg hover:bg-gray-300 transition-colors bg-[#f8f9fa]"
                onClick={() => handleSpeakerClick(speaker)}
              >
                <SpeakerImage
                  className="size48 rounded-full outline-1 -outline-offset-1 outline-white/10"
                  src={speaker.photoUrl || '/empty-user.png'}
                  alt={speaker.name || 'Speaker'}
                  width={128}
                  height={128}
                />
                <div className="text-center mt-4">
                  <h3 className="text-base/7 font-semibold tracking-tight text-gray-900">
                    {speaker.name}
                  </h3>
                  <p className="text-sm/6 font-semibold text-gray-700">
                    {speaker.position}
                  </p>
                  {speaker.company && (
                    <p className="text-sm/6 text-gray-600">{speaker.company}</p>
                  )}
                </div>
              </div>
            </li>
          ))
        ) : (
          <>No speakers found</>
        )}
      </ul>

      {error && (
        <p className="mt-6 text-lg/8 text-gray-400" aria-live="polite">
          Error retrieving speakers
        </p>
      )}

      {selectedSpeaker && (
        <SpeakerModal
          speaker={selectedSpeaker}
          isOpen={!!selectedSpeaker}
          onClose={handleCloseModal}
        />
      )}
    </>
  );
};

export default SpeakerList;
