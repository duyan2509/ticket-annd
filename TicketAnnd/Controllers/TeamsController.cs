using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAnnd.Application.Teams;
using TicketAnnd.Domain.Enums;
using Microsoft.AspNetCore.OutputCaching;

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

    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    [HttpPut("{teamId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid teamId, [FromBody] UpdateTeamRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized();

        await _mediator.Send(new UpdateTeamCommand(teamId, companyId, request.Name), cancellationToken);
        return NoContent();
    }
    [HttpGet]
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Teams" })]
    public async Task<IActionResult> GetByCompany(CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized();
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
        await _mediator.Send(new AssignMemberCommand(companyId, teamId, request.UserId), cancellationToken);
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
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Teams" })]
    public async Task<IActionResult> GetMembers([FromRoute] Guid teamId, [FromQuery] int page = 1, [FromQuery] int size = 10, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetTeamMembersQuery(teamId, page, size), cancellationToken);
        return Ok(result);
    }

    public record CreateTeamRequest(string Name);
    public record UpdateTeamRequest(string Name);
    public record SetLeaderRequest(Guid UserId);
    public record AssignMemberRequest(Guid UserId);
    public record SwitchMemberRequest(Guid UserId, Guid ToTeamId);
}
