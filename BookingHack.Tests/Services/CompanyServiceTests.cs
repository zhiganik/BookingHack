using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Repositories;
using BookingHack.Application.Services;
using BookingHack.Domain.Models;
using FluentAssertions;
using NSubstitute;

namespace BookingHack.Tests.Services;

[TestFixture]
public class CompanyServiceTests
{
    private ICompanyRepository _repository = null!;
    private CompanyService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<ICompanyRepository>();
        _service = new CompanyService(_repository);
    }

    [Test]
    public async Task GetAllAsync_ReturnsResponseForEachCompany()
    {
        var companies = new[]
        {
            new Company("Acme", "IT", "Addr1"),
            new Company("Beta", "Finance", "Addr2")
        };
        _repository.GetAllAsync().Returns(companies);

        var result = (await _service.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result.Select(r => r.Id).Should().BeEquivalentTo(companies.Select(c => c.Id));
    }

    [Test]
    public async Task GetAllAsync_WhenNoCompanies_ReturnsEmpty()
    {
        _repository.GetAllAsync().Returns(Array.Empty<Company>());

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetByIdAsync_ExistingId_ReturnsMatchingResponse()
    {
        var company = new Company("Acme", "IT", "123 Main St");
        _repository.GetByIdAsync(company.Id).Returns(company);

        var result = await _service.GetByIdAsync(company.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
    }

    [Test]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id).Returns((Company?)null);

        var result = await _service.GetByIdAsync(id);

        result.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_PersistsCompanyWithCorrectData()
    {
        var request = new CreateCompanyRequest("Acme", "IT", "123 Main St", "Desc");
        _repository.AddAsync(Arg.Any<Company>()).Returns(x => x.Arg<Company>());

        var result = await _service.CreateAsync(request);

        result.Id.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Is<Company>(c =>
            c.Name == request.Name &&
            c.TypeOfService == request.TypeOfService &&
            c.Address == request.Address &&
            c.Description == request.Description));
    }

    [Test]
    public async Task UpdateAsync_ExistingId_AppliesChangesAndReturnsResponse()
    {
        var company = new Company("Old Name", "Old Type", "Old Addr");
        var request = new UpdateCompanyRequest("New Name", "New Type", "New Addr", "New Desc");
        _repository.GetByIdAsync(company.Id).Returns(company);

        var result = await _service.UpdateAsync(company.Id, request);

        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
        await _repository.Received(1).UpdateAsync(Arg.Is<Company>(c =>
            c.Name == "New Name" &&
            c.TypeOfService == "New Type" &&
            c.Address == "New Addr" &&
            c.Description == "New Desc"));
    }

    [Test]
    public async Task UpdateAsync_NonExistingId_ReturnsNullWithoutCallingUpdate()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id).Returns((Company?)null);

        var result = await _service.UpdateAsync(id, new UpdateCompanyRequest("Name", "Type", "Addr", null));

        result.Should().BeNull();
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Company>());
    }

    [Test]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        _repository.DeleteAsync(id).Returns(true);

        var result = await _service.DeleteAsync(id);

        result.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(id);
    }

    [Test]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _repository.DeleteAsync(id).Returns(false);

        var result = await _service.DeleteAsync(id);

        result.Should().BeFalse();
    }
}
