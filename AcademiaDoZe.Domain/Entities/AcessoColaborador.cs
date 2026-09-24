// Pedro Vinicius

using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Entities
{
    public sealed class AcessoColaborador : Entidade, IAggregateRoot
    {
        public Colaborador Colaborador { get; private set; }

        public DateTime DataHoraEntrada { get; private set; }

        public DateTime? DataHoraSaida { get; private set; }

        public TimeSpan TempoPermanencia
        {
            get
            {
                if (!DataHoraSaida.HasValue)
                    return TimeSpan.Zero;

                return DataHoraSaida.Value - DataHoraEntrada;
            }
        }

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
            var notifications =
                new List<Notification>();

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
                return Result<AcessoColaborador>.Failure(
                    notifications);
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

            DataHoraSaida =
                dataHoraSaida;

            return Result<AcessoColaborador>.Success(
                this);
        }

        public TimeSpan CalcularTempoPermanencia()
        {
            if (!DataHoraSaida.HasValue)
                return TimeSpan.Zero;

            return DataHoraSaida.Value -
                   DataHoraEntrada;
        }

        public TimeSpan ObterLimiteDiario()
        {
            if (Colaborador.Vinculo ==
                EColaboradorVinculo.Estagio)
            {
                return TimeSpan.FromHours(6);
            }

            return TimeSpan.FromHours(8);
        }

        public Result<AcessoColaborador> ValidarLimiteDiario(
            TimeSpan tempoTotalDoDia)
        {
            var limite =
                ObterLimiteDiario();

            if (tempoTotalDoDia > limite)
            {
                return Result<AcessoColaborador>.Failure(
                    "DataHoraEntrada",
                    Colaborador.Vinculo ==
                        EColaboradorVinculo.Estagio
                        ? "LIMITE_DIARIO_ESTAGIO_EXCEDIDO"
                        : "LIMITE_DIARIO_CLT_EXCEDIDO");
            }

            return Result<AcessoColaborador>.Success(
                this);
        }
    }
}