using System;
using FluentValidation;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Application.Tickets;

public class PauseTicketCommandValidator : AbstractValidator<PauseTicketCommand>
{
    private static readonly string[] AllowedPauseTypes = { nameof(TicketStatuses.WaitingCustomer), nameof(TicketStatuses.WaitingThirdParty) };

    public PauseTicketCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required");
        RuleFor(x => x.TicketId).NotEmpty().WithMessage("TicketId is required");
        RuleFor(x => x.ActorId).NotEmpty().WithMessage("ActorId is required");

        RuleFor(x => x.PauseType)
            .NotEmpty().WithMessage("PauseType is required")
            .Must(pt => Array.Exists(AllowedPauseTypes, a => string.Equals(a, pt, StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"PauseType must be one of: {string.Join(',', AllowedPauseTypes)}");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(2000);

        RuleFor(x => x.ResumeAt)
            .Must(dt => dt == null || dt > DateTime.UtcNow.AddMinutes(-1))
            .WithMessage("ResumeAt must be in the future if specified");
    }
}
