import { CommonModule } from '@angular/common';
import { Component, computed, inject, input } from '@angular/core';
import { AuthService } from '../../../core/auth/auth.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-current-user-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './current-user-card.component.html',
  styleUrl: './current-user-card.component.css',
})
export class CurrentUserCardComponent {
  private auth = inject(AuthService);

  user = input<User | null>(null);

  currentUser = computed(() => this.user() ?? this.auth.user());

  initials = computed(() => {
    const current = this.currentUser();
    if (!current) return 'NA';

    const first = current.firstName?.charAt(0) ?? '';
    const last = current.lastName?.charAt(0) ?? '';
    return `${first}${last}`.toUpperCase() || 'NA';
  });
}
