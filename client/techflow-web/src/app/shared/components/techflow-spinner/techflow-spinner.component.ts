import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-techflow-spinner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './techflow-spinner.component.html',
  styleUrl: './techflow-spinner.component.css',
})
export class TechflowSpinnerComponent {
  @Input() size: 'sm' | 'md' = 'md';
  @Input() label = '';
}
