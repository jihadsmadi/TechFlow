import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';

import { BoardsService } from '../../core/api/boards.service';
import { ProjectsService } from '../../core/api/projects.service';
import { TasksService } from '../../core/api/tasks.service';
import { MyTask } from '../../shared/models/task.model';

@Component({
  selector: 'app-my-tasks-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-tasks-page.component.html',
  styleUrl: './my-tasks-page.component.css',
})
export class MyTasksPageComponent implements OnInit {
  private readonly tasksService = inject(TasksService);
  private readonly projectsService = inject(ProjectsService);
  private readonly boardsService = inject(BoardsService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly includeCompleted = signal(true);
  readonly tasks = signal<MyTask[]>([]);
  readonly statusFilter = signal<'all' | 'open' | 'completed'>('all');
  readonly priorityFilter = signal<'all' | 'low' | 'medium' | 'high' | 'critical'>('all');
  readonly dueFilter = signal<'all' | 'overdue' | 'today' | 'thisWeek' | 'noDueDate'>('all');

  readonly filteredTasks = computed(() => this.tasks().filter((task) => this.matchesFilters(task)));

  readonly openTaskCount = computed(() =>
    this.filteredTasks().filter((task) => !task.isCompleted).length,
  );

  readonly groupedTasks = computed(() => {
    const groups = new Map<string, MyTask[]>();

    const sortedTasks = [...this.filteredTasks()].sort((a, b) => {
      if (a.isCompleted !== b.isCompleted) {
        return Number(a.isCompleted) - Number(b.isCompleted);
      }

      const aDue = a.dueDate ? Date.parse(a.dueDate) : Number.MAX_SAFE_INTEGER;
      const bDue = b.dueDate ? Date.parse(b.dueDate) : Number.MAX_SAFE_INTEGER;
      return aDue - bDue;
    });

    for (const task of sortedTasks) {
      const key = task.projectName ?? task.projectId;
      const bucket = groups.get(key) ?? [];
      bucket.push(task);
      groups.set(key, bucket);
    }

    return Array.from(groups.entries()).map(([projectName, tasks]) => ({ projectName, tasks }));
  });

  ngOnInit(): void {
    this.load();
  }

  updateStatusFilter(value: string): void {
    const next = value as 'all' | 'open' | 'completed';
    this.statusFilter.set(next);
    this.includeCompleted.set(next !== 'open');
    this.load();
  }

  updatePriorityFilter(value: string): void {
    this.priorityFilter.set(value as 'all' | 'low' | 'medium' | 'high' | 'critical');
  }

  updateDueFilter(value: string): void {
    this.dueFilter.set(value as 'all' | 'overdue' | 'today' | 'thisWeek' | 'noDueDate');
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      tasks: this.tasksService.getMyTasks(this.includeCompleted()).pipe(catchError(() => of([]))),
      projects: this.projectsService.getProjects(true).pipe(catchError(() => of([]))),
    })
      .pipe(
        switchMap(({ tasks, projects }) => {
          const incomingTasks = tasks ?? [];

          if (!incomingTasks.length) {
            return of([] as MyTask[]);
          }

          const projectNameMap = new Map(projects.map((project) => [project.id, project.name]));
          const projectIds = Array.from(new Set(incomingTasks.map((task) => task.projectId)));

          const boardRequests = projectIds.map((projectId) =>
            this.boardsService.getBoard(projectId).pipe(
              map((board) => ({ projectId, lists: board.lists })),
              catchError(() => of({ projectId, lists: [] })),
            ),
          );

          return forkJoin(boardRequests).pipe(
            map((boards) => {
              const listNameMap = new Map<string, string>();

              for (const board of boards) {
                for (const list of board.lists) {
                  listNameMap.set(list.id, list.name);
                }
              }

              return incomingTasks.map((task) => ({
                ...task,
                projectName: projectNameMap.get(task.projectId),
                listName: listNameMap.get(task.listId),
              }));
            }),
          );
        }),
      )
      .subscribe({
        next: (tasks) => this.tasks.set(tasks),
        error: () => this.error.set('Failed to load your tasks.'),
        complete: () => this.loading.set(false),
      });
  }

  private matchesFilters(task: MyTask): boolean {
    if (this.statusFilter() === 'open' && task.isCompleted) return false;
    if (this.statusFilter() === 'completed' && !task.isCompleted) return false;

    if (this.priorityFilter() !== 'all') {
      const priority = (task.priority ?? '').trim().toLowerCase();
      if (priority !== this.priorityFilter()) return false;
    }

    if (!this.matchesDueFilter(task)) return false;

    return true;
  }

  private matchesDueFilter(task: MyTask): boolean {
    const filter = this.dueFilter();
    if (filter === 'all') return true;

    const due = task.dueDate ? new Date(task.dueDate) : null;
    if (filter === 'noDueDate') return !due;
    if (!due) return false;

    const now = new Date();
    const todayStart = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    const tomorrowStart = new Date(todayStart);
    tomorrowStart.setDate(todayStart.getDate() + 1);

    if (filter === 'overdue') {
      return due < todayStart;
    }

    if (filter === 'today') {
      return due >= todayStart && due < tomorrowStart;
    }

    const weekEnd = new Date(todayStart);
    weekEnd.setDate(todayStart.getDate() + 7);
    return due >= todayStart && due < weekEnd;
  }
}
