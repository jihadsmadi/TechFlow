import { CommonModule } from '@angular/common';
import { Component, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

const DEFAULT_PRESETS = [
  '#1d8f8c',
  '#1d4ed8',
  '#0f766e',
  '#c2410c',
  '#b91c1c',
  '#4f46e5',
] as const;

@Component({
  selector: 'app-techflow-color-input',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './techflow-color-input.component.html',
  styleUrl: './techflow-color-input.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TechflowColorInputComponent),
      multi: true,
    },
  ],
})
export class TechflowColorInputComponent implements ControlValueAccessor {
  @Input() inputId = '';
  @Input() label = '';
  @Input() presets: readonly string[] = [...DEFAULT_PRESETS];
  @Input() disabled = false;

  value = '#1d8f8c';
  private onChange: (v: string) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string | null): void {
    if (value && /^#[0-9A-Fa-f]{6}$/.test(value)) {
      this.value = value.toLowerCase();
    }
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

  onNativePicker(event: Event): void {
    const hex = (event.target as HTMLInputElement).value;
    this.applyHex(hex);
  }

  selectPreset(hex: string): void {
    this.applyHex(hex);
    this.onTouched();
  }

  private applyHex(hex: string): void {
    this.value = hex.toLowerCase();
    this.onChange(this.value);
  }
}
