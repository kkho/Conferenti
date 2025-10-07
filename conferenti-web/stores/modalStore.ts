import { create } from "zustand"

type ModalStore = {
  isOpen: boolean;
  open: () => void;
  close: () => void;
  toggle: () => void;
};

const useModalStore = create<ModalStore>((set) => ({
  isOpen: false,
  open: () => set({ isOpen: true }),
  close: () => set({ isOpen: false }),
  toggle: () => set((s: { isOpen: boolean }) => ({ isOpen: !s.isOpen })),
}));

export default useModalStore;