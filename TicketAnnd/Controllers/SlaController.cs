using System.Security.Claims;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAnnd.Application.Sla;
using TicketAnnd.Domain.Enums;
using TicketAnnd.Domain.ReadModels;
using Microsoft.AspNetCore.OutputCaching;

namespace TicketAnnd.Controllers;

[ApiController]
[Route("api/sla")]
[Authorize]
public class SlaController : ControllerBase
{
    private readonly IMediator _mediator;

    public SlaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("rules")]
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Sla" })]
    public async Task<IActionResult> GetRulesForActivePolicy(CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var rules = await _mediator.Send(new GetSlaRulesForActivePolicyQuery(companyId), cancellationToken);
        if (rules == null || !rules.Any()) return NotFound("Active SLA rules not found");
        return Ok(rules);
    }

    [HttpGet("policies")]
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Sla" })]
    public async Task<IActionResult> GetPolicies([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var result = await _mediator.Send(new GetSlaPoliciesQuery(companyId, page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpPost("policies")]
    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    public async Task<IActionResult> CreatePolicy([FromBody] CreateSlaPolicyRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var id = await _mediator.Send(new CreateSlaPolicyCommand(companyId, request.Name), cancellationToken);
        return Created($"/api/sla/policies/{id}", new { id });
    }

    [HttpPut("policies/{id:guid}")]
    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    public async Task<IActionResult> UpdatePolicy([FromRoute] Guid id, [FromBody] UpdateSlaPolicyRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        await _mediator.Send(new UpdateSlaPolicyCommand(id, companyId, request.Name), cancellationToken);
        return NoContent();
    }

    [HttpPost("policies/{id:guid}/activate")]
    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    public async Task<IActionResult> ActivatePolicy([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        await _mediator.Send(new ActivateSlaPolicyCommand(id, companyId), cancellationToken);
        return NoContent();
    }

    [HttpGet("policies/{id:guid}/rules")]
    [OutputCache(PolicyName = "CompCache", Tags = new[] { "Sla" })]
    public async Task<IActionResult> GetRules([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSlaRulesByPolicyQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("policies/{id:guid}/rules")]
    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    public async Task<IActionResult> CreateRule([FromRoute] Guid id, [FromBody] CreateSlaRuleRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        var ruleId = await _mediator.Send(new CreateSlaRuleCommand(id, companyId, request.Name, request.FirstResponseMinutes, request.ResolutionMinutes), cancellationToken);
        return Created($"/api/sla/policies/{id}/rules/{ruleId}", new { id = ruleId });
    }

    [HttpPut("policies/{policyId:guid}/rules/{ruleId:guid}")]
    [Authorize(Roles = nameof(AppRoles.CompanyAdmin))]
    public async Task<IActionResult> UpdateRule([FromRoute] Guid policyId, [FromRoute] Guid ruleId, [FromBody] UpdateSlaRuleRequest request, CancellationToken cancellationToken)
    {
        var companyIdClaim = User.FindFirstValue("company_id");
        if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            return Unauthorized("Not allow to access without company context.");

        await _mediator.Send(new UpdateSlaRuleCommand(ruleId, companyId, request.Name, request.FirstResponseMinutes, request.ResolutionMinutes), cancellationToken);
        return NoContent();
    }
}

public record CreateSlaPolicyRequest(string Name);
public record UpdateSlaPolicyRequest(string Name);
public record CreateSlaRuleRequest(int FirstResponseMinutes, int ResolutionMinutes, string Name);
public record UpdateSlaRuleRequest(int? FirstResponseMinutes, int? ResolutionMinutes, string? Name);
