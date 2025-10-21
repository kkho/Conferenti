import Image from 'next/image';

export interface Props {
  imageUrl: string;
  title: string;
  description?: string;
}

export const Content = (props: Props) => {
  const { title, description, imageUrl } = props;
  return (
    <>
      <div className="flex flex-row items-start justify-between">
        <div className="pt-24">
          <h2 className="text-3xl font-semibold tracking-tight text-pretty text-white sm:text-4xl">
            {title}
          </h2>
          <p className="mt-6 text-lg/8 text-gray-400">{description}</p>
        </div>
        <Image
          src={imageUrl}
          alt={title}
          className="mt-6 rounded-lg"
          width={200}
          height={200}
        />
      </div>
    </>
  );
};
