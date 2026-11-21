//Pedro Vinicius
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("emailinvalido")]
    [InlineData("email@")]
    [InlineData("@email.com")]
    public void Deve_Falhar_Criacao_Quando_EmailInvalido(string input)
    {
        var result = Email.Criar(input);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
    }

    [Theory]
    [InlineData("teste@email.com")]
    [InlineData("usuario@dominio.com.br")]
    public void Deve_Criar_Email_Quando_Valido(string input)
    {
        var result = Email.Criar(input);

        Assert.True(result.IsSuccess);
        Assert.Equal(input, result.Value!.Valor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Deve_Falhar_Criacao_Quando_EmailNuloOuVazio(string? input)
    {
        var result = Email.Criar(input!);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
    }
}