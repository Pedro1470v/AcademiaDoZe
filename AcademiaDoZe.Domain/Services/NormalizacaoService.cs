//Pedro Vinicius
namespace AcademiaDoZe.Domain.Services;

public static class NormalizacaoService
{
    public static string NormalizarTexto(string valor)
    {
        return valor.Trim();
    }

    public static string ApenasDigitos(string valor)
    {
        return new string(valor.Where(char.IsDigit).ToArray());
    }
}