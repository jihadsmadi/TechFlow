import { TaskSummary } from './task.model';

export interface SprintSummary {
  id: string;
  projectId: string;
  sprintNumber: number;
  displayName: string;
  name: string | null;
  goal: string | null;
  status: string;
  startDate: string;
  endDate: string;
  actualEndDate: string | null;
  isLocked: boolean;
  totalTasks: number;
  completedTasks: number;
}

export interface SprintDetails extends SprintSummary {
  companyId: string;
  createdByUserId: string;
  tasks: TaskSummary[];
  createdAt: string;
  updatedAt: string;
}
