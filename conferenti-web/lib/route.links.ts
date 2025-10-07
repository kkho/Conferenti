
export function resolveHref(
  documentType?: string,
  slug?: string
): string | undefined {
  if (documentType === "") {
    return '/';
  }

  if (documentType?.startsWith('sessions')) {
    return slug ? `/sessions/${slug}` : '/sessions';
  }

  if (documentType?.startsWith('speakers')) {
    return slug ? `/speakers/${slug}` : '/speakers';
  }

  if (documentType === "faq") {
    return '/faq';
  }

  return undefined;
}