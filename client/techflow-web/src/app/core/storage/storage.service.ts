import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class StorageService {

  set(key: string, value: string): void {
    try {
      localStorage.setItem(key, value);
    } catch {
      console.warn(`[Storage] write failed — key: ${key}`);
    }
  }

  setObject<T>(key: string, value: T): void {
    this.set(key, JSON.stringify(value));
  }

  get(key: string): string | null {
    return localStorage.getItem(key);
  }

  getObject<T>(key: string): T | null {
    const raw = this.get(key);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as T;
    } catch {
      this.remove(key);   
      return null;
    }
  }

  remove(key: string): void {
    localStorage.removeItem(key);
  }

  clear(): void {
    localStorage.clear();
  }

  has(key: string): boolean {
    return this.get(key) !== null;
  }
}
