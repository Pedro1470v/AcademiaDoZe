//Pedro Vinicius
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class ArquivoTests
{
    [Fact]
    public void Deve_Criar_Arquivo_Quando_Conteudo_Valido()
    {
        byte[] conteudo = new byte[] { 1, 2, 3 };

        var result = Arquivo.Criar(conteudo);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(conteudo, result.Value!.Conteudo);
    }

    [Fact]
    public void Deve_Falhar_Quando_Conteudo_Nulo()
    {
        var result = Arquivo.Criar(null!);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
    }

    [Fact]
    public void Deve_Falhar_Quando_Arquivo_Ultrapassar_5MB()
    {
        byte[] conteudo = new byte[5 * 1024 * 1024 + 1];

        var result = Arquivo.Criar(conteudo);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
    }
}