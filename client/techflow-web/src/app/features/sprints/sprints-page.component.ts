import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { ProjectsService } from '../../core/api/projects.service';
import { SprintsService } from '../../core/api/sprints.service';
import { WorkspaceContextService } from '../../core/api/workspace-context.service';
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

  readonly context = inject(WorkspaceContextService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly activeSprint = signal<SprintDetails | null>(null);
  readonly sprints = signal<SprintSummary[]>([]);
  readonly backlog = signal<TaskSummary[]>([]);

  readonly activeSprintProgress = computed(() => {
    const sprint = this.activeSprint();
    if (!sprint || sprint.totalTasks === 0) return 0;
    return Math.round((sprint.completedTasks / sprint.totalTasks) * 100);
  });

  ngOnInit(): void {
    this.loadProjects();
  }

  onProjectChange(event: Event): void {
    const projectId = (event.target as HTMLSelectElement).value;
    this.context.setSelectedProject(projectId);
    this.loadSprintData(projectId);
  }

  refresh(): void {
    const projectId = this.context.selectedProjectId();
    if (projectId) {
      this.loadSprintData(projectId);
    }
  }

  private loadProjects(): void {
    this.loading.set(true);
    this.error.set(null);

    this.projectsService.getProjects(false).subscribe({
      next: (projects) => {
        this.context.setProjects(projects ?? []);
        const selectedProjectId = this.context.selectedProjectId();

        if (selectedProjectId) {
          this.loadSprintData(selectedProjectId);
          return;
        }

        this.activeSprint.set(null);
        this.sprints.set([]);
        this.backlog.set([]);
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
