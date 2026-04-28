import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { ProjectsService } from '../../core/api/projects.service';
import { ToastService } from '../../core/notifications/toast.service';
import { WorkspaceContextService } from '../../core/api/workspace-context.service';
import { TechflowColorInputComponent } from '../../shared/components/techflow-color-input/techflow-color-input.component';
import { TechflowDateInputComponent } from '../../shared/components/techflow-date-input/techflow-date-input.component';
import { TechflowSpinnerComponent } from '../../shared/components/techflow-spinner/techflow-spinner.component';
import { ProjectSummary } from '../../shared/models/project.model';
import { getApiError } from '../../shared/utils/api-error';

type CreateProjectForm = FormGroup<{
  name: FormControl<string>;
  description: FormControl<string>;
  color: FormControl<string>;
  startDate: FormControl<string>;
  endDate: FormControl<string>;
}>;

@Component({
  selector: 'app-project-create-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    TechflowDateInputComponent,
    TechflowColorInputComponent,
    TechflowSpinnerComponent,
  ],
  templateUrl: './project-create-page.component.html',
  styleUrl: './project-create-page.component.css',
})
export class ProjectCreatePageComponent {
  private readonly projectsService = inject(ProjectsService);
  private readonly context = inject(WorkspaceContextService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  readonly submitting = signal(false);
  readonly submitError = signal<string | null>(null);

  readonly colorPresets: readonly string[] = [
    '#1d8f8c',
    '#1d4ed8',
    '#0f766e',
    '#c2410c',
    '#b91c1c',
    '#4f46e5',
  ];

  readonly createForm: CreateProjectForm = new FormGroup(
    {
      name: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      description: new FormControl('', {
        nonNullable: true,
        validators: [Validators.maxLength(500)],
      }),
      color: new FormControl('#1d8f8c', {
        nonNullable: true,
        validators: [Validators.required, Validators.pattern(/^#[0-9A-Fa-f]{6}$/)],
      }),
      startDate: new FormControl('', { nonNullable: true }),
      endDate: new FormControl('', { nonNullable: true }),
    },
    { validators: [validateDateRange] },
  );

  get name() {
    return this.createForm.controls.name;
  }

  get description() {
    return this.createForm.controls.description;
  }

  get color() {
    return this.createForm.controls.color;
  }

  get startDate() {
    return this.createForm.controls.startDate;
  }

  get endDate() {
    return this.createForm.controls.endDate;
  }

  submit(): void {
    this.submitError.set(null);
    this.createForm.markAllAsTouched();

    if (this.createForm.invalid) {
      return;
    }

    this.submitting.set(true);

    this.projectsService
      .createProject({
        name: this.name.value.trim(),
        description: this.toOptionalText(this.description.value),
        color: this.color.value,
        startDate: this.toIsoDate(this.startDate.value),
        endDate: this.toIsoDate(this.endDate.value),
      })
      .subscribe({
        next: (createdProject) => {
          this.saveProjectToContext(createdProject);
          this.toast.success('Project created.');
          this.router.navigate(['/app/projects', createdProject.id]);
        },
        error: (err) => {
          const msg = getApiError(err);
          this.submitError.set(msg);
          this.toast.error(msg);
          this.submitting.set(false);
        },
        complete: () => this.submitting.set(false),
      });
  }

  private saveProjectToContext(project: ProjectSummary): void {
    const existing = this.context.projects().filter((item) => item.id !== project.id);
    this.context.setProjects([project, ...existing]);
    this.context.setSelectedProject(project.id);
  }

  private toOptionalText(value: string): string | null {
    const trimmed = value.trim();
    return trimmed ? trimmed : null;
  }

  private toIsoDate(value: string): string | null {
    const trimmed = value.trim();
    return trimmed ? `${trimmed}T00:00:00.000Z` : null;
  }
}

function validateDateRange(control: AbstractControl): Record<string, boolean> | null {
  const startDateValue = control.get('startDate')?.value;
  const endDateValue = control.get('endDate')?.value;

  if (typeof startDateValue !== 'string' || typeof endDateValue !== 'string') {
    return null;
  }

  if (!startDateValue || !endDateValue) {
    return null;
  }

  return startDateValue <= endDateValue ? null : { dateRange: true };
}
