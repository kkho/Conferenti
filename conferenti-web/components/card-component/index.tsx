import { Image } from '@fluentui/react-components';
import { Button } from '@fluentui/react-components';
import { Open16Regular, BookmarkAddRegular } from '@fluentui/react-icons';
import {
  Card,
  CardHeader,
  CardFooter,
  CardPreview,
  CardProps
} from '@fluentui/react-components';

import styles from './card-component.module.scss';

const CardComponent = (
  props: CardProps & {
    title: string;
    description: string;
    imageSource: string;
    altText: string;
    titleClass?: string;
    descriptionClass?: string;
    className?: string;
    handleClick?: (object: unknown) => void;
    showButtons?: boolean;
  }
) => {
  const {
    title,
    description,
    imageSource,
    altText,
    titleClass,
    descriptionClass,
    className,
    handleClick,
    showButtons
  } = props;
  return (
    <Card className={`${className} ${styles.card}`} {...props}>
      <CardPreview>
        <Image src={imageSource} alt={altText} width="100%" height="100px" />
      </CardPreview>

      <CardHeader
        className={`${styles.cardHeader}`}
        header={
          <h5
            style={{ fontWeight: 'bold' }}
            className={`text-base/7 font-semibold tracking-tight ${
              titleClass ?? 'text-white'
            }`}
          >
            {title}
          </h5>
        }
        description={
          <span
            className={`line-clamp-3 text-sm/6 font-semibold ${
              descriptionClass ?? 'text-gray-200'
            }`}
          >
            {description}
          </span>
        }
      />

      <p className={`${styles.text} text-sm/6 text-gray-600`}></p>

      {showButtons && (
        <CardFooter className={styles.footer}>
          <Button
            appearance="primary"
            icon={<Open16Regular />}
            onClick={handleClick}
          >
            Open
          </Button>
          <Button icon={<BookmarkAddRegular />}>Bookmark</Button>
        </CardFooter>
      )}
    </Card>
  );
};

export default CardComponent;
