import { CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { TechflowDateInputComponent } from '../../shared/components/techflow-date-input/techflow-date-input.component';
import { TechflowDatePickerComponent } from '../../shared/components/techflow-date-picker/techflow-date-picker.component';
import { TechflowDropdownComponent } from '../../shared/components/techflow-dropdown/techflow-dropdown.component';
import { getPriorityMetadata, getTypeMetadata } from '../../shared/constants/task-metadata';
import {
  TASK_PRIORITY_DROPDOWN_OPTIONS,
  TASK_TYPE_DROPDOWN_OPTIONS,
} from '../../shared/constants/task-field-options';

export interface BoardTaskCreatePayload {
  listId: string;
  title: string;
  priority: string;
  type: string;
  dueDate: string;
}

@Component({
  selector: 'app-board-task-create-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, TechflowDateInputComponent, TechflowDropdownComponent],
  templateUrl: './board-task-create-dialog.component.html',
  styleUrl: './board-task-create-dialog.component.css',
})
export class BoardTaskCreateDialogComponent {
  @ViewChild('dialogEl') private dialogRef?: ElementRef<HTMLDialogElement>;

  readonly submitted = output<BoardTaskCreatePayload>();
  readonly cancelled = output<void>();

  readonly priorityOptions = TASK_PRIORITY_DROPDOWN_OPTIONS;
  readonly typeOptions = TASK_TYPE_DROPDOWN_OPTIONS;

  readonly listId = signal('');
  title = '';
  priority = 'Medium';
  type = 'Task';
  dueDate = '';

  open(listId: string): void {
    this.listId.set(listId);
    this.title = '';
    this.priority = 'Medium';
    this.type = 'Task';
    this.dueDate = '';
    queueMicrotask(() => this.dialogRef?.nativeElement.showModal());
  }

  close(): void {
    this.dialogRef?.nativeElement.close();
  }

  onDialogClose(): void {
    this.cancelled.emit();
  }

  submit(): void {
    const t = this.title.trim();
    if (!t) return;
    this.submitted.emit({
      listId: this.listId(),
      title: t,
      priority: this.priority,
      type: this.type,
      dueDate: this.dueDate,
    });
    this.close();
  }

  getPriorityMeta(priority: string) {
    return getPriorityMetadata(priority);
  }

  getTypeMeta(type: string) {
    return getTypeMetadata(type);
  }
}
