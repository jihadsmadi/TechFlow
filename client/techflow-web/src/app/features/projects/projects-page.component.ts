import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ProjectsService } from '../../core/api/projects.service';
import { WorkspaceContextService } from '../../core/api/workspace-context.service';
import { TechflowDropdownComponent, TechflowDropdownOption } from '../../shared/components/techflow-dropdown/techflow-dropdown.component';
import { TechflowSpinnerComponent } from '../../shared/components/techflow-spinner/techflow-spinner.component';
import { getApiError } from '../../shared/utils/api-error';
import { ProjectStatusFilter, ProjectSummary } from '../../shared/models/project.model';

@Component({
  selector: 'app-projects-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TechflowDropdownComponent, TechflowSpinnerComponent],
  templateUrl: './projects-page.component.html',
  styleUrl: './projects-page.component.css',
})
export class ProjectsPageComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);
  private readonly context = inject(WorkspaceContextService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly projects = signal<ProjectSummary[]>([]);
  readonly searchTerm = signal('');
  readonly statusFilter = signal<ProjectStatusFilter>('active');

  readonly filteredProjects = computed(() => {
    const query = this.searchTerm().trim().toLowerCase();
    const status = this.statusFilter();

    return this.projects().filter((project) => {
      const matchesStatus = this.matchesStatusFilter(project, status);
      if (!matchesStatus) {
        return false;
      }

      if (!query) {
        return true;
      }

      const text = `${project.name} ${project.description ?? ''}`.toLowerCase();
      return text.includes(query);
    });
  });

  readonly activeCount = computed(() => this.projects().filter((project) => !project.isArchived).length);
  readonly archivedCount = computed(() => this.projects().filter((project) => project.isArchived).length);
  readonly showFilteredEmpty = computed(
    () => !!this.projects().length && !this.filteredProjects().length,
  );

  readonly statusDropdownOptions: TechflowDropdownOption[] = [
    { value: 'active', label: 'Active' },
    { value: 'archived', label: 'Archived' },
    { value: 'all', label: 'All' },
  ];

  ngOnInit(): void {
    this.load();
  }

  onSearchChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
  }

  onStatusPicked(value: string | null): void {
    const raw = value ?? 'active';
    if (!this.isStatusFilter(raw)) {
      return;
    }

    this.statusFilter.set(raw);
    this.load();
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.statusFilter.set('active');
    this.load();
  }

  openCreatePage(): void {
    this.router.navigate(['/app/projects/new']);
  }

  openProjectDetails(projectId: string): void {
    this.context.setSelectedProject(projectId);
    this.router.navigate(['/app/projects', projectId]);
  }

  refresh(): void {
    this.load();
  }

  statusLabel(project: ProjectSummary): string {
    return project.isArchived ? 'Archived' : 'Active';
  }

  statusClass(project: ProjectSummary): string {
    return project.isArchived ? 'badge-warning' : 'badge-success';
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.projectsService.getProjects(this.shouldIncludeArchived()).subscribe({
      next: (projects) => {
        const sorted = [...(projects ?? [])].sort((a, b) => {
          if (a.isArchived !== b.isArchived) {
            return Number(a.isArchived) - Number(b.isArchived);
          }

          return a.name.localeCompare(b.name);
        });

        this.projects.set(sorted);
      },
      error: (err) => {
        this.error.set(getApiError(err));
        this.loading.set(false);
      },
      complete: () => this.loading.set(false),
    });
  }

  private shouldIncludeArchived(): boolean {
    return this.statusFilter() !== 'active';
  }

  private matchesStatusFilter(project: ProjectSummary, filter: ProjectStatusFilter): boolean {
    if (filter === 'all') {
      return true;
    }

    if (filter === 'archived') {
      return project.isArchived;
    }

    return !project.isArchived;
  }

  private isStatusFilter(value: string): value is ProjectStatusFilter {
    return value === 'active' || value === 'archived' || value === 'all';
  }
}
