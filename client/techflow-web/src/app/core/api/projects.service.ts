import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import {
  CreateProjectRequest,
  ProjectDetails,
  ProjectMember,
  ProjectSummary,
} from '../../shared/models/project.model';

@Injectable({ providedIn: 'root' })
export class ProjectsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/projects`;

  getProjects(includeArchived = false) {
    return this.http.get<ProjectSummary[]>(`${this.base}?includeArchived=${includeArchived}`);
  }

  createProject(payload: CreateProjectRequest) {
    return this.http.post<ProjectSummary>(this.base, payload);
  }

  getProjectById(projectId: string) {
    return this.http.get<ProjectDetails>(`${this.base}/${projectId}`);
  }

  getProjectMembers(projectId: string) {
    return this.http.get<ProjectMember[]>(`${this.base}/${projectId}/members`);
  }

  archiveProject(projectId: string) {
    return this.http.patch<void>(`${this.base}/${projectId}/archive`, {});
  }

  restoreProject(projectId: string) {
    return this.http.patch<void>(`${this.base}/${projectId}/restore`, {});
  }
}
