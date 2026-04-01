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
}
