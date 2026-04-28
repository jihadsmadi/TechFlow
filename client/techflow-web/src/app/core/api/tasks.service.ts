import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import { MyTask, SubtaskItem, TaskDetails, TaskSummary } from '../../shared/models/task.model';

@Injectable({ providedIn: 'root' })
export class TasksService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api`;

  getTasksByList(projectId: string, listId: string) {
    return this.http.get<TaskSummary[]>(`${this.base}/projects/${projectId}/tasks/by-list/${listId}`);
  }

  getTaskById(projectId: string, taskId: string) {
    return this.http.get<TaskDetails>(`${this.base}/projects/${projectId}/tasks/${taskId}`);
  }

  getMyTasks(includeCompleted = false) {
    return this.http.get<MyTask[]>(`${this.base}/users/me/tasks?includeCompleted=${includeCompleted}`);
  }

  createTask(
    projectId: string,
    payload: {
      listId: string;
      title: string;
      description?: string | null;
      priority?: string | null;
      type?: string | null;
      dueDate?: string | null;
      estimatedMinutes?: number | null;
    },
  ) {
    return this.http.post<TaskSummary>(`${this.base}/projects/${projectId}/tasks`, {
      projectId,
      ...payload,
    });
  }

  moveTask(
    projectId: string,
    taskId: string,
    payload: {
      newListId: string;
      prevDisplayOrder: number | null;
      nextDisplayOrder: number | null;
    },
  ) {
    return this.http.patch<void>(`${this.base}/projects/${projectId}/tasks/${taskId}/move`, {
      projectId,
      taskId,
      ...payload,
    });
  }

  updateTask(
    projectId: string,
    taskId: string,
    payload: {
      title: string;
      description?: string | null;
      priority?: string | null;
      type?: string | null;
      dueDate?: string | null;
      estimatedMinutes?: number | null;
    },
  ) {
    return this.http.put<void>(`${this.base}/projects/${projectId}/tasks/${taskId}`, {
      projectId,
      taskId,
      ...payload,
    });
  }

  addSubtask(projectId: string, taskId: string, payload: { title: string }) {
    return this.http.post<SubtaskItem>(`${this.base}/projects/${projectId}/tasks/${taskId}/subtasks`, {
      projectId,
      taskId,
      ...payload,
    });
  }

  completeSubtask(projectId: string, taskId: string, subtaskId: string) {
    return this.http.patch<void>(
      `${this.base}/projects/${projectId}/tasks/${taskId}/subtasks/${subtaskId}/complete`,
      {},
    );
  }

  reopenSubtask(projectId: string, taskId: string, subtaskId: string) {
    return this.http.patch<void>(
      `${this.base}/projects/${projectId}/tasks/${taskId}/subtasks/${subtaskId}/reopen`,
      {},
    );
  }

  removeSubtask(projectId: string, taskId: string, subtaskId: string) {
    return this.http.delete<void>(`${this.base}/projects/${projectId}/tasks/${taskId}/subtasks/${subtaskId}`);
  }

  assignTask(projectId: string, taskId: string, userId: string) {
    return this.http.post<void>(
      `${this.base}/projects/${projectId}/tasks/${taskId}/assignments/${userId}`,
      {},
    );
  }

  unassignTask(projectId: string, taskId: string, userId: string) {
    return this.http.delete<void>(`${this.base}/projects/${projectId}/tasks/${taskId}/assignments/${userId}`);
  }
}
