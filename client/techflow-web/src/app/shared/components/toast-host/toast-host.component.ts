import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';

import { ToastService, ToastVariant } from '../../../core/notifications/toast.service';

@Component({
  selector: 'app-toast-host',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast-host.component.html',
  styleUrl: './toast-host.component.css',
})
export class ToastHostComponent {
  readonly toast = inject(ToastService);

  iconFor(variant: ToastVariant): string {
    switch (variant) {
      case 'success':
        return 'check_circle';
      case 'error':
        return 'error';
      default:
        return 'info';
    }
  }

  defaultTitle(variant: ToastVariant): string {
    switch (variant) {
      case 'success':
        return 'Done';
      case 'error':
        return 'Something went wrong';
      default:
        return 'Notice';
    }
  }
}
