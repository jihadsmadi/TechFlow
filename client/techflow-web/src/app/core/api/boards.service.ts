import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import { BoardDto } from '../../shared/models/board.model';

@Injectable({ providedIn: 'root' })
export class BoardsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/projects`;

  getBoard(projectId: string) {
    return this.http.get<BoardDto>(`${this.base}/${projectId}/board`);
  }

  addList(projectId: string, payload: { name: string; color?: string | null }) {
    return this.http.post<void>(`${this.base}/${projectId}/board/lists`, payload);
  }

  renameList(projectId: string, listId: string, payload: { name: string }) {
    return this.http.patch<void>(`${this.base}/${projectId}/board/lists/${listId}/rename`, payload);
  }
}
