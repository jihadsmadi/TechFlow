import { TaskSummary } from './task.model';

export interface BoardList {
  id: string;
  name: string;
  color?: string;
  displayOrder?: number;
  isDefault?: boolean;
  isCompletedList?: boolean;
  tasks?: TaskSummary[];
}

export interface BoardDto {
  id: string;
  projectId: string;
  name: string;
  lists: BoardList[];
}
