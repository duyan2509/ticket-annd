using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAnnd.Application.Teams;
using TicketAnnd.Domain.Enums;

namespace TicketAnnd.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeamRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized();

        var result = await _mediator.Send(new CreateTeamCommand(companyId, request.Name), cancellationToken);
        return Created($"/api/teams/{result.TeamId}", result);
    }
    [HttpGet]
    public async Task<IActionResult> GetByCompany([FromQuery] Guid companyId, CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetTeamsByCompanyQuery(companyId), cancellationToken);
        return Ok(items);
    }
    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    [HttpPost("{teamId:guid}/members")]
    public async Task<IActionResult> AssignMember([FromRoute] Guid teamId, [FromBody] AssignMemberRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized();
        await _mediator.Send(new AssignMemberCommand(companyId,teamId, request.UserId), cancellationToken);
        return NoContent();
    }
    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    [HttpPost("{teamId:guid}/members/switch")]
    public async Task<IActionResult> SwitchMember([FromRoute] Guid teamId, [FromBody] SwitchMemberRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized();
        await _mediator.Send(new SwitchMemberCommand(companyId, request.UserId, teamId, request.ToTeamId), cancellationToken);
        return NoContent();
    }
    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    [HttpPost("{teamId:guid}/leader")]
    public async Task<IActionResult> SetLeader([FromRoute] Guid teamId, [FromBody] SetLeaderRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized();

        await _mediator.Send(new SetTeamLeaderCommand(companyId, teamId, request.UserId), cancellationToken);
        return NoContent();
    }


    [HttpGet("{teamId:guid}/members")]
    public async Task<IActionResult> GetMembers([FromRoute] Guid teamId, [FromQuery] int page = 1, [FromQuery] int size = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetTeamMembersQuery(teamId, page, size), cancellationToken);
        return Ok(result);
    }

    public record CreateTeamRequest(string Name);
    public record SetLeaderRequest(Guid UserId);
    public record AssignMemberRequest(Guid UserId);
    public record SwitchMemberRequest(Guid UserId, Guid ToTeamId);
}
