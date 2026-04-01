import { Injectable, inject } from '@angular/core';
import { StorageService } from '../storage/storage.service';

@Injectable({ providedIn: 'root' })
export class TokenService {
  private storage = inject(StorageService);

  private readonly KEYS = {
    accessToken: 'tf_access_token',
    refreshToken: 'tf_refresh_token',
    user: 'tf_user',
    role: 'tf_role',
    expiresAt: 'tf_access_expires_at',
    refreshTokenExpiresAt: 'tf_refresh_expires_at',
  } as const;

  getAccessToken(): string | null {
    return this.storage.get(this.KEYS.accessToken);
  }

  setAccessToken(token: string): void {
    this.storage.set(this.KEYS.accessToken, token);
  }

  hasToken(): boolean {
    return this.storage.has(this.KEYS.accessToken);
  }

  hasValidAccessToken(): boolean {
    const token = this.getAccessToken();
    if (!token || !this.isJwt(token)) return false;
    return !this.isAccessTokenExpired(token);
  }

  getRefreshToken(): string | null {
    return this.storage.get(this.KEYS.refreshToken);
  }

  setRefreshToken(token: string): void {
    this.storage.set(this.KEYS.refreshToken, token);
  }

  getAccessExpiresAt(): string | null {
    return this.storage.get(this.KEYS.expiresAt);
  }

  setAccessExpiresAt(isoDate: string): void {
    this.storage.set(this.KEYS.expiresAt, isoDate);
  }

  getRefreshExpiresAt(): string | null {
    return this.storage.get(this.KEYS.refreshTokenExpiresAt);
  }

  setRefreshExpiresAt(isoDate: string): void {
    this.storage.set(this.KEYS.refreshTokenExpiresAt, isoDate);
  }

  hasValidRefreshToken(): boolean {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken || !this.isJwt(refreshToken)) return false;

    const storedExpiry = this.getRefreshExpiresAt();
    if (storedExpiry) {
      return Date.now() < Date.parse(storedExpiry);
    }

    const exp = this.getExpClaim(refreshToken);
    if (!exp) return true;
    return Date.now() < exp * 1000;
  }

  getUser<T>(): T | null {
    return this.storage.getObject<T>(this.KEYS.user);
  }

  setUser<T>(user: T): void {
    this.storage.setObject(this.KEYS.user, user);
  }

  getRole(): string | null {
    return this.storage.get(this.KEYS.role);
  }

  setRole(role: string): void {
    this.storage.set(this.KEYS.role, role);
  }

  clearAll(): void {
    Object.values(this.KEYS).forEach((key) => this.storage.remove(key));
  }

  private isAccessTokenExpired(token: string): boolean {
    const storedExpiry = this.getAccessExpiresAt();
    if (storedExpiry) {
      return Date.now() >= Date.parse(storedExpiry);
    }

    const exp = this.getExpClaim(token);
    if (!exp) return true;
    return Date.now() >= exp * 1000;
  }

  private isJwt(token: string): boolean {
    const parts = token.split('.');
    if (parts.length !== 3) return false;
    return this.decodePayload(token) !== null;
  }

  private getExpClaim(token: string): number | null {
    const payload = this.decodePayload<{ exp?: unknown }>(token);
    const exp = payload?.exp;
    return typeof exp === 'number' ? exp : null;
  }

  private decodePayload<T extends object>(token: string): T | null {
    try {
      const payload = token.split('.')[1];
      if (!payload) return null;
      const normalized = payload.replace(/-/g, '+').replace(/_/g, '/');
      const padded = normalized + '='.repeat((4 - (normalized.length % 4)) % 4);
      const json = atob(padded);
      return JSON.parse(json) as T;
    } catch {
      return null;
    }
  }
}
