import { Component, inject, OnDestroy, signal } from '@angular/core';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { ToastService } from '../../../core/notifications/toast.service';
import { RegisterRequest } from '../../../shared/models/auth.model';
import { TechflowDropdownComponent, TechflowDropdownOption } from '../../../shared/components/techflow-dropdown/techflow-dropdown.component';

const passwordMatchValidator = (group: AbstractControl): ValidationErrors | null => {
  const pw = group.get('password')?.value;
  const cpw = group.get('confirmPassword')?.value;
  return pw === cpw ? null : { passwordMismatch: true };
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, TechflowDropdownComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnDestroy {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  step = signal<1 | 2>(1);

  step1 = new FormGroup({
    companyName: new FormControl('', [Validators.required, Validators.minLength(2)]),
    companySlug: new FormControl('', [Validators.required, Validators.pattern(/^[a-z0-9-]+$/)]),
    industry: new FormControl<string | null>(null),
    companyEmail: new FormControl('', [Validators.required, Validators.email]),
  });

  step2 = new FormGroup(
    {
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(8)]),
      confirmPassword: new FormControl('', [Validators.required]),
    },
    { validators: passwordMatchValidator },
  );

  readonly showPassword = signal(false);
  readonly showConfirmPassword = signal(false);
  readonly industryOptions: TechflowDropdownOption[] = [
    { value: 'Technology', label: 'Technology & Software' },
    { value: 'Design', label: 'Creative & Design' },
    { value: 'Marketing', label: 'Marketing & Sales' },
    { value: 'Finance', label: 'Finance & Fintech' },
    { value: 'Other', label: 'Other' },
  ];

  get companyName() {
    return this.step1.controls.companyName;
  }
  get companySlug() {
    return this.step1.controls.companySlug;
  }
  get industry() {
    return this.step1.controls.industry;
  }
  get companyEmail() {
    return this.step1.controls.companyEmail;
  }
  get firstName() {
    return this.step2.controls.firstName;
  }
  get lastName() {
    return this.step2.controls.lastName;
  }
  get email() {
    return this.step2.controls.email;
  }
  get password() {
    return this.step2.controls.password;
  }
  get confirmPassword() {
    return this.step2.controls.confirmPassword;
  }
  get loading() {
    return this.auth.loading;
  }
  get apiError() {
    return this.auth.error;
  }

  get passwordStrength(): 'very-weak' | 'weak' | 'good' | 'strong' | null {
    const v = this.password.value ?? '';
    if (!v) return null;

    const hasLower = /[a-z]/.test(v);
    const hasUpper = /[A-Z]/.test(v);
    const hasNumber = /[0-9]/.test(v);
    const hasSpecial = /[^a-zA-Z0-9]/.test(v);
    const hasMixedLetters = hasLower && hasUpper;
    const score =
      (v.length >= 8 ? 1 : 0) +
      (hasMixedLetters ? 1 : 0) +
      (hasNumber ? 1 : 0) +
      (hasSpecial ? 1 : 0);

    if (score <= 1) return 'very-weak';
    if (score === 2) return 'weak';
    if (score === 3) return 'good';
    return 'strong';
  }

  onCompanyNameInput(event: Event): void {
    const val = (event.target as HTMLInputElement).value;
    const slug = val
      .toLowerCase()
      .trim()
      .replace(/\s+/g, '-')
      .replace(/[^a-z0-9-]/g, '')
      .replace(/-+/g, '-');
    this.companySlug.setValue(slug, { emitEvent: false });
  }

  goToStep2(): void {
    this.step1.markAllAsTouched();
    if (this.step1.invalid) return;

    this.step.set(2);
    this.auth.clearError();
  }

  goToStep1(): void {
    this.step.set(1);
    this.auth.clearError();
  }

  onStepClick(targetStep: 1 | 2): void {
    if (targetStep === 1) {
      this.goToStep1();
      return;
    }

    this.goToStep2();
  }

  onRegisterSubmit(): void {
    this.step2.markAllAsTouched();
    if (this.step2.invalid) return;

    const req: RegisterRequest = {
      companyName: this.companyName.value!,
      companySlug: this.companySlug.value!,
      companyEmail: this.companyEmail.value!,
      industry: this.industry.value ?? null,
      firstName: this.firstName.value!,
      lastName: this.lastName.value!,
      email: this.email.value!,
      password: this.password.value!,
    };

    this.auth.register(req).subscribe({
      next: () => {
        this.toast.success('Workspace created!');
        setTimeout(() => this.router.navigate(['/app'], { queryParams: { onboarding: '1' } }), 600);
      },
      complete: () => {
        const err = this.auth.error();
        if (err) {
          this.toast.error(err);
        }
      },
    });
  }

  togglePassword(): void {
    this.showPassword.update((v) => !v);
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword.update((v) => !v);
  }

  ngOnDestroy(): void {
    this.auth.clearError();
  }
}
