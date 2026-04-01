import { HttpErrorResponse } from '@angular/common/http';

type ValidationErrors = Record<string, string[]>;

type ProblemDetails = {
  message?: unknown;
  title?: unknown;
  detail?: unknown;
  errors?: unknown;
};

export function getApiError(err: HttpErrorResponse): string {
  const payload = (err.error ?? {}) as ProblemDetails;
  const validationMessage = flattenValidationErrors(payload.errors);

  if (typeof payload.message === 'string' && payload.message.trim()) {
    return payload.message;
  }

  if (validationMessage) return validationMessage;

  if (typeof payload.detail === 'string' && payload.detail.trim()) {
    return payload.detail;
  }

  if (typeof payload.title === 'string' && payload.title.trim()) {
    return payload.title;
  }

  if (err.status === 0) return 'No internet connection. Check your network.';
  if (err.status === 400) return 'Invalid request.';
  if (err.status === 401) return 'Invalid email or password.';
  if (err.status === 403) return 'You do not have permission.';
  if (err.status === 404) return 'Not found.';
  if (err.status === 409) return 'Already exists.';
  if (err.status >= 500) return 'Server error. Try again later.';

  return 'Something went wrong.';
}

function flattenValidationErrors(errors: unknown): string | null {
  if (!errors || typeof errors !== 'object') return null;

  const allMessages = Object.values(errors as ValidationErrors)
    .flat()
    .filter((item): item is string => typeof item === 'string' && item.trim().length > 0);

  if (allMessages.length === 0) return null;
  return allMessages.join(' ');
}
