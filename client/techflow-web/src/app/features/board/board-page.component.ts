import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

import { BoardsService } from '../../core/api/boards.service';
import { ProjectsService } from '../../core/api/projects.service';
import { TasksService } from '../../core/api/tasks.service';
import { BoardList } from '../../shared/models/board.model';
import { ProjectSummary } from '../../shared/models/project.model';

@Component({
  selector: 'app-board-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './board-page.component.html',
  styleUrl: './board-page.component.css',
})
export class BoardPageComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);
  private readonly boardsService = inject(BoardsService);
  private readonly tasksService = inject(TasksService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly projects = signal<ProjectSummary[]>([]);
  readonly selectedProjectId = signal<string>('');
  readonly boardName = signal<string>('');
  readonly lists = signal<BoardList[]>([]);

  ngOnInit(): void {
    this.loadProjects();
  }

  onProjectChange(event: Event): void {
    const id = (event.target as HTMLSelectElement).value;
    this.selectedProjectId.set(id);
    if (id) this.loadBoard(id);
  }

  private loadProjects(): void {
    this.loading.set(true);
    this.error.set(null);

    this.projectsService.getProjects(false).subscribe({
      next: (projects) => {
        this.projects.set(projects ?? []);
        const first = projects?.[0]?.id ?? '';
        this.selectedProjectId.set(first);
        if (first) {
          this.loadBoard(first);
          return;
        }
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load projects.');
        this.loading.set(false);
      },
    });
  }

  private loadBoard(projectId: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.boardsService
      .getBoard(projectId)
      .pipe(
        switchMap((board) => {
          this.boardName.set(board.name);
          if (!board.lists?.length) {
            return of([] as BoardList[]);
          }

          return forkJoin(
            board.lists.map((list) =>
              this.tasksService.getTasksByList(projectId, list.id).pipe(
                catchError(() => of([])),
                switchMap((tasks) => of({ ...list, tasks }))
              )
            )
          );
        }),
        catchError(() => {
          this.error.set('Failed to load board for this project.');
          return of([] as BoardList[]);
        })
      )
      .subscribe({
        next: (lists) => this.lists.set(lists),
        complete: () => this.loading.set(false),
      });
  }
}
