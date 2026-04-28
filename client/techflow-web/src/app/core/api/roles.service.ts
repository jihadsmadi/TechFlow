import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import { RoleSummary } from '../../shared/models/role.model';

@Injectable({ providedIn: 'root' })
export class RolesService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/roles`;

  getRoles() {
    return this.http.get<RoleSummary[]>(this.base);
  }
}
