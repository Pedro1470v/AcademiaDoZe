// Pedro Vinicius

namespace AcademiaDoZe.Application.DTOs;

public class ColaboradorDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public bool Ativo { get; set; }
}