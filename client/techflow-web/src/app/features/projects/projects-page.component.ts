import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';

import { ProjectsService } from '../../core/api/projects.service';
import { ProjectSummary } from '../../shared/models/project.model';

@Component({
  selector: 'app-projects-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './projects-page.component.html',
  styleUrl: './projects-page.component.css',
})
export class ProjectsPageComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly projects = signal<ProjectSummary[]>([]);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.projectsService.getProjects(false).subscribe({
      next: (projects) => this.projects.set(projects ?? []),
      error: () => this.error.set('Failed to load projects.'),
      complete: () => this.loading.set(false),
    });
  }
}
