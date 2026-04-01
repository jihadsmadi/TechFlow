import { Component, inject, signal, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Router }    from '@angular/router';
import { AuthService }  from '../../../core/auth/auth.service';
import { LoginRequest } from '../../../shared/models/auth.model';

@Component({
  selector:    'app-login',
  standalone:  true,
  imports:     [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl:    './login.component.css'
})
export class LoginComponent implements OnDestroy {
  private auth   = inject(AuthService);
  private router = inject(Router);
  private readonly toastDurationMs = 3000;
  private readonly toastExitMs = 220;

  form = new FormGroup({
    email:    new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)])
  });

  showPassword = signal(false);
  toast        = signal<string | null>(null);
  toastLeaving = signal(false);
  private toastTimer?: ReturnType<typeof setTimeout>;
  private toastExitTimer?: ReturnType<typeof setTimeout>;

  get email()    { return this.form.controls.email;    }
  get password() { return this.form.controls.password; }
  get loading()  { return this.auth.loading;           }
  get apiError() { return this.auth.error;             }

  onSubmit(): void {
    this.auth.clearError();
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    const req: LoginRequest = {
      email:    this.email.value!,
      password: this.password.value!
    };

    this.auth.login(req).subscribe({
      complete: () => {
        if (!this.auth.error()) {
          this.showToast('Welcome back!');
          setTimeout(() => this.router.navigate(['/app/projects']), 1200);
        }
      }
    });
  }

  togglePassword()          { this.showPassword.update(v => !v); }

  private showToast(msg: string): void {
    clearTimeout(this.toastExitTimer);
    this.toastLeaving.set(false);
    this.toast.set(msg);
    clearTimeout(this.toastTimer);
    this.toastTimer = setTimeout(() => this.dismissToast(), this.toastDurationMs);
  }

  dismissToast(): void {
    if (!this.toast()) return;
    clearTimeout(this.toastTimer);
    clearTimeout(this.toastExitTimer);
    this.toastLeaving.set(true);
    this.toastExitTimer = setTimeout(() => {
      this.toast.set(null);
      this.toastLeaving.set(false);
    }, this.toastExitMs);
  }

  ngOnDestroy(): void {
    clearTimeout(this.toastTimer);
    clearTimeout(this.toastExitTimer);
    this.auth.clearError();
  }
}
