import Image from 'next/image';

export interface Props {
  classes?: string;
  imageUrl?: string;
  title: string;
  description?: string;
  extraClasses?: string;
  textClasses?: string;
  imageHeight?: number;
  imageWidth?: number;
  imageClasses?: string;
}

export const Content = (props: Props) => {
  const {
    title,
    description,
    imageUrl,
    extraClasses,
    classes,
    textClasses,
    imageHeight,
    imageWidth,
    imageClasses
  } = props;
  return (
    <>
      <div className={`flex items-start ${classes}`}>
        <div className={`${extraClasses}`}>
          <h2
            className={`text-3xl font-semibold tracking-tight text-pretty sm:text-4xl z-index-11 ${textClasses}`}
          >
            {title}
          </h2>
          <p className="mt-6 text-lg/8 max-w-[700px] text-gray-600 z-index-1">
            {description}
          </p>
        </div>
        {imageUrl && (
          <Image
            src={imageUrl}
            alt={title}
            className={`mt-6 mb-8 rounded-lg object-cover ${imageClasses}`}
            style={{
              height: imageHeight ? `${imageHeight}px` : 'auto'
            }}
            width={imageWidth ?? 300}
            height={imageHeight ?? 300}
          />
        )}
      </div>
    </>
  );
};
