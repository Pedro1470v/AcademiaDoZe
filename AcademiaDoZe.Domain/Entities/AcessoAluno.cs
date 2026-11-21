using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities
{
    public sealed class AcessoAluno : Entidade, IAggregateRoot
    {
        public Aluno Aluno { get; private set; }
        public DateTime DataHoraEntrada { get; private set; }
        public DateTime? DataHoraSaida { get; private set; }

        private AcessoAluno(
            int id,
            Aluno aluno,
            DateTime dataHoraEntrada,
            DateTime? dataHoraSaida)
            : base(id)
        {
            Aluno = aluno;
            DataHoraEntrada = dataHoraEntrada;
            DataHoraSaida = dataHoraSaida;
        }

        public static Result<AcessoAluno> Criar(
            int id,
            Aluno aluno,
            DateTime dataHoraEntrada,
            DateTime? dataHoraSaida = null)
        {
            var notifications = new List<Notification>();

            if (aluno == null)
            {
                notifications.Add(
                    new Notification(
                        "Aluno",
                        "ALUNO_OBRIGATORIO"));
            }

            if (dataHoraEntrada == default)
            {
                notifications.Add(
                    new Notification(
                        "DataHoraEntrada",
                        "DATA_HORA_ENTRADA_OBRIGATORIA"));
            }

            if (dataHoraSaida.HasValue &&
                dataHoraSaida < dataHoraEntrada)
            {
                notifications.Add(
                    new Notification(
                        "DataHoraSaida",
                        "DATA_HORA_SAIDA_INVALIDA"));
            }

            if (notifications.Count != 0)
            {
                return Result<AcessoAluno>.Failure(
                    notifications);
            }

            return Result<AcessoAluno>.Success(
                new AcessoAluno(
                    id,
                    aluno!,
                    dataHoraEntrada,
                    dataHoraSaida));
        }

        public Result<AcessoAluno> RegistrarSaida(
            DateTime dataHoraSaida)
        {
            if (dataHoraSaida < DataHoraEntrada)
            {
                return Result<AcessoAluno>.Failure(
                    "DataHoraSaida",
                    "DATA_HORA_SAIDA_INVALIDA");
            }

            DataHoraSaida = dataHoraSaida;

            return Result<AcessoAluno>.Success(this);
        }
    }
}