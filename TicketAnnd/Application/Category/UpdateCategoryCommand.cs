using MediatR;
using TicketAnnd.Domain;
using TicketAnnd.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TicketAnnd.Application.Category;

public record UpdateCategoryCommand(Guid CategoryId, Guid CompanyId, string Name) : IRequest<Unit>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand,Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existing = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (existing == null) throw new NotFoundException("Category not found");
        if (existing.CompanyId != request.CompanyId) throw new ForbiddenException("Not allowed to update this category");

        existing.Name = request.Name;

        await _categoryRepository.UpdateAsync(existing, cancellationToken);
        int rowAffect = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (rowAffect != 0)
        {
            await _mediator.Publish(new CategoryChangeNotification(request.CompanyId), cancellationToken);
        }

        return Unit.Value;
    }
}
