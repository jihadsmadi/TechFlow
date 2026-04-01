import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import { SprintDetails, SprintSummary } from '../../shared/models/sprint.model';
import { TaskSummary } from '../../shared/models/task.model';

@Injectable({ providedIn: 'root' })
export class SprintsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/projects`;

  getSprints(projectId: string) {
    return this.http.get<SprintSummary[]>(`${this.base}/${projectId}/sprints`);
  }

  getActiveSprint(projectId: string) {
    return this.http.get<SprintDetails | null>(`${this.base}/${projectId}/sprints/active`);
  }

  getBacklog(projectId: string) {
    return this.http.get<TaskSummary[]>(`${this.base}/${projectId}/sprints/backlog`);
  }
}
