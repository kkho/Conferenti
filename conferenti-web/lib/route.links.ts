
export function resolveHref(
  documentType?: string,
  slug?: string
): string | undefined {
  switch (documentType) {
    case "":
      return '/';
    case "sessions":
      return slug ? `/sessions/${slug}` : '/sessions';
    case "speakers":
      return slug ? `/speakers/${slug}` : '/speakers';
    case "faq":
      return '/faq/';
    default:
      return undefined;
  }
}