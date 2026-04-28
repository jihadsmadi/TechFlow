import { Injectable, signal } from '@angular/core';

export type ToastVariant = 'success' | 'error' | 'info';

export interface ToastItem {
  readonly id: string;
  readonly variant: ToastVariant;
  readonly message: string;
  readonly title?: string;
  leaving: boolean;
}

const DEFAULT_DURATION_MS = 4000;

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly toasts = signal<ToastItem[]>([]);

  private seq = 0;
  private timers = new Map<string, ReturnType<typeof setTimeout>>();
  private exitTimers = new Map<string, ReturnType<typeof setTimeout>>();

  success(message: string, title?: string, durationMs = DEFAULT_DURATION_MS): void {
    this.push('success', message, title, durationMs);
  }

  error(message: string, title?: string, durationMs = DEFAULT_DURATION_MS): void {
    this.push('error', message, title, durationMs);
  }

  info(message: string, title?: string, durationMs = DEFAULT_DURATION_MS): void {
    this.push('info', message, title, durationMs);
  }

  dismiss(id: string): void {
    const item = this.toasts().find((t) => t.id === id);
    if (!item || item.leaving) return;

    clearTimeout(this.timers.get(id));
    this.timers.delete(id);

    this.patch(id, (t) => ({ ...t, leaving: true }));

    const exitTimer = setTimeout(() => {
      this.exitTimers.delete(id);
      this.toasts.update((list) => list.filter((t) => t.id !== id));
    }, 220);
    this.exitTimers.set(id, exitTimer);
  }

  private push(variant: ToastVariant, message: string, title: string | undefined, durationMs: number): void {
    const id = `tf-toast-${++this.seq}`;
    const item: ToastItem = { id, variant, message, title, leaving: false };
    this.toasts.update((list) => [...list, item]);

    if (durationMs > 0) {
      const t = setTimeout(() => this.dismiss(id), durationMs);
      this.timers.set(id, t);
    }
  }

  private patch(id: string, fn: (t: ToastItem) => ToastItem): void {
    this.toasts.update((list) => list.map((t) => (t.id === id ? fn(t) : t)));
  }
}
