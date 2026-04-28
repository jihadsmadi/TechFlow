import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { ProjectsService } from '../../core/api/projects.service';
import { TasksService } from '../../core/api/tasks.service';
import { AuthService } from '../../core/auth/auth.service';
import { CurrentUserCardComponent } from '../../shared/components/current-user-card/current-user-card.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, CurrentUserCardComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit {
  auth = inject(AuthService);

  private readonly projectsService = inject(ProjectsService);
  private readonly tasksService = inject(TasksService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showProfile = signal(false);

  readonly kpis = signal([
    { label: 'Open Issues', value: '0', note: 'Across assigned tasks' },
    { label: 'In Progress', value: '0', note: 'Tasks with active subtasks' },
    { label: 'Resolved', value: '0', note: 'Completed tasks' },
    { label: 'Projects', value: '0', note: 'Active projects in workspace' },
  ]);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      projects: this.projectsService.getProjects(false).pipe(catchError(() => of([]))),
      tasks: this.tasksService.getMyTasks(true).pipe(catchError(() => of([]))),
    }).subscribe({
      next: ({ projects, tasks }) => {
        const openIssues = tasks.filter((task) => !task.isCompleted).length;
        const inProgress = tasks.filter((task) => !task.isCompleted && task.subtasksCompleted > 0).length;
        const resolved = tasks.filter((task) => task.isCompleted).length;

        this.kpis.set([
          { label: 'Open Issues', value: String(openIssues), note: 'Across assigned tasks' },
          { label: 'In Progress', value: String(inProgress), note: 'Tasks with active subtasks' },
          { label: 'Resolved', value: String(resolved), note: 'Completed tasks' },
          { label: 'Projects', value: String(projects.length), note: 'Active projects in workspace' },
        ]);
      },
      error: () => this.error.set('Failed to load dashboard metrics.'),
      complete: () => this.loading.set(false),
    });
  }

  toggleProfileTab(): void {
    this.showProfile.update((value) => !value);
  }
}
