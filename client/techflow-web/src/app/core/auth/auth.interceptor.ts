import {
  HttpInterceptorFn,
  HttpErrorResponse,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';

import { TokenService } from './token.service';
import { AuthService } from './auth.service';
import { AUTH_RETRY, SKIP_AUTH } from './auth-http-context';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const authService = inject(AuthService);

  if (req.context.get(SKIP_AUTH)) {
    return next(req);
  }

  const token = tokenService.getAccessToken();
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status !== 401) {
        return throwError(() => err);
      }

      if (req.context.get(AUTH_RETRY)) {
        authService.handleUnauthorizedRedirect();
        return throwError(() => err);
      }

      return authService.refreshSession().pipe(
        switchMap((refreshed) => {
          if (!refreshed) {
            authService.handleUnauthorizedRedirect();
            return throwError(() => err);
          }

          const nextToken = tokenService.getAccessToken();
          if (!nextToken) {
            authService.handleUnauthorizedRedirect();
            return throwError(() => err);
          }

          const retryReq = req.clone({
            setHeaders: { Authorization: `Bearer ${nextToken}` },
            context: req.context.set(AUTH_RETRY, true),
          });
          return next(retryReq);
        }),
        catchError((refreshError) => {
          authService.handleUnauthorizedRedirect();
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
