using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Company;
public record CreateCompanyCommand(Guid UserId, string Name) : IRequest<CreateCompanyResult>;

public record CreateCompanyResult(Guid CompanyId, string Name);
public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CreateCompanyResult>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserCompanyRoleRepository _userCompanyRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUserCompanyRoleRepository userCompanyRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _userCompanyRoleRepository = userCompanyRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCompanyResult> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = Domain.Entities.Company.Create(
            request.Name.Trim(),
            request.UserId
        );

        await _companyRepository.AddAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCompanyResult(company.Id, company.Name);
    }
}
