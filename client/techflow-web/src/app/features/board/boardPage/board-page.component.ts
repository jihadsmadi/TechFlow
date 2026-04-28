import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';

import { BoardsService } from '../../../core/api/boards.service';
import { ProjectsService } from '../../../core/api/projects.service';
import { TasksService } from '../../../core/api/tasks.service';
import { WorkspaceContextService } from '../../../core/api/workspace-context.service';
import { ToastService } from '../../../core/notifications/toast.service';
import { BoardDto, BoardList } from '../../../shared/models/board.model';
import { ProjectMember } from '../../../shared/models/project.model';
import { TaskDetails, TaskSummary } from '../../../shared/models/task.model';
import { TechflowDatePickerComponent } from '../../../shared/components/techflow-date-picker/techflow-date-picker.component';
import { TechflowSpinnerComponent } from '../../../shared/components/techflow-spinner/techflow-spinner.component';
import { getApiError } from '../../../shared/utils/api-error';
import { getPriorityMetadata, getTypeMetadata } from '../../../shared/constants/task-metadata';

import { TechflowDropdownComponent } from '../../../shared/components/techflow-dropdown/techflow-dropdown.component';
import {
  TASK_PRIORITY_DROPDOWN_OPTIONS,
  TASK_TYPE_DROPDOWN_OPTIONS,
} from '../../../shared/constants/task-field-options';

import { BoardAddListDialogComponent } from '../board-add-list-dialog.component';
import { BoardTaskCreateDialogComponent, BoardTaskCreatePayload } from '../board-task-create-dialog.component';

@Component({
  selector: 'app-board-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    BoardTaskCreateDialogComponent,
    BoardAddListDialogComponent,
    TechflowSpinnerComponent,
    TechflowDatePickerComponent,
    TechflowDropdownComponent,
  ],
  templateUrl: './board-page.component.html',
  styleUrl: './board-page.component.css',
})
export class BoardPageComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);
  private readonly boardsService = inject(BoardsService);
  private readonly tasksService = inject(TasksService);
  private readonly toast = inject(ToastService);

  readonly context = inject(WorkspaceContextService);

  @ViewChild('createDlg') private createDialog?: BoardTaskCreateDialogComponent;
  @ViewChild('addListDlg') private addListDialog?: BoardAddListDialogComponent;

  readonly priorityDropdownOptions = TASK_PRIORITY_DROPDOWN_OPTIONS;
  readonly typeDropdownOptions = TASK_TYPE_DROPDOWN_OPTIONS;

  readonly projectPickerOptions = computed(() =>
    this.context.projects().map((p) => ({ value: p.id, label: p.name })),
  );

  readonly loading = signal(false);
  readonly silentRefreshing = signal(false);
  readonly error = signal<string | null>(null);
  readonly boardName = signal('');
  readonly lists = signal<BoardList[]>([]);
  readonly totalTasks = signal(0);
  readonly movingTaskId = signal<string | null>(null);
  readonly renamingListId = signal<string | null>(null);
  readonly dropListId = signal<string | null>(null);
  readonly taskDrawerOpen = signal(false);
  readonly taskDrawerLoading = signal(false);
  readonly taskDrawerError = signal<string | null>(null);
  readonly selectedTask = signal<TaskDetails | null>(null);
  readonly projectMembers = signal<ProjectMember[]>([]);
  readonly savingTaskDetails = signal(false);
  readonly addingSubtask = signal(false);

  renameListName = '';
  taskEditTitle = '';
  taskEditDescription = '';
  taskEditPriority = 'Medium';
  taskEditType = 'Task';
  taskEditDueDate = '';
  newSubtaskTitle = '';

  private draggingTask: TaskSummary | null = null;
  private draggingFromListId: string | null = null;

  ngOnInit(): void {
    this.loadProjects();
  }

  onProjectPicked(projectId: string): void {
    this.context.setSelectedProject(projectId);
    if (projectId) {
      this.loadBoard(projectId, { silent: false });
    } else {
      this.boardName.set('');
      this.lists.set([]);
    }
  }

  refresh(): void {
    const projectId = this.context.selectedProjectId();
    if (projectId) {
      this.loadBoard(projectId, { silent: true });
    }
  }

  startAddList(): void {
    this.addListDialog?.open();
  }

  onAddListSubmit(payload: { name: string }): void {
    this.createList(payload.name);
  }

  createList(nameRaw: string): void {
    const projectId = this.context.selectedProjectId();
    const name = nameRaw.trim();
    if (!projectId || !name) return;

    const snapshot = this.cloneLists(this.lists());

    this.boardsService.addList(projectId, { name }).subscribe({
      next: () => {
        this.boardsService.getBoard(projectId).subscribe({
          next: (board) => {
            this.mergeBoardStructure(board, snapshot);
            this.toast.success('List created.');
          },
          error: () => {
            this.lists.set(snapshot);
            this.toast.error('List was created but the board could not be refreshed.');
          },
        });
      },
      error: (err) => this.toast.error(getApiError(err)),
    });
  }

  beginRenameList(list: BoardList): void {
    this.renamingListId.set(list.id);
    this.renameListName = list.name;
  }

  cancelRenameList(): void {
    this.renamingListId.set(null);
    this.renameListName = '';
  }

  submitRenameList(list: BoardList): void {
    const projectId = this.context.selectedProjectId();
    const name = this.renameListName.trim();
    if (!projectId || !name) return;

    const snapshot = this.cloneLists(this.lists());
    this.lists.update((rows) =>
      rows.map((row) => (row.id === list.id ? { ...row, name } : row)),
    );

    this.boardsService.renameList(projectId, list.id, { name }).subscribe({
      next: () => {
        this.cancelRenameList();
        this.toast.success('List renamed.');
      },
      error: (err) => {
        this.lists.set(snapshot);
        this.toast.error(getApiError(err));
      },
    });
  }

  startCreateTask(listId: string): void {
    const projectId = this.context.selectedProjectId();
    if (projectId) {
      this.context.setActiveBoardList(projectId, listId);
    }
    this.createDialog?.open(listId);
  }

  onTaskCreateSubmit(payload: BoardTaskCreatePayload): void {
    const projectId = this.context.selectedProjectId();
    if (!projectId) return;

    this.tasksService
      .createTask(projectId, {
        listId: payload.listId,
        title: payload.title,
        priority: payload.priority,
        type: payload.type,
        dueDate: payload.dueDate ? `${payload.dueDate}T00:00:00.000Z` : null,
      })
      .subscribe({
        next: (created) => {
          this.context.setActiveBoardList(projectId, payload.listId);
          this.lists.update((rows) =>
            rows.map((row) =>
              row.id === payload.listId ? { ...row, tasks: [...(row.tasks ?? []), created] } : row,
            ),
          );
          this.recountTasks();
          this.toast.success('Task created.');
        },
        error: (err) => this.toast.error(getApiError(err)),
      });
  }

  openTaskDrawer(task: TaskSummary, listId: string): void {
    const projectId = this.context.selectedProjectId();
    if (!projectId) return;

    this.context.setActiveBoardList(projectId, listId);
    this.taskDrawerOpen.set(true);
    this.taskDrawerLoading.set(true);
    this.taskDrawerError.set(null);

    forkJoin({
      task: this.tasksService.getTaskById(projectId, task.id),
      members: this.projectsService.getProjectMembers(projectId).pipe(catchError(() => of([]))),
    }).subscribe({
      next: ({ task: taskDetails, members }) => {
        this.selectedTask.set(taskDetails);
        this.projectMembers.set(members ?? []);
        this.taskEditTitle = taskDetails.title;
        this.taskEditDescription = taskDetails.description ?? '';
        this.taskEditPriority = taskDetails.priority || 'Medium';
        this.taskEditType = taskDetails.type || 'Task';
        this.taskEditDueDate = taskDetails.dueDate ? taskDetails.dueDate.slice(0, 10) : '';
      },
      error: (err) => {
        const msg = getApiError(err);
        this.taskDrawerError.set(msg);
        this.toast.error(msg, 'Task');
      },
      complete: () => this.taskDrawerLoading.set(false),
    });
  }

  closeTaskDrawer(): void {
    this.taskDrawerOpen.set(false);
    this.selectedTask.set(null);
    this.projectMembers.set([]);
    this.taskDrawerError.set(null);
    this.newSubtaskTitle = '';
  }

  saveTaskDetails(): void {
    const projectId = this.context.selectedProjectId();
    const task = this.selectedTask();
    if (!projectId || !task || !this.taskEditTitle.trim()) return;

    this.taskDrawerError.set(null);
    this.savingTaskDetails.set(true);

    this.tasksService
      .updateTask(projectId, task.id, {
        title: this.taskEditTitle.trim(),
        description: this.taskEditDescription.trim() || null,
        priority: this.taskEditPriority,
        type: this.taskEditType,
        dueDate: this.taskEditDueDate ? `${this.taskEditDueDate}T00:00:00.000Z` : null,
      })
      .subscribe({
        next: () => {
          this.patchTaskOnBoard(task.id, {
            title: this.taskEditTitle.trim(),
            priority: this.taskEditPriority,
            type: this.taskEditType,
            dueDate: this.taskEditDueDate ? `${this.taskEditDueDate}T00:00:00.000Z` : null,
          });
          this.reloadTaskDetails(task.id);
          this.toast.success('Task updated.');
        },
        error: (err) => {
          const msg = getApiError(err);
          this.taskDrawerError.set(msg);
          this.toast.error(msg);
        },
        complete: () => this.savingTaskDetails.set(false),
      });
  }

  addSubtask(): void {
    const projectId = this.context.selectedProjectId();
    const task = this.selectedTask();
    const title = this.newSubtaskTitle.trim();
    if (!projectId || !task || !title) return;

    this.addingSubtask.set(true);
    this.tasksService.addSubtask(projectId, task.id, { title }).subscribe({
      next: () => {
        this.newSubtaskTitle = '';
        this.reloadTaskDetails(task.id);
        this.toast.success('Subtask added.');
      },
      error: (err) => {
        const msg = getApiError(err);
        this.taskDrawerError.set(msg);
        this.toast.error(msg);
      },
      complete: () => this.addingSubtask.set(false),
    });
  }

  toggleSubtask(subtaskId: string, isCompleted: boolean): void {
    const projectId = this.context.selectedProjectId();
    const task = this.selectedTask();
    if (!projectId || !task) return;

    const request = isCompleted
      ? this.tasksService.reopenSubtask(projectId, task.id, subtaskId)
      : this.tasksService.completeSubtask(projectId, task.id, subtaskId);

    request.subscribe({
      next: () => this.reloadTaskDetails(task.id),
      error: (err) => this.toast.error(getApiError(err)),
    });
  }

  removeSubtask(subtaskId: string): void {
    const projectId = this.context.selectedProjectId();
    const task = this.selectedTask();
    if (!projectId || !task) return;

    this.tasksService.removeSubtask(projectId, task.id, subtaskId).subscribe({
      next: () => {
        this.reloadTaskDetails(task.id);
        this.toast.success('Subtask removed.');
      },
      error: (err) => this.toast.error(getApiError(err)),
    });
  }

  isAssigned(userId: string): boolean {
    return !!this.selectedTask()?.assignments?.some((assignment) => assignment.userId === userId);
  }

  toggleAssignee(userId: string): void {
    const projectId = this.context.selectedProjectId();
    const task = this.selectedTask();
    if (!projectId || !task) return;

    const assigned = this.isAssigned(userId);
    const request = assigned
      ? this.tasksService.unassignTask(projectId, task.id, userId)
      : this.tasksService.assignTask(projectId, task.id, userId);

    request.subscribe({
      next: () => this.reloadTaskDetails(task.id),
      error: (err) => this.toast.error(getApiError(err)),
    });
  }

  onTaskDragStart(task: TaskSummary, listId: string): void {
    this.draggingTask = task;
    this.draggingFromListId = listId;
  }

  onTaskDragEnd(): void {
    this.draggingTask = null;
    this.draggingFromListId = null;
    this.dropListId.set(null);
  }

  allowTaskDrop(event: DragEvent, listId: string): void {
    event.preventDefault();
    this.dropListId.set(listId);
  }

  onTaskDrop(event: DragEvent, targetList: BoardList): void {
    event.preventDefault();
    const projectId = this.context.selectedProjectId();
    if (!projectId || !this.draggingTask) return;

    const movingTask = this.draggingTask;
    const sourceListId = this.draggingFromListId;
    this.onTaskDragEnd();

    const sourceList = this.lists().find((list) => list.id === sourceListId);
    const targetTasks = [...(targetList.tasks ?? [])];
    const filteredTarget = targetTasks.filter((item) => item.id !== movingTask.id);

    const sameList = sourceListId === targetList.id;
    const sourceTasks = sourceList?.tasks ?? [];
    const movedFromBottom = sameList && sourceTasks.at(-1)?.id === movingTask.id;

    if (sameList && movedFromBottom) {
      return;
    }

    const prevDisplayOrder =
      filteredTarget.length > 0 ? filteredTarget[filteredTarget.length - 1].displayOrder : null;

    const snapshot = this.cloneLists(this.lists());
    const nextBoard = this.cloneLists(this.lists());
    const moved: TaskSummary = {
      ...movingTask,
      listId: targetList.id,
      displayOrder: (prevDisplayOrder ?? 0) + 1,
    };

    for (const list of nextBoard) {
      if (list.id === sourceListId) {
        list.tasks = (list.tasks ?? []).filter((t) => t.id !== movingTask.id);
      }
      if (list.id === targetList.id) {
        const base = list.id === sourceListId ? list.tasks ?? [] : [...(list.tasks ?? [])];
        const without = base.filter((t) => t.id !== movingTask.id);
        list.tasks = [...without, moved];
      }
    }

    this.lists.set(nextBoard);
    this.recountTasks();
    this.context.setActiveBoardList(projectId, targetList.id);

    this.movingTaskId.set(movingTask.id);
    this.tasksService
      .moveTask(projectId, movingTask.id, {
        newListId: targetList.id,
        prevDisplayOrder,
        nextDisplayOrder: null,
      })
      .subscribe({
        next: () => {
          this.toast.info(`Moved to ${targetList.name}.`, 'Task');
        },
        error: (err) => {
          this.lists.set(snapshot);
          this.recountTasks();
          this.toast.error(getApiError(err));
        },
        complete: () => this.movingTaskId.set(null),
      });
  }

  isActiveList(listId: string): boolean {
    return this.context.activeBoardListId() === listId;
  }

  trackByListId(_: number, list: BoardList): string {
    return list.id;
  }

  trackByMemberId(_: number, member: ProjectMember): string {
    return member.id;
  }

  getPriorityMeta(priority: string) {
    return getPriorityMetadata(priority);
  }

  getTypeMeta(type: string) {
    return getTypeMetadata(type);
  }

  private loadProjects(): void {
    this.loading.set(true);
    this.error.set(null);

    this.projectsService.getProjects(false).subscribe({
      next: (projects) => {
        this.context.setProjects(projects ?? []);
        this.context.refreshActiveBoardListFromSession();
        const selectedProjectId = this.context.selectedProjectId();

        if (selectedProjectId) {
          this.loadBoard(selectedProjectId, { silent: false });
          return;
        }

        this.boardName.set('');
        this.lists.set([]);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load projects.');
        this.loading.set(false);
      },
    });
  }

  private loadBoard(projectId: string, opts: { silent: boolean }): void {
    if (opts.silent) {
      this.silentRefreshing.set(true);
    } else {
      this.loading.set(true);
    }
    this.error.set(null);

    this.boardsService
      .getBoard(projectId)
      .pipe(
        switchMap((board) => {
          this.boardName.set(board.name);

          const orderedLists = [...(board.lists ?? [])].sort(
            (a, b) => a.displayOrder - b.displayOrder,
          );

          if (!orderedLists.length) {
            return of([] as BoardList[]);
          }

          return forkJoin(
            orderedLists.map((list) =>
              this.tasksService.getTasksByList(projectId, list.id).pipe(
                catchError(() => of([] as TaskSummary[])),
                map((tasks) => ({ ...list, tasks })),
              ),
            ),
          );
        }),
        catchError(() => {
          this.error.set('Failed to load board for this project.');
          return of([] as BoardList[]);
        }),
      )
      .subscribe({
        next: (lists) => {
          this.lists.set(lists);
          this.recountTasks();
          this.context.refreshActiveBoardListFromSession();
        },
        complete: () => {
          this.loading.set(false);
          this.silentRefreshing.set(false);
        },
      });
  }

  private reloadTaskDetails(taskId: string): void {
    const projectId = this.context.selectedProjectId();
    if (!projectId || !this.taskDrawerOpen()) return;

    this.tasksService.getTaskById(projectId, taskId).subscribe({
      next: (task) => this.selectedTask.set(task),
      error: (err) => {
        const msg = getApiError(err);
        this.taskDrawerError.set(msg);
        this.toast.error(msg);
      },
    });
  }

  private mergeBoardStructure(board: BoardDto, previous: BoardList[]): void {
    const orderedLists = [...(board.lists ?? [])].sort((a, b) => a.displayOrder - b.displayOrder);
    const merged = orderedLists.map((list) => ({
      ...list,
      tasks: previous.find((p) => p.id === list.id)?.tasks ?? [],
    }));
    this.lists.set(merged);
    this.boardName.set(board.name);
    this.recountTasks();
  }

  private cloneLists(lists: BoardList[]): BoardList[] {
    return lists.map((list) => ({
      ...list,
      tasks: [...(list.tasks ?? [])].map((t) => ({ ...t })),
    }));
  }

  private recountTasks(): void {
    this.totalTasks.set(
      this.lists().reduce((total, list) => total + (list.tasks?.length ?? 0), 0),
    );
  }

  private patchTaskOnBoard(
    taskId: string,
    patch: Partial<Pick<TaskSummary, 'title' | 'priority' | 'type' | 'dueDate'>>,
  ): void {
    this.lists.update((rows) =>
      rows.map((row) => ({
        ...row,
        tasks: (row.tasks ?? []).map((t) => (t.id === taskId ? { ...t, ...patch } : t)),
      })),
    );
  }
}
