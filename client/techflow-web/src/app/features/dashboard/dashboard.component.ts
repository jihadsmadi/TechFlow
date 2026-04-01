import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { CurrentUserCardComponent } from '../../shared/components/current-user-card/current-user-card.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, CurrentUserCardComponent],
  template: `
    <div class="dashboard-shell">
      <header class="dashboard-header">
        <h1>Dashboard</h1>

        <div class="header-actions">
          <button class="icon-btn" type="button" (click)="toggleProfileTab()" aria-label="View profile">
            <svg viewBox="0 0 24 24" aria-hidden="true">
              <path
                d="M12 12a5 5 0 1 0-5-5 5 5 0 0 0 5 5Zm0 2c-4.42 0-8 2.24-8 5v1h16v-1c0-2.76-3.58-5-8-5Z"
                fill="currentColor"
              />
            </svg>
          </button>

          <button class="logout-btn" type="button" (click)="auth.logout()">
            Logout
          </button>
        </div>
      </header>

      <section class="hero">
        <h2>Welcome, {{ auth.fullName() }}</h2>
        <p>{{ auth.user()?.email }} | {{ auth.role() }}</p>
      </section>

      <section class="profile-tab" *ngIf="showProfile()">
        <app-current-user-card></app-current-user-card>
      </section>
    </div>
  `,
  styles: [
    `
      .dashboard-shell {
        min-height: 100vh;
        background: #0f172a;
        color: #f1f5f9;
        font-family: system-ui, sans-serif;
        padding: 20px;
      }

      .dashboard-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 24px;
      }

      .dashboard-header h1 {
        margin: 0;
        font-size: 20px;
      }

      .header-actions {
        display: flex;
        align-items: center;
        gap: 10px;
      }

      .icon-btn,
      .logout-btn {
        border: 1px solid #334155;
        background: #1e293b;
        color: #e2e8f0;
        border-radius: 8px;
        cursor: pointer;
      }

      .icon-btn {
        width: 38px;
        height: 38px;
        display: grid;
        place-items: center;
      }

      .icon-btn svg {
        width: 18px;
        height: 18px;
      }

      .logout-btn {
        padding: 8px 12px;
      }

      .hero h2 {
        margin: 0;
        font-size: 24px;
      }

      .hero p {
        margin-top: 8px;
        color: #94a3b8;
      }

      .profile-tab {
        margin-top: 18px;
      }
    `,
  ],
})
export class DashboardComponent {
  auth = inject(AuthService);

  showProfile = signal(false);

  toggleProfileTab(): void {
    this.showProfile.update((value) => !value);
  }
}
