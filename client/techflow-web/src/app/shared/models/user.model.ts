export interface UserPreferences {
  theme: string;
  boardView: string;
  language: string;
  notifyOnTaskAssigned: boolean;
  notifyOnCommentAdded: boolean;
  notifyOnDueDateNear: boolean;
  notifyOnTaskMoved: boolean;
  notifyOnMentioned: boolean;
  dueDateReminderDays: number;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  companyId: string;
  avatarUrl: string | null;
  isActive: boolean;
  role: string;
  preferences?: UserPreferences;
}
