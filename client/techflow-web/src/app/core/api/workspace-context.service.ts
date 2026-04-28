import { computed, Injectable, signal } from '@angular/core';

import { ProjectSummary } from '../../shared/models/project.model';
import { StorageService } from '../storage/storage.service';

@Injectable({ providedIn: 'root' })
export class WorkspaceContextService {
  private readonly selectedProjectStorageKey = 'tf_selected_project_id';

  readonly projects = signal<ProjectSummary[]>([]);
  readonly selectedProjectId = signal('');
  /** Last-focused board column per project (session). */
  readonly activeBoardListId = signal('');

  readonly selectedProject = computed(() =>
    this.projects().find((project) => project.id === this.selectedProjectId()) ?? null
  );

  constructor(private readonly storage: StorageService) {
    const savedId = this.storage.get(this.selectedProjectStorageKey);
    if (savedId) {
      this.selectedProjectId.set(savedId);
    }
  }

  setProjects(projects: ProjectSummary[]): void {
    this.projects.set(projects);

    if (!projects.length) {
      this.setSelectedProject('');
      return;
    }

    const currentId = this.selectedProjectId();
    const hasCurrentSelection = projects.some((project) => project.id === currentId);

    if (!hasCurrentSelection) {
      this.setSelectedProject(projects[0].id);
    }
  }

  setSelectedProject(projectId: string): void {
    this.selectedProjectId.set(projectId);
    this.refreshActiveBoardListFromSession();

    if (projectId) {
      this.storage.set(this.selectedProjectStorageKey, projectId);
      return;
    }

    this.storage.remove(this.selectedProjectStorageKey);
    this.activeBoardListId.set('');
  }

  setActiveBoardList(projectId: string, listId: string): void {
    if (!projectId || !listId) return;
    try {
      sessionStorage.setItem(this.boardListSessionKey(projectId), listId);
    } catch {
      /* ignore */
    }
    if (this.selectedProjectId() === projectId) {
      this.activeBoardListId.set(listId);
    }
  }

  refreshActiveBoardListFromSession(): void {
    const projectId = this.selectedProjectId();
    if (!projectId) {
      this.activeBoardListId.set('');
      return;
    }
    let listId = '';
    try {
      listId = sessionStorage.getItem(this.boardListSessionKey(projectId)) ?? '';
    } catch {
      listId = '';
    }
    this.activeBoardListId.set(listId);
  }

  private boardListSessionKey(projectId: string): string {
    return `tf_board_list_${projectId}`;
  }
}
