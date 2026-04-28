import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient, HttpBackend, HttpContext } from '@angular/common/http';
import { Router } from '@angular/router';
import { EMPTY, Observable, of } from 'rxjs';
import { catchError, finalize, map, shareReplay, switchMap, tap } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { TokenService } from './token.service';
import { getApiError } from '../../shared/utils/api-error';
import {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  UserAuthDto,
} from '../../shared/models/auth.model';
import { User } from '../../shared/models/user.model';
import { SKIP_AUTH } from './auth-http-context';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private tokenService = inject(TokenService);
  private httpBackend = inject(HttpBackend);

  private readonly rawHttp = new HttpClient(this.httpBackend);
  private readonly base = `${environment.apiUrl}/api`;

  private _user = signal<User | null>(null);
  private _loading = signal(false);
  private _error = signal<string | null>(null);

  private refreshInFlight$?: Observable<boolean>;

  user = this._user.asReadonly();
  loading = this._loading.asReadonly();
  error = this._error.asReadonly();
  isLoggedIn = computed(() => this._user() !== null);
  fullName = computed(() => this._user()?.fullName ?? '');
  role = computed(() => this._user()?.role ?? '');

  init(): void {
    const cached = this.tokenService.getUser<User>();
    if (cached && this.tokenService.hasValidAccessToken()) {
      this._user.set(cached);
      this.getCurrentUser().subscribe();
      return;
    }

    this.clearSession();
  }

  login(req: LoginRequest) {
    this._loading.set(true);
    this._error.set(null);

    return this.http.post<AuthResponse>(`${this.base}/auth/login`, req, {
      context: new HttpContext().set(SKIP_AUTH, true),
    }).pipe(
      tap((res) => this.handleAuthResponse(res)),
      switchMap((res) =>
        this.getCurrentUser().pipe(
          map(() => res),
          catchError(() => of(res)),
        ),
      ),
      catchError((err) => {
        this._error.set(getApiError(err));
        return EMPTY;
      }),
      finalize(() => this._loading.set(false)),
    );
  }

  register(req: RegisterRequest) {
    this._loading.set(true);
    this._error.set(null);

    return this.http.post<AuthResponse>(`${this.base}/auth/register`, req, {
      context: new HttpContext().set(SKIP_AUTH, true),
    }).pipe(
      tap((res) => this.handleAuthResponse(res)),
      switchMap((res) =>
        this.getCurrentUser().pipe(
          map(() => res),
          catchError(() => of(res)),
        ),
      ),
      catchError((err) => {
        this._error.set(getApiError(err));
        return EMPTY;
      }),
      finalize(() => this._loading.set(false)),
    );
  }

  getCurrentUser(): Observable<User | null> {
    if (!this.tokenService.hasValidAccessToken()) return of(null);

    return this.http.get<User>(`${this.base}/users/me`).pipe(
      tap((user) => {
        this.tokenService.setUser(user);
        this.tokenService.setRole(user.role);
        this._user.set(user);
      }),
      catchError(() => of(null)),
    );
  }

  refreshSession(): Observable<boolean> {
    if (this.refreshInFlight$) return this.refreshInFlight$;
    if (!this.tokenService.hasValidRefreshToken()) return of(false);

    const refreshToken = this.tokenService.getRefreshToken();
    const accessToken = this.tokenService.getAccessToken();
    if (!refreshToken || !accessToken) return of(false);

    this.refreshInFlight$ = this.rawHttp
      .post<AuthResponse>(`${this.base}/auth/refresh`, { accessToken, refreshToken })
      .pipe(
        tap((res) => this.handleAuthResponse(res)),
        switchMap(() => this.getCurrentUser()),
        map(() => true),
        catchError(() => of(false)),
        finalize(() => {
          this.refreshInFlight$ = undefined;
        }),
        shareReplay(1),
      );

    return this.refreshInFlight$;
  }

  logout(): void {
    this.clearSession();
    this.router.navigate(['/']);
  }

  handleUnauthorizedRedirect(): void {
    this.clearSession();
    this.router.navigate(['/login']);
  }

  clearError(): void {
    this._error.set(null);
  }

  private handleAuthResponse(res: AuthResponse): void {
    this.tokenService.setAccessToken(res.accessToken);
    this.tokenService.setRefreshToken(res.refreshToken);
    this.tokenService.setAccessExpiresAt(res.expiresAt);
    this.tokenService.setRefreshExpiresAt(res.refreshTokenExpiresAt);

    const user = this.fromAuthUser(res.user);
    this.tokenService.setUser(user);
    this.tokenService.setRole(user.role);
    this._user.set(user);
  }

  private fromAuthUser(user: UserAuthDto): User {
    return {
      ...user,
      avatarUrl: null,
      isActive: true,
    };
  }

  private clearSession(): void {
    this.tokenService.clearAll();
    this._user.set(null);
    this._error.set(null);
  }
}
