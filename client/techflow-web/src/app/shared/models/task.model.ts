export interface TaskSummary {
  id: string;
  title: string;
  description?: string | null;
  priority?: string;
  type?: string;
  dueDate?: string | null;
  isCompleted?: boolean;
}

export interface MyTask extends TaskSummary {
  projectId?: string;
  projectName?: string;
  listId?: string;
  listName?: string;
}
