export interface TaskSummary {
  id: string;
  listId: string;
  title: string;
  priority: string;
  type: string;
  displayOrder: number;
  dueDate: string | null;
  isCompleted: boolean;
  subtasksTotal: number;
  subtasksCompleted: number;
  assignedUserIds: string[];
}

export interface TaskAssignment {
  id: string;
  taskId: string;
  userId: string;
  userName: string;
  userEmail: string;
  userAvatarUrl: string | null;
  assignedByUserId: string;
  assignedAt: string;
}

export interface SubtaskItem {
  id: string;
  taskId: string;
  title: string;
  isCompleted: boolean;
  createdAt: string;
}

export interface TaskDetails extends TaskSummary {
  projectId: string;
  companyId: string;
  createdByUserId: string;
  description: string | null;
  estimatedMinutes: number | null;
  actualMinutes: number | null;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string;
  assignments: TaskAssignment[];
  subtasks: SubtaskItem[];
}

export interface MyTask extends TaskSummary {
  projectId: string;
  projectName?: string;
  listName?: string;
}
