import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import { ProjectMember, ProjectSummary } from '../../shared/models/project.model';

@Injectable({ providedIn: 'root' })
export class ProjectsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/projects`;

  getProjects(includeArchived = false) {
    return this.http.get<ProjectSummary[]>(`${this.base}?includeArchived=${includeArchived}`);
  }

  getProjectMembers(projectId: string) {
    return this.http.get<ProjectMember[]>(`${this.base}/${projectId}/members`);
  }
}
