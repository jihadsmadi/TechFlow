import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { ProjectsService } from '../../core/api/projects.service';
import { SprintsService } from '../../core/api/sprints.service';
import { ProjectSummary } from '../../shared/models/project.model';
import { SprintDetails, SprintSummary } from '../../shared/models/sprint.model';
import { TaskSummary } from '../../shared/models/task.model';

@Component({
  selector: 'app-sprints-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sprints-page.component.html',
  styleUrl: './sprints-page.component.css',
})
export class SprintsPageComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);
  private readonly sprintsService = inject(SprintsService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly projects = signal<ProjectSummary[]>([]);
  readonly selectedProjectId = signal<string>('');

  readonly activeSprint = signal<SprintDetails | null>(null);
  readonly sprints = signal<SprintSummary[]>([]);
  readonly backlog = signal<TaskSummary[]>([]);

  ngOnInit(): void {
    this.loadProjects();
  }

  onProjectChange(event: Event): void {
    const id = (event.target as HTMLSelectElement).value;
    this.selectedProjectId.set(id);
    if (id) this.loadSprintData(id);
  }

  private loadProjects(): void {
    this.loading.set(true);
    this.error.set(null);

    this.projectsService.getProjects(false).subscribe({
      next: (projects) => {
        this.projects.set(projects ?? []);
        const first = projects?.[0]?.id ?? '';
        this.selectedProjectId.set(first);
        if (first) {
          this.loadSprintData(first);
          return;
        }
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load projects.');
        this.loading.set(false);
      },
    });
  }

  private loadSprintData(projectId: string): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      sprints: this.sprintsService.getSprints(projectId).pipe(catchError(() => of([]))),
      active: this.sprintsService.getActiveSprint(projectId).pipe(catchError(() => of(null))),
      backlog: this.sprintsService.getBacklog(projectId).pipe(catchError(() => of([]))),
    }).subscribe({
      next: ({ sprints, active, backlog }) => {
        this.sprints.set(sprints);
        this.activeSprint.set(active);
        this.backlog.set(backlog);
      },
      error: () => this.error.set('Failed to load sprint data.'),
      complete: () => this.loading.set(false),
    });
  }
}
