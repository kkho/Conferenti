'use client';

import { useRef, useEffect, useState } from 'react';
import { Chat48Regular } from '@fluentui/react-icons';
import { AnimatePresence, motion } from 'framer-motion';
import {
  Avatar,
  FluentProvider,
  Input,
  webLightTheme
} from '@fluentui/react-components';
import { Chat, ChatMessage, ChatMyMessage } from '@fluentui-contrib/react-chat';

const ChatBubbleComponent = () => {
  const wrappedRef = useRef(null);
  const containerRef = useRef<HTMLDivElement | null>(null);
  const [buttonTitle, setButtonTitle] = useState<string>('Open chat');
  const [isOpen, setIsOpen] = useState<boolean>(false);

  useEffect(() => {});

  return (
    <FluentProvider theme={webLightTheme}>
      <AnimatePresence>
        <div className="absolute block right-0 mr-4 w-150 z-50">
          <div className="fixed bottom-8 right-0 mr-12">
            {isOpen && (
              <motion.div
                className="fixed bottom-28 right-8 z-50 pointer-events-none w-150"
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                exit={{ opacity: 0 }}
                aria-modal="true"
                role="dialog"
                aria-label="Chat window"
              >
                <motion.div
                  ref={containerRef}
                  className={`relative bg-white dark:bg-gray-900 rounded-2xl shadow-2xl overflow-hidden pointer-events-auto`}
                  initial={{ y: 30, opacity: 0, scale: 0.98 }}
                  animate={{ y: 0, opacity: 1, scale: 1 }}
                  exit={{ y: 20, opacity: 0, scale: 0.99 }}
                  transition={{ type: 'spring', stiffness: 320, damping: 30 }}
                >
                  <div className="flex items-start justify-between gap-4 p-6">
                    <h3 className="text-lg font-semibold">Hello</h3>
                    <button
                      onClick={() => setIsOpen(false)}
                      className="ml-auto rounded-md p-1 focus:outline-none focus:ring-2 focus:ring-offset-2 text-white"
                      aria-label="Close modal"
                    >
                      ✕
                    </button>
                  </div>

                  <div className="px-6 pb-6">
                    <Chat>
                      <ChatMessage
                        avatar={
                          <Avatar
                            name="Ashley McCarthy"
                            color="colorful"
                            badge={{ status: 'available' }}
                          />
                        }
                      >
                        Hello I am Ashley
                      </ChatMessage>
                      <ChatMyMessage>Nice to meet you!</ChatMyMessage>
                    </Chat>

                    <div className="bottom-container">
                      <input
                        type="text"
                        id="ChatTextField"
                        className="ms-TextField-field field-107"
                        placeholder="Ask me anything about Conferenti..."
                        aria-invalid="false"
                      ></input>
                      <div className="bottom-row flex justify-between items-center gap-4">
                        <div className="bottom-left-container">
                          <button>New Chat</button>
                          <button>Prompt Guide</button>
                        </div>
                        <div className="bottom-right-container"></div>
                      </div>
                    </div>
                  </div>
                </motion.div>
              </motion.div>
            )}
            <button
              id="chat-button"
              className="bg-[#392F94] hover:bg-[#2C1536] text-white rounded-full p-3 shadow-lg cursor-pointer"
              onClick={() => setIsOpen(!isOpen)}
              title={buttonTitle}
              aria-label="Start chat"
            >
              <Chat48Regular />
            </button>
          </div>
        </div>
      </AnimatePresence>
    </FluentProvider>
  );
};

export default ChatBubbleComponent;
