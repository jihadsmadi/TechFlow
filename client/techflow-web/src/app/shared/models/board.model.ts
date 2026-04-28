import { TaskSummary } from './task.model';

export interface BoardList {
  id: string;
  boardId: string;
  name: string;
  color: string | null;
  displayOrder: number;
  isDefault: boolean;
  isCompletedList: boolean;
  createdAt: string;
  updatedAt: string;
  tasks?: TaskSummary[];
}

export interface BoardDto {
  id: string;
  projectId: string;
  name: string;
  lists: BoardList[];
  createdAt: string;
  updatedAt: string;
}
