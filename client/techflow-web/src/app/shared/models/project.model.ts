export interface ProjectSummary {
  id: string;
  name: string;
  description?: string | null;
  status?: string;
  color?: string;
  startDate?: string | null;
  endDate?: string | null;
  isArchived?: boolean;
}

export interface ProjectMember {
  id?: string;
  userId?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
  role?: string;
}
