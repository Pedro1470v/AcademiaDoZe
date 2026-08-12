//Pedro Vinicius
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities
{
    internal sealed class AcessoColaborador : Entidade
    {
        public Colaborador Colaborador { get; private set; }
        public DateTime DataHoraEntrada { get; private set; }
        public DateTime? DataHoraSaida { get; private set; }

        private AcessoColaborador(
            int id,
            Colaborador colaborador,
            DateTime dataHoraEntrada,
            DateTime? dataHoraSaida)
            : base(id)
        {
            Colaborador = colaborador;
            DataHoraEntrada = dataHoraEntrada;
            DataHoraSaida = dataHoraSaida;
        }

        public static Result<AcessoColaborador> Criar(
            int id,
            Colaborador colaborador,
            DateTime dataHoraEntrada,
            DateTime? dataHoraSaida = null)
        {
            var notifications = new List<Notification>();

            if (colaborador == null)
            {
                notifications.Add(
                    new Notification(
                        "Colaborador",
                        "COLABORADOR_OBRIGATORIO"));
            }

            if (dataHoraEntrada == default)
            {
                notifications.Add(
                    new Notification(
                        "DataHoraEntrada",
                        "DATA_HORA_ENTRADA_OBRIGATORIA"));
            }

            if (dataHoraSaida.HasValue &&
                dataHoraSaida.Value < dataHoraEntrada)
            {
                notifications.Add(
                    new Notification(
                        "DataHoraSaida",
                        "DATA_HORA_SAIDA_INVALIDA"));
            }

            if (notifications.Count != 0)
            {
                return Result<AcessoColaborador>.Failure(notifications);
            }

            return Result<AcessoColaborador>.Success(
                new AcessoColaborador(
                    id,
                    colaborador!,
                    dataHoraEntrada,
                    dataHoraSaida));
        }

        public Result<AcessoColaborador> RegistrarSaida(
            DateTime dataHoraSaida)
        {
            if (dataHoraSaida < DataHoraEntrada)
            {
                return Result<AcessoColaborador>.Failure(
                    "DataHoraSaida",
                    "DATA_HORA_SAIDA_INVALIDA");
            }

            DataHoraSaida = dataHoraSaida;

            return Result<AcessoColaborador>.Success(this);
        }
    }
}