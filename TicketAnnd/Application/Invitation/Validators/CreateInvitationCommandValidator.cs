using FluentValidation;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Invitation.Validators;

public class CreateInvitationCommandValidator : AbstractValidator<CreateInvitationCommand>
{
    private readonly IInvitationRepository invitationRepo;

    public CreateInvitationCommandValidator(IInvitationRepository invitationRepo)
    {
        this.invitationRepo = invitationRepo;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.");

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
                !await invitationRepo.PendingExists(cmd.Email, cmd.CompanyId))
            .WithMessage("Email already invited");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => role is AppRoles.Agent or AppRoles.Customer)
            .WithMessage("Cannot invite owner role.");
    }
}
