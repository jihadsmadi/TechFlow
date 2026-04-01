import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import { MyTask, TaskSummary } from '../../shared/models/task.model';

@Injectable({ providedIn: 'root' })
export class TasksService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api`;

  getTasksByList(projectId: string, listId: string) {
    return this.http.get<TaskSummary[]>(`${this.base}/projects/${projectId}/tasks/by-list/${listId}`);
  }

  getMyTasks(includeCompleted = false) {
    return this.http.get<MyTask[]>(`${this.base}/users/me/tasks?includeCompleted=${includeCompleted}`);
  }
}
