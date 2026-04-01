import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, Input, forwardRef, inject, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export interface TechflowDropdownOption {
  value: string;
  label: string;
}

@Component({
  selector: 'app-techflow-dropdown',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './techflow-dropdown.component.html',
  styleUrl: './techflow-dropdown.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TechflowDropdownComponent),
      multi: true
    }
  ]
})
export class TechflowDropdownComponent implements ControlValueAccessor {
  private host = inject(ElementRef<HTMLElement>);

  @Input() id = '';
  @Input() placeholder = 'Select an option';
  @Input() icon = 'expand_more';
  @Input() options: TechflowDropdownOption[] = [];

  value: string | null = null;
  disabled = false;
  isOpen = signal(false);

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

  get selectedLabel(): string {
    const selected = this.options.find(o => o.value === this.value);
    return selected?.label ?? this.placeholder;
  }

  toggleOpen(): void {
    if (this.disabled) return;
    this.isOpen.update(v => !v);
  }

  selectOption(value: string): void {
    this.value = value;
    this.onChange(this.value);
    this.onTouched();
    this.isOpen.set(false);
  }

  trackByValue(_: number, option: TechflowDropdownOption): string {
    return option.value;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.host.nativeElement.contains(event.target as Node)) {
      this.isOpen.set(false);
      this.onTouched();
    }
  }
}
