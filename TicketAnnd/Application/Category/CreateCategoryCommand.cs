using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TicketAnnd.Application.Category;

public record CreateCategoryCommand(Guid CompanyId, string Name) : IRequest<Guid>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new TicketAnnd.Domain.Entities.Categrory
        {
            Id = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            Name = request.Name
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        int rowAffect = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (rowAffect != 0)
        {
            await _mediator.Publish(new CategoryChangeNotification(request.CompanyId), cancellationToken);
        }

        return category.Id;
    }
}
