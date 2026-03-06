using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAnnd.Application.Company;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Controllers;

[ApiController]
[Route("api/companies")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new CreateCompanyCommand(userId, request.Name), cancellationToken);
        return Created($"/api/companies/{result.CompanyId}", result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10, CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetCompaniesQuery(userId, page, size), cancellationToken);
        return Ok(result);
    }
    [HttpGet("current")]
    [Authorize(Roles = $"{nameof(AppRoles.CompanyAdmin)},{nameof(AppRoles.Agent)},{nameof(AppRoles.Customer)}")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new GetCompanyByIdQuery(companyId), cancellationToken);
        if (result == null)
            return NotFound();

        var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        return Ok(new { id = result.Id, name = result.Name, slug = result.Slug, role });
    }

    [HttpGet("members")]
    [Authorize]
    public async Task<IActionResult> GetMembers([FromQuery] int page = 1, [FromQuery] int size = 10, CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new GetCompanyMembersQuery(companyId, userId, page, size), cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }
    
}

public record CreateCompanyRequest(string Name);
