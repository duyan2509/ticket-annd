using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAnnd.Application.Invitation;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Controllers;

[ApiController]
[Route("api/invitations")]
[Authorize]
public class InvitationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvitationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(AppRoles.CompanyAdmin)}")]
    public async Task<IActionResult> Create([FromBody] CreateInvitationRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var role = request.Role;
        var result = await _mediator.Send(new CreateInvitationCommand(companyId, request.Email, role), cancellationToken);
        return Created($"/api/invitations/{result.InvitationId}", result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByEmail([FromQuery] string? status = null, [FromQuery] string? companyName = null, [FromQuery] int page = 1, [FromQuery] int size = 50, CancellationToken cancellationToken = default)
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        if (string.IsNullOrEmpty(email))
            return Unauthorized();

        InviationStatuses? st = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<InviationStatuses>(status, true, out var parsed))
            st = parsed;

        var items = await _mediator.Send(new GetInvitationsByEmailQuery(email, st, companyName, page, size), cancellationToken);
        return Ok(items);
    }
}

public record CreateInvitationRequest(string Email, AppRoles Role);
