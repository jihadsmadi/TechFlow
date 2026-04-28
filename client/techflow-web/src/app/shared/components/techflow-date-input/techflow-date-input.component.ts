import { CommonModule } from '@angular/common';
import { Component, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-techflow-date-input',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './techflow-date-input.component.html',
  styleUrl: './techflow-date-input.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TechflowDateInputComponent),
      multi: true,
    },
  ],
})
export class TechflowDateInputComponent implements ControlValueAccessor {
  @Input() inputId = '';
  @Input() label = '';
  @Input() disabled = false;

  value = '';
  private onChange: (v: string) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string | null): void {
    this.value = value ?? '';
  }

  registerOnChange(fn: (v: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onInputEvent(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.value = value;
    this.onChange(value);
  }

  blur(): void {
    this.onTouched();
  }
}
