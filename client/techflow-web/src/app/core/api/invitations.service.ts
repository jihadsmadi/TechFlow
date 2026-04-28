import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../../../environments/environment';
import { Invitation, InviteUserRequest } from '../../shared/models/invitation.model';

@Injectable({ providedIn: 'root' })
export class InvitationsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/invitations`;

  getPending() {
    return this.http.get<Invitation[]>(this.base);
  }

  invite(payload: InviteUserRequest) {
    return this.http.post<Invitation>(this.base, payload);
  }

  revoke(invitationId: string) {
    return this.http.delete<void>(`${this.base}/${invitationId}`);
  }
}
