import { CommonModule } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { ProjectsService } from '../../core/api/projects.service';
import { SprintsService } from '../../core/api/sprints.service';
import { TasksService } from '../../core/api/tasks.service';
import { WorkspaceContextService } from '../../core/api/workspace-context.service';

@Component({
  selector: 'app-workspace-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './workspace-home.component.html',
  styleUrl: './workspace-home.component.css',
})
export class WorkspaceHomeComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly projectsService = inject(ProjectsService);
  private readonly tasksService = inject(TasksService);
  private readonly sprintsService = inject(SprintsService);

  readonly context = inject(WorkspaceContextService);

  readonly showOnboarding = signal(false);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly selectedProjectBacklog = signal(0);
  readonly selectedProjectActiveSprint = signal('No active sprint');

  readonly statusCards = signal([
    { label: 'Open Issues', value: '0', trend: 'Across all assigned tasks' },
    { label: 'In Progress', value: '0', trend: 'Tasks in active delivery lists' },
    { label: 'Done This Sprint', value: '0', trend: 'Completed tasks from API' },
  ]);

  readonly selectedProjectName = computed(
    () => this.context.selectedProject()?.name ?? 'No project selected',
  );

  constructor() {
    this.route.queryParamMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((params) => {
        this.showOnboarding.set(params.get('onboarding') === '1');
      });

    this.loadOverview();
  }

  dismissOnboarding(): void {
    this.showOnboarding.set(false);
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { onboarding: null },
      queryParamsHandling: 'merge',
      replaceUrl: true,
    });
  }

  loadOverview(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      projects: this.projectsService.getProjects(false).pipe(catchError(() => of([]))),
      tasks: this.tasksService.getMyTasks(true).pipe(catchError(() => of([]))),
    })
      .pipe(
        switchMap(({ projects, tasks }) => {
          this.context.setProjects(projects ?? []);

          const openIssues = tasks.filter((task) => !task.isCompleted).length;
          const inProgress = tasks.filter((task) => !task.isCompleted && task.subtasksCompleted > 0).length;
          const done = tasks.filter((task) => task.isCompleted).length;

          this.statusCards.set([
            {
              label: 'Open Issues',
              value: String(openIssues),
              trend: `${projects.length} active projects`,
            },
            {
              label: 'In Progress',
              value: String(inProgress),
              trend: inProgress ? 'Tasks are actively moving' : 'No active in-progress tasks',
            },
            {
              label: 'Done This Sprint',
              value: String(done),
              trend: done ? 'Completed tasks from API' : 'No completed tasks yet',
            },
          ]);

          const selectedProjectId = this.context.selectedProjectId();
          if (!selectedProjectId) {
            return of({ activeName: 'No active sprint', backlogCount: 0 });
          }

          return forkJoin({
            activeSprint: this.sprintsService.getActiveSprint(selectedProjectId).pipe(catchError(() => of(null))),
            backlog: this.sprintsService.getBacklog(selectedProjectId).pipe(catchError(() => of([]))),
          }).pipe(
            map(({ activeSprint, backlog }) => ({
              activeName: activeSprint?.displayName ?? 'No active sprint',
              backlogCount: backlog.length,
            })),
          );
        }),
      )
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ({ activeName, backlogCount }) => {
          this.selectedProjectActiveSprint.set(activeName);
          this.selectedProjectBacklog.set(backlogCount);
        },
        error: () => this.error.set('Failed to load workspace overview.'),
        complete: () => this.loading.set(false),
      });
  }
}
