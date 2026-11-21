
// Pedro Vinicius

using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities;

public class Acesso : Entidade, IAggregateRoot
{
    public int? AlunoId { get; private set; }

    public int? ColaboradorId { get; private set; }

    public DateTime DataHoraEntrada { get; private set; }

    public DateTime? DataHoraSaida { get; private set; }

    public bool EstaAberto => DataHoraSaida == null;

    public TimeSpan? TempoPermanencia =>
        DataHoraSaida.HasValue
            ? DataHoraSaida.Value - DataHoraEntrada
            : null;

    private Acesso(
        int id,
        int? alunoId,
        int? colaboradorId,
        DateTime dataHoraEntrada,
        DateTime? dataHoraSaida)
        : base(id)
    {
        AlunoId = alunoId;
        ColaboradorId = colaboradorId;
        DataHoraEntrada = dataHoraEntrada;
        DataHoraSaida = dataHoraSaida;
    }

    public static Result<Acesso> Criar(
        int id,
        int? alunoId,
        int? colaboradorId,
        DateTime dataHoraEntrada)
    {
        var notifications = new List<Notification>();

        bool alunoInformado = alunoId.HasValue;
        bool colaboradorInformado = colaboradorId.HasValue;

        if (alunoInformado == colaboradorInformado)
        {
            notifications.Add(
                new Notification(
                    "Pessoa",
                    "INFORME_EXATAMENTE_UM_ALUNO_OU_COLABORADOR"));
        }

        if (alunoId.HasValue && alunoId.Value <= 0)
        {
            notifications.Add(
                new Notification(
                    "AlunoId",
                    "ID_ALUNO_INVALIDO"));
        }

        if (colaboradorId.HasValue && colaboradorId.Value <= 0)
        {
            notifications.Add(
                new Notification(
                    "ColaboradorId",
                    "ID_COLABORADOR_INVALIDO"));
        }

        if (dataHoraEntrada == default)
        {
            notifications.Add(
                new Notification(
                    "DataHoraEntrada",
                    "DATA_HORA_ENTRADA_OBRIGATORIA"));
        }

        if (notifications.Count > 0)
        {
            return Result<Acesso>.Failure(notifications);
        }

        var acesso = new Acesso(
            id,
            alunoId,
            colaboradorId,
            dataHoraEntrada,
            null);

        return Result<Acesso>.Success(acesso);
    }

    public Result<Acesso> RegistrarSaida(DateTime dataHoraSaida)
    {
        var notifications = new List<Notification>();

        if (DataHoraSaida.HasValue)
        {
            notifications.Add(
                new Notification(
                    "DataHoraSaida",
                    "SAIDA_JA_REGISTRADA"));
        }

        if (dataHoraSaida == default)
        {
            notifications.Add(
                new Notification(
                    "DataHoraSaida",
                    "DATA_HORA_SAIDA_OBRIGATORIA"));
        }

        if (dataHoraSaida < DataHoraEntrada)
        {
            notifications.Add(
                new Notification(
                    "DataHoraSaida",
                    "SAIDA_NAO_PODE_SER_ANTERIOR_A_ENTRADA"));
        }

        if (notifications.Count > 0)
        {
            return Result<Acesso>.Failure(notifications);
        }

        DataHoraSaida = dataHoraSaida;

        return Result<Acesso>.Success(this);
    }
}
