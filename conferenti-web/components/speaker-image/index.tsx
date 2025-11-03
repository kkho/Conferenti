import Image, { ImageProps } from 'next/image';
import { useState } from 'react';

export function SpeakerImage(props: ImageProps) {
  const [src, setSrc] = useState(props.src);

  return (
    <Image
      {...props}
      src={src}
      onError={() => setSrc('/empty-user.png')}
      alt={props.alt}
    />
  );
}
