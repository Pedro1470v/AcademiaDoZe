//Pedro vinicius
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class SenhaTests
{
    [Theory]
    [InlineData("abcdef", false)]
    [InlineData("Abcdef", true)]
    public void Deve_Validar_RequisitoUppercase_Senha(
        string senha,
        bool esperado)
    {
        var result = Senha.Criar(senha);

        Assert.Equal(esperado, result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Deve_Falhar_Criacao_Quando_SenhaNulaOuVazia(
        string? input)
    {
        var result = Senha.Criar(input!);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "SENHA_OBRIGATORIO"
        );
    }
}