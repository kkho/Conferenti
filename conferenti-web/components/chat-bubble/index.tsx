'use client';

import { useRef, useEffect, useState, ChangeEvent } from 'react';
import {
  Chat48Regular,
  ChatAdd24Regular,
  Notebook24Regular,
  Send24Regular
} from '@fluentui/react-icons';
import { AnimatePresence, motion } from 'framer-motion';
import {
  Avatar,
  FluentProvider,
  Input,
  webLightTheme
} from '@fluentui/react-components';
import { Chat, ChatMessage, ChatMyMessage } from '@fluentui-contrib/react-chat';
import { useAiChat } from '@/hooks/useAiChat';
import { useUser } from '@auth0/nextjs-auth0';
import { Role } from '@/types';
import styles from './chat-bubble.module.scss';

const BouncingDots = () => (
  <div className="flex items-center space-x-1 mt-2">
    <div className="w-2.5 h-2.5 dark:bg-gray-900 rounded-full animate-bounce [animation-delay:0ms]"></div>
    <div className="w-2.5 h-2.5 dark:bg-gray-900 rounded-full animate-bounce [animation-delay:150ms]"></div>
    <div className="w-2.5 h-2.5 dark:bg-gray-900 rounded-full animate-bounce [animation-delay:300ms]"></div>
  </div>
);

const ChatBubbleComponent = () => {
  const { messages, isLoading, error, sendMessage, startNewChat } = useAiChat();
  const { user, isLoading: isUserLoading } = useUser(); // Get Auth0 user
  const [inputValue, setInputValue] = useState('');
  const wrappedRef = useRef(null);
  const containerRef = useRef<HTMLDivElement | null>(null);
  const [buttonTitle, setButtonTitle] = useState<string>('Open chat');
  const [isOpen, setIsOpen] = useState<boolean>(false);

  const handleInputChange = (event: ChangeEvent<HTMLInputElement>): void =>
    setInputValue(event.target.value);

  const handleSendMessage = async () => {
    if (!inputValue.trim()) {
      return;
    }
    const userId = user?.sub || user?.email || 'user';

    await sendMessage(userId, inputValue);
    setInputValue('');
  };

  const handleKeyPress = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      handleSendMessage();
    }
  };

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
                    <h3 className="text-lg font-semibold bg-white">Hello</h3>
                    <button
                      onClick={() => setIsOpen(false)}
                      className="ml-auto rounded-md p-1 focus:outline-none focus:ring-2 focus:ring-offset-2 text-white"
                      aria-label="Close modal"
                    >
                      ✕
                    </button>
                  </div>

                  <div
                    className={`px-6 pb-6 ${styles.chatWindow} overflow-y-auto overflow-x-hidden scrollbar`}
                  >
                    <Chat>
                      <ChatMessage
                        avatar={
                          <Avatar
                            name="Conferenti Bot"
                            color="colorful"
                            badge={{ status: 'available' }}
                          />
                        }
                      >
                        Hello I am Conferenti Bot! Your AI-assistant.
                      </ChatMessage>
                      {!messages ||
                        (messages.length === 0 && <ChatMessage></ChatMessage>)}

                      {messages.map((msg) =>
                        msg.role === Role.Assistant ? (
                          <ChatMessage
                            avatar={
                              <Avatar
                                name="Conferenti Bot"
                                color="colorful"
                                badge={{ status: 'available' }}
                              />
                            }
                            key={msg.id}
                          >
                            {msg.content}
                          </ChatMessage>
                        ) : (
                          <ChatMyMessage key={msg.id}>
                            {msg.content}
                          </ChatMyMessage>
                        )
                      )}

                      {isLoading && (
                        <>
                          <ChatMessage
                            avatar={
                              <Avatar
                                name="Conferenti Bot"
                                color="colorful"
                                badge={{ status: 'available' }}
                              />
                            }
                          >
                            <BouncingDots />
                          </ChatMessage>
                        </>
                      )}
                    </Chat>
                  </div>

                  <div
                    className={`${styles.bottomContainer} bg-white w-full pt-2 pb-2 pl-4 pr-4`}
                  >
                    <input
                      type="text"
                      id="ChatTextField"
                      className="field-107 w-full "
                      placeholder="Ask me anything about Conferenti..."
                      aria-invalid="false"
                      value={inputValue}
                      onChange={handleInputChange}
                      onKeyUp={handleKeyPress}
                    ></input>
                    <div className="bottom-row flex justify-between items-center w-full">
                      <div className={`${styles.bottomLeftContainer}`}>
                        <button
                          className="flex align-center gap-4 p-2 text-gray-600"
                          aria-label="New Chat"
                          onClick={startNewChat}
                        >
                          <ChatAdd24Regular />
                        </button>
                        <button
                          className="flex align-center gap-8 p-2 text-gray-600"
                          aria-label="Prompt Guide"
                        >
                          <Notebook24Regular />
                        </button>
                      </div>
                      <div className="bottom-right-container pr-2 pl-2">
                        <button
                          aria-label="Send"
                          onClick={handleSendMessage}
                          className={`
                            ${
                              inputValue
                                ? 'text-gray-600 cursor-pointer'
                                : 'text-gray-400 cursor-not-allowed'
                            }`}
                          disabled={inputValue ? false : true}
                        >
                          <Send24Regular />
                        </button>
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
