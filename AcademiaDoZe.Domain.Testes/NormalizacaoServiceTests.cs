//Pedro vinicius
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.Tests;

public class NormalizacaoServiceTests
{
    [Fact]
    public void Deve_Normalizar_Texto()
    {
        var resultado = NormalizacaoService.NormalizarTexto("  Pedro  ");

        Assert.Equal("Pedro", resultado);
    }

    [Fact]
    public void Deve_Remover_Caracteres_Nao_Numericos()
    {
        var resultado = NormalizacaoService.ApenasDigitos("(11) 91234-5678");

        Assert.Equal("11912345678", resultado);
    }
}