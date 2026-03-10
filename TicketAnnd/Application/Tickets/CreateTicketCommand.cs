using MediatR;
using TicketAnnd.Domain.Repositories;
using FluentValidation;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain;
using TicketAnnd.Application;
using TicketAnnd.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using TicketAnnd.Infrastructure.Persistence;

namespace TicketAnnd.Application.Tickets;

public record CreateTicketCommand(Guid CompanyId, Guid RaiserId, Guid CategoryId, Guid SlaRuleId, string Subject, string? Body, Guid? TeamId) : IRequest<Guid>;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ISlaPolicyRepository _slaPolicyRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly ITeamRepository _teamRepo;
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        ISlaPolicyRepository slaPolicyRepo,
        ICategoryRepository categoryRepo,
        ITeamRepository teamRepo,
        IUserCompanyRoleRepository userCompanyRoleRepo,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _slaPolicyRepo = slaPolicyRepo;
        _categoryRepo = categoryRepo;
        _teamRepo = teamRepo;
        _userCompanyRoleRepo = userCompanyRoleRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Subject)) throw new ValidationException("Subject is required");

        var category = await _categoryRepo.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null) throw new NotFoundException("Category not found");
        if (category.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to use this category");

        var slaRule = await _slaPolicyRepo.GetReadRuleByIdAsync(request.SlaRuleId, cancellationToken);
        if (slaRule == null) throw new NotFoundException("SLA rule not found in current SLA Policy");
        if (!slaRule.IsPolicyActive) throw new BadRequestException("Sla rule is not in active Sla policy");
        if (slaRule.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to use this SLA rule");
        if (request.TeamId.HasValue)
        {
            var team = await _teamRepo.GetByIdAsync(request.TeamId.Value, cancellationToken);
            if (team == null) throw new NotFoundException("Team not found");
            if (team.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to assign to this team");

            var role = await _userCompanyRoleRepo.GetByUserIdAndCompanyIdAsync(request.RaiserId, request.CompanyId,
                cancellationToken);
            if (role == null || role.Role != AppRoles.CompanyAdmin)
                throw new ForbiddenException("Only company admin can assign team on ticket creation");
        }
        var ticket = new Ticket
        {
            CompanyId = request.CompanyId,
            RaiserId = request.RaiserId,
            CategoryId = request.CategoryId,
            SlaRuleId = request.SlaRuleId,
            Subject = request.Subject.Trim(),
            Body = request.Body,
            TeamId = request.TeamId,
            Status = TicketStatuses.Open,
            SlaFirstResponseMinutes = slaRule.FirstResponseMinutes,
            SlaResolutionMinutes = slaRule.ResolutionMinutes,
        };

        await _ticketRepository.AddAsync(ticket,cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
