import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { InvitationsService } from '../../core/api/invitations.service';
import { ProjectsService } from '../../core/api/projects.service';
import { RolesService } from '../../core/api/roles.service';
import { WorkspaceContextService } from '../../core/api/workspace-context.service';
import { Invitation } from '../../shared/models/invitation.model';
import { ProjectMember } from '../../shared/models/project.model';
import { RoleSummary } from '../../shared/models/role.model';

@Component({
  selector: 'app-team-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './team-page.component.html',
  styleUrl: './team-page.component.css',
})
export class TeamPageComponent implements OnInit {
  private readonly projectsService = inject(ProjectsService);
  private readonly rolesService = inject(RolesService);
  private readonly invitationsService = inject(InvitationsService);

  readonly context = inject(WorkspaceContextService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly members = signal<ProjectMember[]>([]);
  readonly invitations = signal<Invitation[]>([]);
  readonly roles = signal<RoleSummary[]>([]);

  readonly inviteLoading = signal(false);
  readonly inviteError = signal<string | null>(null);

  readonly inviteForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email, Validators.maxLength(256)],
    }),
    roleId: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    scopeToProject: new FormControl(true, { nonNullable: true }),
  });

  get email() {
    return this.inviteForm.controls.email;
  }

  get roleId() {
    return this.inviteForm.controls.roleId;
  }

  get scopeToProject() {
    return this.inviteForm.controls.scopeToProject;
  }

  ngOnInit(): void {
    this.loadInitialData();
  }

  onProjectChange(event: Event): void {
    const projectId = (event.target as HTMLSelectElement).value;
    this.context.setSelectedProject(projectId);

    if (projectId) {
      this.loadMembers(projectId);
    }
  }

  refresh(): void {
    this.loadInitialData();
  }

  memberDisplay(member: ProjectMember): string {
    if (member.fullName?.trim()) return member.fullName;

    const name = `${member.firstName ?? ''} ${member.lastName ?? ''}`.trim();
    if (name) return name;

    return member.email || member.userId || member.id;
  }

  submitInvite(): void {
    this.inviteError.set(null);
    this.inviteForm.markAllAsTouched();

    if (this.inviteForm.invalid) {
      return;
    }

    const selectedProjectId = this.context.selectedProjectId();
    const isProjectScoped = this.scopeToProject.value;

    const payload = {
      email: this.email.value.trim(),
      roleId: this.roleId.value,
      ...(isProjectScoped && selectedProjectId ? { projectId: selectedProjectId } : {}),
    };

    this.inviteLoading.set(true);

    this.invitationsService.invite(payload).subscribe({
      next: (invitation) => {
        this.invitations.set([invitation, ...this.invitations()]);
        this.email.setValue('');
      },
      error: () => {
        this.inviteError.set('Failed to send invitation. Check role and email then try again.');
      },
      complete: () => this.inviteLoading.set(false),
    });
  }

  revokeInvitation(invitationId: string): void {
    this.invitationsService.revoke(invitationId).subscribe({
      next: () => {
        this.invitations.set(
          this.invitations().filter((invitation) => invitation.id !== invitationId),
        );
      },
      error: () => {
        this.error.set('Failed to revoke invitation.');
      },
    });
  }

  invitationTypeLabel(type: number): string {
    return type === 1 ? 'Project' : 'Company';
  }

  private loadInitialData(): void {
    this.loading.set(true);
    this.error.set(null);
    this.inviteError.set(null);

    forkJoin({
      projects: this.projectsService.getProjects(false).pipe(catchError(() => of([]))),
      roles: this.rolesService.getRoles().pipe(catchError(() => of([]))),
      invitations: this.invitationsService.getPending().pipe(catchError(() => of([]))),
    }).subscribe({
      next: ({ projects, roles, invitations }) => {
        this.context.setProjects(projects ?? []);
        this.roles.set(roles ?? []);
        this.invitations.set(
          [...(invitations ?? [])].sort(
            (a, b) => Date.parse(b.createdAt) - Date.parse(a.createdAt),
          ),
        );

        if (roles.length) {
          const existingRoleId = this.roleId.value;
          const roleExists = roles.some((role) => role.id === existingRoleId);
          if (!roleExists) {
            this.roleId.setValue(roles[0].id);
          }
        }

        const selectedProjectId = this.context.selectedProjectId();
        if (selectedProjectId) {
          this.loadMembers(selectedProjectId);
          return;
        }

        this.members.set([]);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load team data.');
        this.loading.set(false);
      },
    });
  }

  private loadMembers(projectId: string): void {
    this.projectsService.getProjectMembers(projectId).subscribe({
      next: (members) => this.members.set(Array.isArray(members) ? members : []),
      error: () => {
        this.error.set('Failed to load project members.');
        this.members.set([]);
      },
      complete: () => this.loading.set(false),
    });
  }
}
