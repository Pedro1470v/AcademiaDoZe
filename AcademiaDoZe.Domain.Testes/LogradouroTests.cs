using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Tests;

public class LogradouroTests
{
    [Fact]
    public void Deve_Criar_Logradouro_Quando_Dados_Validos()
    {
        var result = Logradouro.Criar(
            "01001000",
            "Rua das Flores",
            "123",
            "Centro",
            "São Paulo",
            "SP"
        );

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void Deve_Falhar_Quando_Cep_For_Invalido()
    {
        var result = Logradouro.Criar(
            "123",
            "Rua das Flores",
            "123",
            "Centro",
            "São Paulo",
            "SP"
        );

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
    }
}