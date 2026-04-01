import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.css',
})
export class AppShellComponent {
  readonly auth = inject(AuthService);

  readonly links = [
    { path: '/app/projects', label: 'Projects' },
    { path: '/app/board', label: 'Board' },
    { path: '/app/sprints', label: 'Sprints' },
    { path: '/app/my-tasks', label: 'My Tasks' },
    { path: '/app/team', label: 'Team' },
  ];
}
