import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';

import { ProjectsService } from '../../core/api/projects.service';
import { ProjectMember, ProjectSummary } from '../../shared/models/project.model';

@Component({
  selector: 'app-team-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './team-page.component.html',
  styleUrl: './team-page.component.css',
})
export class TeamPageComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly projects = signal<ProjectSummary[]>([]);
  readonly selectedProjectId = signal<string>('');
  readonly members = signal<ProjectMember[]>([]);

  ngOnInit(): void {
    this.loadProjects();
  }

  onProjectChange(event: Event): void {
    const id = (event.target as HTMLSelectElement).value;
    this.selectedProjectId.set(id);
    if (id) this.loadMembers(id);
  }

  memberDisplay(member: ProjectMember): string {
    const fullName = member.fullName?.trim();
    if (fullName) return fullName;

    const firstLast = `${member.firstName ?? ''} ${member.lastName ?? ''}`.trim();
    if (firstLast) return firstLast;

    return member.email ?? member.userId ?? member.id ?? 'Unknown member';
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
          this.loadMembers(first);
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

  private loadMembers(projectId: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.projectsService.getProjectMembers(projectId).subscribe({
      next: (members) => this.members.set(Array.isArray(members) ? members : []),
      error: () => {
        this.error.set('Failed to load project members.');
        this.members.set([]);
      },
      complete: () => this.loading.set(false),
    });
  }
}
