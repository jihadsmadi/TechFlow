import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService }        from '../../core/auth/auth.service';

@Component({
  selector:   'app-home',
  standalone: true,
  imports:    [RouterLink],
  templateUrl: './home.component.html',
  styleUrl:    './home.component.css'
})
export class HomeComponent {
  auth   = inject(AuthService);
  router = inject(Router);

  logout(): void {
    this.auth.logout();
  }
}