using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Services.Abstractions;
using BookingHack.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingHack.Core.Controllers;

[ApiController]
[Route("api/companies/{companyId:guid}/members")]
[Authorize(Policy = Policies.CompanyOwner)]
public class CompanyMembersController : ControllerBase
{
    private readonly ICompanyMemberService _service;

    public CompanyMembersController(ICompanyMemberService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid companyId)
    {
        var members = await _service.GetAllAsync(companyId);
        if (members is null) return NotFound();
        return Ok(members);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(Guid companyId, string userId)
    {
        var member = await _service.GetAsync(companyId, userId);
        return member is null ? NotFound() : Ok(member);
    }

    [HttpPost]
    public async Task<IActionResult> Add(Guid companyId, [FromBody] AddCompanyMemberRequest request)
    {
        var member = await _service.AddAsync(companyId, request);
        if (member is null) return NotFound();
        return CreatedAtAction(nameof(Get), new { companyId, userId = request.UserId }, member);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> Update(Guid companyId, string userId, [FromBody] UpdateCompanyMemberRequest request)
    {
        var member = await _service.UpdateAsync(companyId, userId, request);
        return member is null ? NotFound() : Ok(member);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(Guid companyId, string userId)
    {
        var deleted = await _service.DeleteAsync(companyId, userId);
        return deleted ? NoContent() : NotFound();
    }
}
