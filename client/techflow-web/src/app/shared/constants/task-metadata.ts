/**
 * Task Priority Metadata - colors, icons, and display info
 */
export const PRIORITY_METADATA: Record<string, { label: string; color: string; bgColor: string; icon: string }> = {
  Low: {
    label: 'Low',
    color: '#0f766e',
    bgColor: '#ccfbf1',
    icon: 'arrow_downward'
  },
  Medium: {
    label: 'Medium',
    color: '#a16207',
    bgColor: '#fef3c7',
    icon: 'equal'
  },
  High: {
    label: 'High',
    color: '#dc2626',
    bgColor: '#fee2e2',
    icon: 'arrow_upward'
  },
  Critical: {
    label: 'Critical',
    color: '#991b1b',
    bgColor: '#fecaca',
    icon: 'priority_high'
  }
};

/**
 * Task Type Metadata - colors, icons, and display info
 */
export const TYPE_METADATA: Record<string, { label: string; color: string; bgColor: string; icon: string }> = {
  Task: {
    label: 'Task',
    color: '#1d8f8c',
    bgColor: '#dbeafe',
    icon: 'task_alt'
  },
  Bug: {
    label: 'Bug',
    color: '#be123c',
    bgColor: '#ffe4e6',
    icon: 'bug_report'
  },
  Story: {
    label: 'Story',
    color: '#7c3aed',
    bgColor: '#f3e8ff',
    icon: 'book'
  },
  Research: {
    label: 'Research',
    color: '#0891b2',
    bgColor: '#cffafe',
    icon: 'search'
  }
};

/**
 * Helper to get priority metadata
 */
export function getPriorityMetadata(priority: string) {
  return PRIORITY_METADATA[priority] || PRIORITY_METADATA['Medium'];
}

/**
 * Helper to get type metadata
 */
export function getTypeMetadata(type: string) {
  return TYPE_METADATA[type] || TYPE_METADATA['Task'];
}
