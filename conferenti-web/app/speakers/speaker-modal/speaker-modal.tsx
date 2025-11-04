'use client';

import { Speaker } from '@/types';
import Image from 'next/image';
import Modal from '@/components/modal';

interface SpeakerModalProps {
  speaker?: Speaker | null;
  isOpen: boolean;
  onClose: () => void;
}

export default function SpeakerModal({
  speaker,
  isOpen,
  onClose
}: SpeakerModalProps) {
  return (
    <Modal title={''} isOpen={isOpen} onClose={onClose} maxWidth="max-w-2xl">
      <div className="max-h-[70vh] overflow-y-auto">
        <div className="flex flex-col md:flex-row gap-6">
          <div className="flex-shrink-0">
            <Image
              className="w-32 h-32 md:w-40 md:h-40 rounded-full object-cover"
              src={speaker?.photoUrl || '/empty-user.png'}
              alt={speaker?.name || 'Speaker'}
              width={160}
              height={160}
            />
          </div>

          <div className="flex-1">
            <h3 className="text-2xl font-bold text-gray-900 dark:text-white mb-1">
              {speaker?.name}
            </h3>

            {speaker?.position && (
              <p className="text-lg text-gray-600 dark:text-gray-300">
                {speaker?.position}
              </p>
            )}

            {speaker?.company && (
              <p className="text-gray-600 dark:text-gray-300 mb-4">
                {speaker?.company}
              </p>
            )}

            {speaker?.bio && (
              <div className="mb-6">
                <h4 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                  Biography
                </h4>
                <p className="text-gray-700 dark:text-gray-300 leading-relaxed">
                  {speaker?.bio}
                </p>
              </div>
            )}

            {speaker?.sessions && speaker?.sessions.length > 0 && (
              <div>
                <h4 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                  Sessions
                </h4>
                <ul className="space-y-2">
                  {speaker?.sessions.map((session, index) => (
                    <li
                      key={session.id || index}
                      className="bg-gray-50 dark:bg-gray-800 rounded-lg p-3"
                    >
                      <p className="font-medium text-gray-900 dark:text-white">
                        {session.title}
                      </p>
                      {session.startTime && (
                        <p className="text-sm text-gray-600 dark:text-gray-400">
                          {new Date(session.startTime).toLocaleString()}
                        </p>
                      )}
                      {session.room && (
                        <p className="text-sm text-gray-600 dark:text-gray-400">
                          Room: {session.room}
                        </p>
                      )}
                    </li>
                  ))}
                </ul>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Footer */}
      <div className="flex justify-end gap-3 pt-4 mt-6 border-t border-gray-200 dark:border-gray-700">
        <button
          onClick={onClose}
          className="px-4 py-2 rounded-md bg-gray-100 dark:bg-gray-800 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-700"
        >
          Close
        </button>
      </div>
    </Modal>
  );
}
