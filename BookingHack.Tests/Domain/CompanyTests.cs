using BookingHack.Domain.Models;
using FluentAssertions;

namespace BookingHack.Tests.Domain;

[TestFixture]
public class CompanyTests
{
    [Test]
    public void Constructor_SetsAllProperties()
    {
        var before = DateTime.UtcNow;

        var company = new Company("Acme", "IT Services", "123 Main St", "A great company");

        company.Id.Should().NotBeEmpty();
        company.Name.Should().Be("Acme");
        company.TypeOfService.Should().Be("IT Services");
        company.Address.Should().Be("123 Main St");
        company.Description.Should().Be("A great company");
        company.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Test]
    public void Constructor_WithNullDescription_SetsDescriptionToNull()
    {
        var company = new Company("Acme", "IT", "Addr");

        company.Description.Should().BeNull();
    }

    [Test]
    public void Constructor_EachInstanceGetsUniqueId()
    {
        var company1 = new Company("A", "B", "C");
        var company2 = new Company("A", "B", "C");

        company1.Id.Should().NotBe(company2.Id);
    }

    [Test]
    public void Update_ChangesAllMutableProperties()
    {
        var company = new Company("Old Name", "Old Type", "Old Addr", "Old Desc");

        company.Update("New Name", "New Type", "New Addr", "New Desc");

        company.Name.Should().Be("New Name");
        company.TypeOfService.Should().Be("New Type");
        company.Address.Should().Be("New Addr");
        company.Description.Should().Be("New Desc");
    }

    [Test]
    public void Update_CanClearDescription()
    {
        var company = new Company("Name", "Type", "Addr", "Some desc");

        company.Update("Name", "Type", "Addr", null);

        company.Description.Should().BeNull();
    }

    [Test]
    public void Update_DoesNotChangeIdOrCreatedAt()
    {
        var company = new Company("Name", "Type", "Addr");
        var originalId = company.Id;
        var originalCreatedAt = company.CreatedAt;

        company.Update("New Name", "New Type", "New Addr", null);

        company.Id.Should().Be(originalId);
        company.CreatedAt.Should().Be(originalCreatedAt);
    }
}
