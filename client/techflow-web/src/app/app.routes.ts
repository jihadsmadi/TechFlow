import { Routes }    from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home.component')
        .then(m => m.HomeComponent)
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component')
        .then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component')
        .then(m => m.RegisterComponent)
  },
  {
    path: 'app',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/app-shell/app-shell.component')
        .then(m => m.AppShellComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'projects'
      },
      {
        path: 'projects',
        loadComponent: () =>
          import('./features/projects/projects-page.component')
            .then(m => m.ProjectsPageComponent)
      },
      {
        path: 'board',
        loadComponent: () =>
          import('./features/board/board-page.component')
            .then(m => m.BoardPageComponent)
      },
      {
        path: 'sprints',
        loadComponent: () =>
          import('./features/sprints/sprints-page.component')
            .then(m => m.SprintsPageComponent)
      },
      {
        path: 'my-tasks',
        loadComponent: () =>
          import('./features/my-tasks/my-tasks-page.component')
            .then(m => m.MyTasksPageComponent)
      },
      {
        path: 'team',
        loadComponent: () =>
          import('./features/team/team-page.component')
            .then(m => m.TeamPageComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
