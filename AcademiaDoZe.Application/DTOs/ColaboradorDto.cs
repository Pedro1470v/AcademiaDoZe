//Pedro Vinicius 
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.DTOs;

public class ColaboradorDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string Telefone { get; set; } = string.Empty;

    public DateOnly DataNascimento { get; set; }

    public string? Senha { get; set; }

    public LogradouroDto? Endereco { get; set; } = new();

    public string Numero { get; set; } = string.Empty;

    public string? Complemento { get; set; }

    public DateOnly DataAdmissao { get; set; }

    public EColaboradorTipo Tipo { get; set; } = EColaboradorTipo.Atendente;

    public EColaboradorVinculo Vinculo { get; set; } = EColaboradorVinculo.CLT;

    public bool Ativo { get; set; } = true;

    public ArquivoDto? Foto { get; set; }
}
