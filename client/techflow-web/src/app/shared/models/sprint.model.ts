import { TaskSummary } from './task.model';

export interface SprintSummary {
  id: string;
  sprintNumber?: number;
  name?: string | null;
  displayName?: string;
  status?: string;
  startDate?: string | null;
  endDate?: string | null;
  totalTasks?: number;
  completedTasks?: number;
}

export interface SprintDetails {
  id: string;
  sprintNumber?: number;
  name?: string | null;
  displayName?: string;
  status?: string;
  goal?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  tasks?: TaskSummary[];
}
