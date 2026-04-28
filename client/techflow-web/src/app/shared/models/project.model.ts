export interface ProjectSummary {
  id: string;
  name: string;
  description: string | null;
  status: string;
  color: string;
  isArchived: boolean;
  memberCount: number;
  startDate: string | null;
  endDate: string | null;
}

export interface ProjectSettings {
  defaultListNames: string[];
  defaultTaskType: string;
  defaultPriority: string;
  autoAssignCreator: boolean;
  requireEstimate: boolean;
  allowSubtasks: boolean;
  sprintLockOnStart: boolean;
  sprintDurationDays: number;
  incompleteTasksAction: string;
}

export interface ProjectDetails {
  id: string;
  companyId: string;
  createdByUserId: string;
  name: string;
  description: string | null;
  status: string;
  color: string;
  startDate: string | null;
  endDate: string | null;
  isArchived: boolean;
  archivedAt: string | null;
  memberCount: number;
  settings: ProjectSettings;
  createdAt: string;
  updatedAt: string;
}

export interface ProjectMember {
  id: string;
  projectId: string;
  userId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  avatarUrl: string | null;
  addedByUserId: string;
  addedAt: string;
}

export interface CreateProjectRequest {
  name: string;
  description: string | null;
  color: string | null;
  startDate: string | null;
  endDate: string | null;
}

export type ProjectStatusFilter = 'active' | 'archived' | 'all';
