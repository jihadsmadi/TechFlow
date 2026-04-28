import { TechflowDropdownOption } from '../components/techflow-dropdown/techflow-dropdown.component';
import { PRIORITY_METADATA, TYPE_METADATA } from './task-metadata';

export const TASK_PRIORITY_DROPDOWN_OPTIONS: TechflowDropdownOption[] = [
  { value: 'Low', label: 'Low' },
  { value: 'Medium', label: 'Medium' },
  { value: 'High', label: 'High' },
  { value: 'Critical', label: 'Critical' },
];

export const TASK_TYPE_DROPDOWN_OPTIONS: TechflowDropdownOption[] = [
  { value: 'Task', label: 'Task' },
  { value: 'Bug', label: 'Bug' },
  { value: 'Story', label: 'Story' },
  { value: 'Research', label: 'Research' },
];
