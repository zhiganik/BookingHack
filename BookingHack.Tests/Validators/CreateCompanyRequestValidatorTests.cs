using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Validators;
using FluentAssertions;
using FluentValidation;

namespace BookingHack.Tests.Validators;

[TestFixture]
public class CreateCompanyRequestValidatorTests
{
    private IValidator<CreateCompanyRequest> _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateCompanyRequestValidator();

    private static CreateCompanyRequest ValidRequest() =>
        new("Acme Corp", "IT Services", "123 Main St", null);

    [Test]
    public async Task Validate_ValidRequest_Succeeds()
    {
        var result = await _validator.ValidateAsync(ValidRequest());

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task Validate_ValidRequestWithDescription_Succeeds()
    {
        var request = ValidRequest() with { Description = "A great company" };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    // --- Name ---

    [TestCase("")]
    [TestCase("   ")]
    public async Task Validate_EmptyName_Fails(string name)
    {
        var request = ValidRequest() with { Name = name };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCompanyRequest.Name));
    }

    [Test]
    public async Task Validate_NameAtMaxLength_Succeeds()
    {
        var request = ValidRequest() with { Name = new string('A', 200) };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task Validate_NameExceedsMaxLength_Fails()
    {
        var request = ValidRequest() with { Name = new string('A', 201) };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCompanyRequest.Name));
    }

    // --- TypeOfService ---

    [TestCase("")]
    [TestCase("   ")]
    public async Task Validate_EmptyTypeOfService_Fails(string typeOfService)
    {
        var request = ValidRequest() with { TypeOfService = typeOfService };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCompanyRequest.TypeOfService));
    }

    [Test]
    public async Task Validate_TypeOfServiceExceedsMaxLength_Fails()
    {
        var request = ValidRequest() with { TypeOfService = new string('A', 101) };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCompanyRequest.TypeOfService));
    }

    // --- Address ---

    [TestCase("")]
    [TestCase("   ")]
    public async Task Validate_EmptyAddress_Fails(string address)
    {
        var request = ValidRequest() with { Address = address };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCompanyRequest.Address));
    }

    [Test]
    public async Task Validate_AddressExceedsMaxLength_Fails()
    {
        var request = ValidRequest() with { Address = new string('A', 501) };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCompanyRequest.Address));
    }

    // --- Description ---

    [Test]
    public async Task Validate_NullDescription_Succeeds()
    {
        var request = ValidRequest() with { Description = null };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task Validate_DescriptionAtMaxLength_Succeeds()
    {
        var request = ValidRequest() with { Description = new string('A', 1000) };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task Validate_DescriptionExceedsMaxLength_Fails()
    {
        var request = ValidRequest() with { Description = new string('A', 1001) };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCompanyRequest.Description));
    }
}
