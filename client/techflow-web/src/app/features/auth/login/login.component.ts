import { Component, inject, OnDestroy, signal } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { ToastService } from '../../../core/notifications/toast.service';
import { LoginRequest } from '../../../shared/models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnDestroy {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  form = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });

  readonly showPassword = signal(false);

  get email() {
    return this.form.controls.email;
  }
  get password() {
    return this.form.controls.password;
  }
  get loading() {
    return this.auth.loading;
  }
  get apiError() {
    return this.auth.error;
  }

  onSubmit(): void {
    this.auth.clearError();
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    const req: LoginRequest = {
      email: this.email.value!,
      password: this.password.value!,
    };

    this.auth.login(req).subscribe({
      next: () => {
        this.toast.success('Welcome back!');
        setTimeout(() => this.router.navigate(['/app']), 600);
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

  ngOnDestroy(): void {
    this.auth.clearError();
  }
}
