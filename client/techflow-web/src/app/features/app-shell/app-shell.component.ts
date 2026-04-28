import { CommonModule } from '@angular/common';
import { Component, DestroyRef, HostListener, inject, signal } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs';

import { AuthService } from '../../core/auth/auth.service';

interface NavItem {
  readonly id: string;
  readonly label: string;
  readonly icon: string;
  readonly route: string;
  readonly exact?: boolean;
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.css',
})
export class AppShellComponent {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly sidebarItems: readonly NavItem[] = [
    { id: 'workspace', label: 'Workspace', icon: 'dashboard', route: '/app/workspace' },
    { id: 'projects', label: 'Projects', icon: 'folder_open', route: '/app/projects', exact: false },
    { id: 'tasks', label: 'My Tasks', icon: 'check_circle', route: '/app/my-tasks' },
    { id: 'boards', label: 'Boards', icon: 'view_kanban', route: '/app/board' },
    { id: 'sprints', label: 'Sprints', icon: 'sprint', route: '/app/sprints' },
    { id: 'team', label: 'Team', icon: 'group', route: '/app/team' },
  ];

  readonly topTabs: readonly string[] = ['Your Work', 'Recent', 'Starred'];

  readonly activeTab = signal('Your Work');
  readonly mobileSidebarOpen = signal(false);

  constructor() {
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => this.mobileSidebarOpen.set(false));
  }

  onNavigate(): void {
    this.mobileSidebarOpen.set(false);
  }

  setTopTab(tab: string): void {
    this.activeTab.set(tab);
  }

  toggleMobileSidebar(): void {
    this.mobileSidebarOpen.update((value) => !value);
  }

  closeMobileSidebar(): void {
    this.mobileSidebarOpen.set(false);
  }

  @HostListener('document:keydown.escape')
  handleEscapeKey(): void {
    if (this.mobileSidebarOpen()) {
      this.mobileSidebarOpen.set(false);
    }
  }
}
