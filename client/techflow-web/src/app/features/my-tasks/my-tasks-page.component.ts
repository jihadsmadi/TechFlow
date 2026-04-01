import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';

import { TasksService } from '../../core/api/tasks.service';
import { MyTask } from '../../shared/models/task.model';

@Component({
  selector: 'app-my-tasks-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-tasks-page.component.html',
  styleUrl: './my-tasks-page.component.css',
})
export class MyTasksPageComponent implements OnInit {
  private readonly tasksService = inject(TasksService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly includeCompleted = signal(false);
  readonly tasks = signal<MyTask[]>([]);

  ngOnInit(): void {
    this.load();
  }

  toggleIncludeCompleted(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.includeCompleted.set(checked);
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.tasksService.getMyTasks(this.includeCompleted()).subscribe({
      next: (tasks) => this.tasks.set(tasks ?? []),
      error: () => this.error.set('Failed to load your tasks.'),
      complete: () => this.loading.set(false),
    });
  }
}
