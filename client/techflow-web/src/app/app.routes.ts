import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login',
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then((m) => m.RegisterComponent),
  },
  {
    path: 'app',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/app-shell/app-shell.component').then((m) => m.AppShellComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'workspace',
      },
      {
        path: 'workspace',
        loadComponent: () =>
          import('./features/workspace/workspace-home.component').then((m) => m.WorkspaceHomeComponent),
      },
      {
        path: 'projects',
        loadComponent: () =>
          import('./features/projects/projects-page.component').then((m) => m.ProjectsPageComponent),
      },
      {
        path: 'projects/new',
        loadComponent: () =>
          import('./features/projects/project-create-page.component').then(
            (m) => m.ProjectCreatePageComponent,
          ),
      },
      {
        path: 'projects/:projectId',
        loadComponent: () =>
          import('./features/projects/project-details-page.component').then(
            (m) => m.ProjectDetailsPageComponent,
          ),
      },
      {
        path: 'my-tasks',
        loadComponent: () =>
          import('./features/my-tasks/my-tasks-page.component').then((m) => m.MyTasksPageComponent),
      },
      {
        path: 'board',
        loadComponent: () =>
          import('./features/board/boardPage/board-page.component').then((m) => m.BoardPageComponent),
      },
      {
        path: 'sprints',
        loadComponent: () =>
          import('./features/sprints/sprints-page.component').then((m) => m.SprintsPageComponent),
      },
      {
        path: 'team',
        loadComponent: () =>
          import('./features/team/team-page.component').then((m) => m.TeamPageComponent),
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];
