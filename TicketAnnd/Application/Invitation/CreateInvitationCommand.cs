using MediatR;
using Microsoft.Extensions.Configuration;
using TicketAnnd.Application.Common;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.ReadModels;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Application.Invitation;

public record CreateInvitationCommand(Guid CompanyId, string Email, AppRoles Role) : IRequest<CreateInvitationResult>;

public record CreateInvitationResult(Guid InvitationId);

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, CreateInvitationResult>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IMediator mediator,
        IUnitOfWork unitOfWork)
    {
        _invitationRepository = invitationRepository;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }
    const int InvitationExpireDay = 7;
    public async Task<CreateInvitationResult> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var company = await _companyRepository.GetReadModelByIdAsync(request.CompanyId, cancellationToken);
        var companyName = company?.Name ?? "your company";
        var existingUser = await _userRepository.GetLoginUserByEmailAsync(normalizedEmail, cancellationToken);
        Guid? userId = existingUser?.Id;
        var ivtn = await _invitationRepository.GetByEmailAndCompanyIdAsync(normalizedEmail, request.CompanyId, cancellationToken);
            if(ivtn !=null && ivtn.Status !=InviationStatuses.Accepted)
            {
                ivtn.Update(userId ?? Guid.Empty,
                            email: null,
                            expires: DateTime.UtcNow.AddDays(InvitationExpireDay),
                            role: request.Role,
                            expireDays: InvitationExpireDay,
                            companyName: companyName);
                await _invitationRepository.UpdateAsync(ivtn);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new CreateInvitationResult(ivtn.Id);
            }
        var invitation = new Domain.Entities.Invitation
        {
            Id = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            Email = normalizedEmail,
            Expires = DateTime.UtcNow.AddDays(InvitationExpireDay),
            Status = InviationStatuses.Pending,
            Role = request.Role,
            UserId = userId
        };

        invitation.CreateNew(request.Role, userId, InvitationExpireDay, companyName);
        await _invitationRepository.AddAsync(invitation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateInvitationResult(invitation.Id);
    }
}
