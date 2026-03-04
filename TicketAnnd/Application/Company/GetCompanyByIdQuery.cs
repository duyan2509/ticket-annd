using MediatR;
using TicketAnnd.Domain.Repositories;

namespace TicketAnnd.Application.Company;

public record CompanyCurrentDto(Guid Id, string Name, string Slug);

public record GetCompanyByIdQuery(Guid CompanyId) : IRequest<CompanyCurrentDto?>;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, CompanyCurrentDto?>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyByIdQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyCurrentDto?> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var read = await _companyRepository.GetReadModelByIdAsync(request.CompanyId, cancellationToken);
        if (read == null) return null;

        return new CompanyCurrentDto(read.Id, read.Name, read.Slug ?? string.Empty);
    }
}
