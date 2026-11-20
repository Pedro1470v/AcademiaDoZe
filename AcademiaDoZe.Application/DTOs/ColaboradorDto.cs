
// Pedro Vinicius

using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.DTOs;

public class ColaboradorDto : PessoaDto
{
    public DateOnly DataAdmissao { get; set; } =
        DateOnly.FromDateTime(DateTime.Today);

    public EColaboradorTipo Tipo { get; set; } =
        EColaboradorTipo.Atendente;

    public EColaboradorVinculo Vinculo { get; set; } =
        EColaboradorVinculo.CLT;

    public bool Ativo { get; set; } = true;
}
