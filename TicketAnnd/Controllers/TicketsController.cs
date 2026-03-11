using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAnnd.Application.Tickets;
using TicketAnnd.Domain.Enums;
using Microsoft.AspNetCore.OutputCaching;

namespace TicketAnnd.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("by-code")]
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Tickets" })]
    public async Task<IActionResult> GetByCode([FromQuery] string? ticketCode, CancellationToken cancellationToken = default)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new GetTicketByCodeQuery(companyId, ticketCode), cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Tickets" })]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? teamId = null, [FromQuery] string? status = null, [FromQuery] Guid? categoryId = null, [FromQuery] string? subject = null, CancellationToken cancellationToken = default)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new GetTicketsQuery(companyId, page, pageSize, teamId, status, categoryId, subject), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new CreateTicketCommand(companyId, userId, request.CategoryId, request.SlaRuleId, request.Subject, request.Body, request.TeamId), cancellationToken);
        return Created($"/api/tickets/{result}", new { id = result });
    }
    [Authorize(Roles = $"{nameof(AppRoles.CompanyAdmin)}")]
    [HttpPost("{ticketId:guid}/team/{teamId:guid}")]
    public async Task<IActionResult> AssignTeam([FromRoute] Guid ticketId,[FromRoute] Guid teamId ,CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new AssignTeamCommand(companyId, ticketId, teamId), cancellationToken);
        return Created($"/api/tickets/{result}", new { id = result });
    }
}

public record CreateTicketRequest(Guid CategoryId, Guid SlaRuleId, string Subject, string? Body, Guid? TeamId = null);
