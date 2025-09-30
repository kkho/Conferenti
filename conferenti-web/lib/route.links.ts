
export function resolveHref(
  documentType?: string,
  slug?: string
): string | undefined {
  switch (documentType) {
    case "sessions":
      return slug ? `/sessions/${slug}` : undefined;
    case "speakers":
      return slug ? `/speakers/${slug}` : undefined;
    case "faq":
      return '/faq/';
    default:
      return undefined;
  }
}