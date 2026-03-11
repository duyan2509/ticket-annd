using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAnnd.Application.Category;
using TicketAnnd.Domain.ReadModels;
using Microsoft.AspNetCore.OutputCaching;

namespace TicketAnnd.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Categories" })]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new GetCategoriesQuery(companyId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(TicketAnnd.Domain.Enums.AppRoles.CompanyAdmin))]
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Categories" })]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var id = await _mediator.Send(new CreateCategoryCommand(companyId, request.Name), cancellationToken);
        return Created($"/api/categories/{id}", new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = nameof(TicketAnnd.Domain.Enums.AppRoles.CompanyAdmin))]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        await _mediator.Send(new UpdateCategoryCommand(id, companyId, request.Name), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(TicketAnnd.Domain.Enums.AppRoles.CompanyAdmin))]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        await _mediator.Send(new DeleteCategoryCommand(id, companyId), cancellationToken);
        return NoContent();
    }
}

public record CreateCategoryRequest(string Name);
public record UpdateCategoryRequest(string Name);
