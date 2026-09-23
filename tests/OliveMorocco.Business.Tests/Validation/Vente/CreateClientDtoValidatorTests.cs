using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Validation.Vente;

namespace OliveMorocco.Business.Tests.Validation.Vente;

[TestClass]
public sealed class CreateClientDtoValidatorTests
{
    private readonly CreateClientDtoValidator _validator = new();

    [TestMethod]
    public void Valid_dto_passes()
    {
        var dto = new CreateClientDto("Acme", "", "", "", "", "", "", true);
        var result = _validator.Validate(dto);
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Empty_nom_fails()
    {
        var dto = new CreateClientDto("", "", "", "", "", "", "", true);
        var result = _validator.Validate(dto);
        Assert.IsFalse(result.IsValid);
        StringAssert.Contains(result.Errors[0].ErrorMessage, "nom est obligatoire");
    }

    [TestMethod]
    public void Invalid_email_fails()
    {
        var dto = new CreateClientDto("Acme", "", "", "", "not-an-email", "", "", true);
        var result = _validator.Validate(dto);
        Assert.IsFalse(result.IsValid);
        StringAssert.Contains(result.Errors[0].ErrorMessage, "e-mail");
    }

    [TestMethod]
    public void Ice_wrong_length_fails()
    {
        var dto = new CreateClientDto("Acme", "", "", "", "", "123", "", true);
        var result = _validator.Validate(dto);
        Assert.IsFalse(result.IsValid);
        StringAssert.Contains(result.Errors[0].ErrorMessage, "15 caractères");
    }

    [TestMethod]
    public void Ice_15_chars_passes()
    {
        var dto = new CreateClientDto("Acme", "", "", "", "", "123456789123456", "", true);
        var result = _validator.Validate(dto);
        Assert.IsTrue(result.IsValid);
    }
}
