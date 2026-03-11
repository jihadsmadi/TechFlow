using FluentValidation;

namespace TechFlow.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        // ── Company 
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(50).WithMessage("Company name must not exceed 50 characters.");

        RuleFor(x => x.CompanySlug)
            .NotEmpty().WithMessage("Company slug is required.")
            .MaximumLength(60).WithMessage("Company slug must not exceed 60 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must be lowercase letters, numbers, and hyphens only (e.g. my-company).");

        RuleFor(x => x.CompanyEmail)
            .NotEmpty().WithMessage("Company contact email is required.")
            .EmailAddress().WithMessage("Company contact email is not valid.");

        RuleFor(x => x.Industry)
            .MaximumLength(50).WithMessage("Industry must not exceed 50 characters.")
            .When(x => x.Industry is not null);

        // ── User 
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.");
    }
}