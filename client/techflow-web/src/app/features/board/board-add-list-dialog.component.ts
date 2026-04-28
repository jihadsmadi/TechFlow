import { CommonModule } from '@angular/common';
import { Component, ElementRef, ViewChild, output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-board-add-list-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './board-add-list-dialog.component.html',
  styleUrl: './board-add-list-dialog.component.css',
})
export class BoardAddListDialogComponent {
  @ViewChild('dialogEl') private dialogRef?: ElementRef<HTMLDialogElement>;

  readonly submitted = output<{ name: string }>();
  readonly cancelled = output<void>();

  listName = '';

  open(): void {
    this.listName = '';
    queueMicrotask(() => this.dialogRef?.nativeElement.showModal());
  }

  close(): void {
    this.dialogRef?.nativeElement.close();
  }

  onDialogClose(): void {
    this.cancelled.emit();
  }

  submit(): void {
    const name = this.listName.trim();
    if (!name) return;
    this.submitted.emit({ name });
    this.close();
  }
}
