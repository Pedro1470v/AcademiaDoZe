
using System;

namespace AcademiaDoZe.Application.DTOs;

public class AcessoColaboradorDto
{
    public int Id { get; set; }

    public int ColaboradorId { get; set; }

    public string NomeColaborador { get; set; } = string.Empty;

    public string CpfColaborador { get; set; } = string.Empty;

    public string TipoColaborador { get; set; } = string.Empty;

    public string VinculoColaborador { get; set; } = string.Empty;

    public DateTime DataHoraEntrada { get; set; }

    public DateTime? DataHoraSaida { get; set; }

    public TimeSpan TempoPermanencia { get; set; }

    public bool EstaEmAberto => !DataHoraSaida.HasValue;
}
