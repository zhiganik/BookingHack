using BookingHack.Application.Contracts.Requests;
using BookingHack.Application.Validators;
using BookingHack.Domain.Enums;
using FluentAssertions;
using FluentValidation;

namespace BookingHack.Tests.Validators;

[TestFixture]
public class RegisterRequestValidatorTests
{
    private IValidator<RegisterRequest> _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new RegisterRequestValidator();

    private static RegisterRequest ValidRequest() =>
        new("user@example.com", "Password1", "John", "Doe", RegisterRole.Customer);

    [Test]
    public async Task Validate_ValidRequest_Succeeds()
    {
        var result = await _validator.ValidateAsync(ValidRequest());

        result.IsValid.Should().BeTrue();
    }

    // --- Email ---

    [Test]
    public async Task Validate_EmptyEmail_Fails()
    {
        var request = ValidRequest() with { Email = "" };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterRequest.Email));
    }

    [TestCase("notanemail")]
    [TestCase("missing@")]
    [TestCase("@domain.com")]
    public async Task Validate_MalformedEmail_Fails(string email)
    {
        var request = ValidRequest() with { Email = email };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterRequest.Email));
    }

    // --- Password ---

    [Test]
    public async Task Validate_EmptyPassword_Fails()
    {
        var request = ValidRequest() with { Password = "" };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterRequest.Password));
    }

    [Test]
    public async Task Validate_PasswordBelowMinLength_Fails()
    {
        var request = ValidRequest() with { Password = "Ab1" };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterRequest.Password));
    }

    [Test]
    public async Task Validate_PasswordWithoutDigit_Fails()
    {
        var request = ValidRequest() with { Password = "NoDigitHere" };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(RegisterRequest.Password) &&
            e.ErrorMessage == "Password must contain at least one digit.");
    }

    [TestCase("pass1word")]
    [TestCase("123456")]
    public async Task Validate_ValidPassword_Succeeds(string password)
    {
        var request = ValidRequest() with { Password = password };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    // --- FirstName ---

    [Test]
    public async Task Validate_EmptyFirstName_Fails()
    {
        var request = ValidRequest() with { FirstName = "" };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterRequest.FirstName));
    }

    [Test]
    public async Task Validate_FirstNameExceedsMaxLength_Fails()
    {
        var request = ValidRequest() with { FirstName = new string('A', 101) };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterRequest.FirstName));
    }

    // --- LastName ---

    [Test]
    public async Task Validate_EmptyLastName_Fails()
    {
        var request = ValidRequest() with { LastName = "" };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterRequest.LastName));
    }

    [Test]
    public async Task Validate_LastNameExceedsMaxLength_Fails()
    {
        var request = ValidRequest() with { LastName = new string('A', 101) };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterRequest.LastName));
    }

    // --- Role ---

    [TestCase(RegisterRole.Customer)]
    [TestCase(RegisterRole.Company)]
    public async Task Validate_ValidRole_Succeeds(RegisterRole role)
    {
        var request = ValidRequest() with { Role = role };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public async Task Validate_InvalidRole_Fails()
    {
        var request = ValidRequest() with { Role = (RegisterRole)99 };
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterRequest.Role));
    }
}
