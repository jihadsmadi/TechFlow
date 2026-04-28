import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, Input, forwardRef, inject, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-techflow-date-picker',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './techflow-date-picker.component.html',
  styleUrl: './techflow-date-picker.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TechflowDatePickerComponent),
      multi: true
    }
  ]
})
export class TechflowDatePickerComponent implements ControlValueAccessor {
  private host = inject(ElementRef<HTMLElement>);

  @Input() label: string | null = null;
  @Input() disabled = false;
  @Input() inputId = '';

  value: string | null = null;
  isOpen = signal(false);
  currentMonth = signal(new Date());

  private onChange: (value: string | null) => void = () => { };
  private onTouched: () => void = () => { };

  writeValue(value: string | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  get displayValue(): string {
    if (!this.value) return '';
    const date = new Date(this.value + 'T00:00:00Z');
    const formatter = new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
    return formatter.format(date);
  }

  get monthYear(): string {
    const formatter = new Intl.DateTimeFormat('en-US', {
      month: 'long',
      year: 'numeric'
    });
    return formatter.format(this.currentMonth());
  }

  get daysOfWeek(): string[] {
    return ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
  }

  get calendarDays(): (number | null)[] {
    const month = this.currentMonth().getMonth();
    const year = this.currentMonth().getFullYear();
    const firstDay = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    const days: (number | null)[] = [];
    for (let i = 0; i < firstDay; i++) {
      days.push(null);
    }
    for (let i = 1; i <= daysInMonth; i++) {
      days.push(i);
    }
    return days;
  }

  toggleOpen(): void {
    if (this.disabled) return;
    this.isOpen.update(v => !v);
    if (this.isOpen()) {
      if (this.value) {
        const date = new Date(this.value + 'T00:00:00Z');
        this.currentMonth.set(new Date(date.getUTCFullYear(), date.getUTCMonth(), 1));
      } else {
        this.currentMonth.set(new Date(new Date().getFullYear(), new Date().getMonth(), 1));
      }
    }
  }

  previousMonth(): void {
    const current = this.currentMonth();
    this.currentMonth.set(new Date(current.getFullYear(), current.getMonth() - 1, 1));
  }

  nextMonth(): void {
    const current = this.currentMonth();
    this.currentMonth.set(new Date(current.getFullYear(), current.getMonth() + 1, 1));
  }

  selectDate(day: number): void {
    const month = this.currentMonth();
    const date = new Date(month.getFullYear(), month.getMonth(), day);
    const isoString = date.toISOString().split('T')[0];
    this.value = isoString;
    this.onChange(isoString);
    this.onTouched();
    this.isOpen.set(false);
  }

  isSelected(day: number | null): boolean {
    if (!day || !this.value) return false;
    const month = this.currentMonth();
    const dateString = `${month.getFullYear()}-${String(month.getMonth() + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
    return dateString === this.value;
  }

  isToday(day: number | null): boolean {
    if (!day) return false;
    const today = new Date();
    const month = this.currentMonth();
    return (
      day === today.getDate() &&
      month.getMonth() === today.getMonth() &&
      month.getFullYear() === today.getFullYear()
    );
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.host.nativeElement.contains(event.target as Node)) {
      this.isOpen.set(false);
      this.onTouched();
    }
  }
}
