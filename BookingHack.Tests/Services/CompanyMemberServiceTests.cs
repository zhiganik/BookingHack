using System.Security.Claims;
using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Repositories;
using BookingHack.Application.Services;
using BookingHack.Domain.Constants;
using BookingHack.Domain.Enums;
using BookingHack.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace BookingHack.Tests.Services;

[TestFixture]
public class CompanyMemberServiceTests
{
    private ICompanyRepository _repository = null!;
    private UserManager<ApplicationUser> _userManager = null!;
    private CompanyMemberService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<ICompanyRepository>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        _service = new CompanyMemberService(_repository, _userManager);
    }

    [TearDown]
    public void TearDown() => _userManager.Dispose();

    private static Company CompanyWithMember(string userId, CompanyRole role)
    {
        var company = new Company("Acme", "IT", "Addr");
        company.AddMember(userId, role);

        var member = company.Members.First();
        var user = new ApplicationUser { Id = userId, Email = "member@test.com", FirstName = "John", LastName = "Doe" };
        typeof(CompanyMember).GetProperty(nameof(CompanyMember.User))!.SetValue(member, user);

        return company;
    }

    [Test]
    public async Task GetAllAsync_CompanyNotFound_ReturnsNull()
    {
        _repository.GetByIdWithMembersAsync(Arg.Any<Guid>()).Returns((Company?)null);

        var result = await _service.GetAllAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Test]
    public async Task GetAsync_MemberNotFound_ReturnsNull()
    {
        var company = new Company("Acme", "IT", "Addr");
        _repository.GetByIdWithMembersAsync(company.Id).Returns(company);

        var result = await _service.GetAsync(company.Id, Guid.NewGuid().ToString());

        result.Should().BeNull();
    }

    [Test]
    public async Task AddAsync_CompanyNotFound_ReturnsNull()
    {
        _repository.GetByIdWithMembersAsync(Arg.Any<Guid>()).Returns((Company?)null);

        var result = await _service.AddAsync(Guid.NewGuid(), new AddCompanyMemberRequest(Guid.NewGuid().ToString(), CompanyRole.Manager));

        result.Should().BeNull();
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Company>());
    }

    [Test]
    public async Task AddAsync_ValidRequest_PersistsMemberAndAddsClaim()
    {
        var userId = Guid.NewGuid().ToString();
        var company = new Company("Acme", "IT", "Addr");
        var user = new ApplicationUser { Id = userId, Email = "new@test.com", FirstName = "Jane", LastName = "Smith" };

        _repository.GetByIdWithMembersAsync(company.Id).Returns(company);
        _userManager.FindByIdAsync(userId).Returns(user);
        _userManager.AddClaimAsync(user, Arg.Any<Claim>()).Returns(IdentityResult.Success);

        var result = await _service.AddAsync(company.Id, new AddCompanyMemberRequest(userId, CompanyRole.Manager));

        result.Should().NotBeNull();
        result!.Role.Should().Be(CompanyRole.Manager);
        await _repository.Received(1).UpdateAsync(Arg.Is<Company>(c => c.Members.Any(m => m.UserId == userId)));
        await _userManager.Received(1).AddClaimAsync(user,
            Arg.Is<Claim>(c => c.Type == Claims.CompanyRole && c.Value == nameof(CompanyRole.Manager)));
    }

    [Test]
    public async Task UpdateAsync_ValidRequest_SyncsClaimsOnRoleChange()
    {
        var userId = Guid.NewGuid().ToString();
        var company = CompanyWithMember(userId, CompanyRole.Manager);
        var user = new ApplicationUser { Id = userId };

        _repository.GetByIdWithMembersAsync(company.Id).Returns(company);
        _userManager.FindByIdAsync(userId).Returns(user);
        _userManager.RemoveClaimAsync(user, Arg.Any<Claim>()).Returns(IdentityResult.Success);
        _userManager.AddClaimAsync(user, Arg.Any<Claim>()).Returns(IdentityResult.Success);

        await _service.UpdateAsync(company.Id, userId, new UpdateCompanyMemberRequest(CompanyRole.Owner));

        await _userManager.Received(1).RemoveClaimAsync(user,
            Arg.Is<Claim>(c => c.Type == Claims.CompanyRole && c.Value == nameof(CompanyRole.Manager)));
        await _userManager.Received(1).AddClaimAsync(user,
            Arg.Is<Claim>(c => c.Type == Claims.CompanyRole && c.Value == nameof(CompanyRole.Owner)));
    }

    [Test]
    public async Task DeleteAsync_ValidMember_RemovesFromCompanyAndDeletesClaim()
    {
        var userId = Guid.NewGuid().ToString();
        var company = CompanyWithMember(userId, CompanyRole.Manager);
        var user = new ApplicationUser { Id = userId };

        _repository.GetByIdWithMembersAsync(company.Id).Returns(company);
        _userManager.FindByIdAsync(userId).Returns(user);
        _userManager.RemoveClaimAsync(user, Arg.Any<Claim>()).Returns(IdentityResult.Success);

        var result = await _service.DeleteAsync(company.Id, userId);

        result.Should().BeTrue();
        await _repository.Received(1).UpdateAsync(Arg.Is<Company>(c => !c.Members.Any(m => m.UserId == userId)));
        await _userManager.Received(1).RemoveClaimAsync(user,
            Arg.Is<Claim>(c => c.Type == Claims.CompanyRole && c.Value == nameof(CompanyRole.Manager)));
    }

    [Test]
    public async Task DeleteAsync_CompanyNotFound_ReturnsFalse()
    {
        _repository.GetByIdWithMembersAsync(Arg.Any<Guid>()).Returns((Company?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid().ToString());

        result.Should().BeFalse();
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Company>());
    }
}
