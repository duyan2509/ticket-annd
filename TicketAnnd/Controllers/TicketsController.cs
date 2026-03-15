using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAnnd.Application.Tickets;
using TicketAnnd.Domain.Repositories;
using TicketAnnd.Domain.Enums;
using Microsoft.AspNetCore.OutputCaching;

namespace TicketAnnd.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITeamRepository _teamRepository;

    public TicketsController(IMediator mediator, ITicketRepository ticketRepository, ITeamRepository teamRepository)
    {
        _mediator = mediator;
        _ticketRepository = ticketRepository;
        _teamRepository = teamRepository;
    }

    private bool TryGetCompanyId(out Guid companyId)
    {
        companyId = Guid.Empty;
        var companyIdClaim = User.FindFirstValue("company_id");
        return !string.IsNullOrEmpty(companyIdClaim) && Guid.TryParse(companyIdClaim, out companyId);
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return !string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out userId);
    }

    [HttpGet("by-code")]
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Tickets" })]
    public async Task<IActionResult> GetByCode([FromQuery] string? ticketCode, CancellationToken cancellationToken = default)
    {
        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new GetTicketByCodeQuery(companyId, ticketCode), cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? teamId = null, [FromQuery] string? status = null, [FromQuery] Guid? categoryId = null, [FromQuery] string? subject = null, [FromQuery] bool? unassigned = null, CancellationToken cancellationToken = default)
    {
        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new GetTicketsQuery(companyId, page, pageSize, teamId, status, categoryId, subject, unassigned), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new CreateTicketCommand(companyId, userId, request.CategoryId, request.SlaRuleId, request.Subject, request.Body, request.TeamId), cancellationToken);
        return Created($"/api/tickets/{result}", new { id = result });
    }
    [Authorize(Roles = $"{nameof(AppRoles.CompanyAdmin)}")]
    [HttpPost("{ticketId:guid}/team/{teamId:guid}")]
    public async Task<IActionResult> AssignTeam([FromRoute] Guid ticketId,[FromRoute] Guid teamId ,CancellationToken cancellationToken)
    {
        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var actorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        var result = await _mediator.Send(new AssignTeamCommand(companyId, ticketId, teamId, userId, actorEmail), cancellationToken);
        return Created($"/api/tickets/{result}", new { id = result });
    }

    [HttpPost("{ticketId:guid}/pause")]
    public async Task<IActionResult> Pause([FromRoute] Guid ticketId, [FromBody] PauseRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var actorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

        var result = await _mediator.Send(new PauseTicketCommand(companyId, ticketId, request.PauseType.ToString(), request.Reason, userId, actorEmail), cancellationToken);
        return Ok(new { id = result });
    }

    [HttpPost("{ticketId:guid}/continue")]
    public async Task<IActionResult> Continue([FromRoute] Guid ticketId, CancellationToken cancellationToken)
    {
        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var actorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

        var result = await _mediator.Send(new ContinueTicketCommand(companyId, ticketId, userId, actorEmail), cancellationToken);
        return Ok(new { id = result });
    }

    [HttpPost("{ticketId:guid}/resolve")]
    public async Task<IActionResult> Resolve([FromRoute] Guid ticketId, [FromBody] ResolveRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var actorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

        var result = await _mediator.Send(new ResolveTicketCommand(companyId, ticketId, userId, request.Note, actorEmail), cancellationToken);
        return Ok(new { id = result });
    }

    [HttpPost("{ticketId:guid}/start")]
    public async Task<IActionResult> Start([FromRoute] Guid ticketId, CancellationToken cancellationToken)
    {
        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var actorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

        var result = await _mediator.Send(new StartTicketCommand(companyId, ticketId, userId, actorEmail), cancellationToken);
        return Ok(new { id = result });
    }


    [HttpPost("{ticketId:guid}/member/{userId:guid}")]
    public async Task<IActionResult> AssignMember([FromRoute] Guid ticketId, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var actorClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(actorClaim) || !Guid.TryParse(actorClaim, out var actorId))
            return Unauthorized();

        var actorEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

        var result = await _mediator.Send(new AssignMemberToTicketCommand(companyId, ticketId, userId, actorId, actorEmail), cancellationToken);
        return Created($"/api/tickets/{result}", new { id = result });
    }

    [HttpGet("{ticketId:guid}/logs")]
    public async Task<IActionResult> GetLogs([FromRoute] Guid ticketId, [FromQuery] int page = 1, [FromQuery] int size = 50, CancellationToken cancellationToken = default)
    {
        if (!TryGetCompanyId(out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new GetTicketLogsQuery(companyId, ticketId, page, size), cancellationToken);
        return Ok(result);
    }
}

public record CreateTicketRequest(Guid CategoryId, Guid SlaRuleId, string Subject, string? Body, Guid? TeamId = null);

public record PauseRequest(TicketPauseType PauseType, string Reason);
public record ResolveRequest(string? Note);
