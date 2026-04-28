import { CommonModule } from '@angular/common';
import { TechflowSpinnerComponent } from '../../shared/components/techflow-spinner/techflow-spinner.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { ProjectsService } from '../../core/api/projects.service';
import { ToastService } from '../../core/notifications/toast.service';
import { WorkspaceContextService } from '../../core/api/workspace-context.service';
import { ProjectDetails, ProjectMember, ProjectSettings, ProjectSummary } from '../../shared/models/project.model';
import { getApiError } from '../../shared/utils/api-error';

@Component({
  selector: 'app-project-details-page',
  standalone: true,
  imports: [CommonModule, RouterLink, TechflowSpinnerComponent],
  templateUrl: './project-details-page.component.html',
  styleUrl: './project-details-page.component.css',
})
export class ProjectDetailsPageComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly context = inject(WorkspaceContextService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly toast = inject(ToastService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly actionLoading = signal(false);
  readonly project = signal<ProjectDetails | null>(null);
  readonly members = signal<ProjectMember[]>([]);

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const projectId = params.get('projectId');

      if (!projectId) {
        const msg = 'Project id is missing from the route.';
        this.error.set(msg);
        this.toast.error(msg);
        this.loading.set(false);
        return;
      }

      this.load(projectId);
    });
  }

  openBoard(): void {
    this.openProjectScopedRoute('/app/board');
  }

  openSprints(): void {
    this.openProjectScopedRoute('/app/sprints');
  }

  openTeam(): void {
    this.openProjectScopedRoute('/app/team');
  }

  toggleArchiveState(): void {
    const project = this.project();

    if (!project || this.actionLoading()) {
      return;
    }

    this.actionLoading.set(true);

    const request = project.isArchived
      ? this.projectsService.restoreProject(project.id)
      : this.projectsService.archiveProject(project.id);

    request.subscribe({
      next: () => {
        this.toast.success(project.isArchived ? 'Project restored.' : 'Project archived.');
        this.load(project.id);
      },
      error: (err) => {
        this.toast.error(getApiError(err));
        this.actionLoading.set(false);
      },
    });
  }

  yn(value: boolean): string {
    return value ? 'Yes' : 'No';
  }

  incompleteTasksLabel(action: string): string {
    const key = (action ?? '').trim();
    const map: Record<string, string> = {
      MoveToBacklog: 'Move incomplete tasks to backlog',
      MoveToNextSprint: 'Move incomplete tasks to next sprint',
      LeaveInPlace: 'Leave incomplete tasks in place',
    };
    return map[key] ?? key;
  }

  settingsRows(settings: ProjectSettings): { label: string; value: string }[] {
    return [
      { label: 'Default lists', value: settings.defaultListNames.join(', ') },
      { label: 'Default task type', value: settings.defaultTaskType },
      { label: 'Default priority', value: settings.defaultPriority },
      { label: 'Auto-assign creator', value: this.yn(settings.autoAssignCreator) },
      { label: 'Require estimate', value: this.yn(settings.requireEstimate) },
      { label: 'Allow subtasks', value: this.yn(settings.allowSubtasks) },
      { label: 'Sprint lock on start', value: this.yn(settings.sprintLockOnStart) },
      { label: 'Sprint duration', value: `${settings.sprintDurationDays} days` },
      { label: 'When sprint ends (incomplete tasks)', value: this.incompleteTasksLabel(settings.incompleteTasksAction) },
    ];
  }

  displayMemberName(member: ProjectMember): string {
    if (member.fullName?.trim()) {
      return member.fullName;
    }

    const fallbackName = `${member.firstName ?? ''} ${member.lastName ?? ''}`.trim();
    return fallbackName || member.email || member.userId;
  }

  private load(projectId: string): void {
    this.loading.set(true);
    this.error.set(null);
    forkJoin({
      project: this.projectsService.getProjectById(projectId),
      members: this.projectsService.getProjectMembers(projectId).pipe(
        catchError((err) => {
          const msg = getApiError(err);
          this.toast.error(msg, 'Members');
          return of([] as ProjectMember[]);
        }),
      ),
    }).subscribe({
      next: ({ project, members }) => {
        this.project.set(project);
        this.members.set(members);
        this.syncProjectContext(project);
      },
      error: (err) => {
        const msg = getApiError(err);
        this.error.set(msg);
        this.toast.error(msg, 'Project');
        this.project.set(null);
        this.members.set([]);
        this.loading.set(false);
        this.actionLoading.set(false);
      },
      complete: () => {
        this.loading.set(false);
        this.actionLoading.set(false);
      },
    });
  }

  private openProjectScopedRoute(route: string): void {
    const project = this.project();
    if (!project) {
      return;
    }

    this.context.setSelectedProject(project.id);
    this.router.navigate([route]);
  }

  private syncProjectContext(project: ProjectDetails): void {
    const existing = this.context.projects();
    const asSummary = this.mapProjectSummary(project);

    if (project.isArchived) {
      const activeProjects = existing.filter((item) => item.id !== project.id && !item.isArchived);
      this.context.setProjects(activeProjects);
      return;
    }

    const hasExisting = existing.some((item) => item.id === project.id);
    const next = hasExisting
      ? existing.map((item) => (item.id === project.id ? asSummary : item))
      : [asSummary, ...existing];

    this.context.setProjects(next);
    this.context.setSelectedProject(project.id);
  }

  private mapProjectSummary(project: ProjectDetails): ProjectSummary {
    return {
      id: project.id,
      name: project.name,
      description: project.description,
      status: project.status,
      color: project.color,
      isArchived: project.isArchived,
      memberCount: project.memberCount,
      startDate: project.startDate,
      endDate: project.endDate,
    };
  }
}
