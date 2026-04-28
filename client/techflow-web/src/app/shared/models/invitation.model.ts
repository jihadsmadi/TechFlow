export interface Invitation {
  id: string;
  companyId: string;
  invitedByUserId: string;
  roleId: string;
  email: string;
  projectId: string | null;
  type: number;
  expiresAt: string;
  isUsed: boolean;
  usedAt: string | null;
  isRevoked: boolean;
  createdAt: string;
}

export interface InviteUserRequest {
  roleId: string;
  email: string;
  projectId?: string;
}
