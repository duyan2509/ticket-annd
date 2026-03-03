using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Entities;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Company;

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
        var company = new Domain.Entities.Company
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim()
        };
        await _companyRepository.AddAsync(company, cancellationToken);

        var userCompanyRole = new Domain.Entities.UserCompanyRole
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            CompanyId = company.Id,
            Role = AppRoles.CompanyAdmin
        };
        await _userCompanyRoleRepository.AddAsync(userCompanyRole, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateCompanyResult(company.Id, company.Name);
    }
}
