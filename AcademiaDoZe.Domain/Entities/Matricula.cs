// Pedro Vinicius

using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Matricula : Entidade, IAggregateRoot
    {
        public Aluno Aluno { get; private set; }

        public int AlunoId
        {
            get
            {
                return Aluno.ID;
            }
        }

        public MatriculaPlano Plano { get; private set; }

        public DateOnly DataInicio { get; private set; }

        public DateOnly DataFim { get; private set; }

        public string Objetivo { get; private set; }

        public MatriculaRestricoes RestricoesMedicas { get; private set; }

        public string? ObservacoesRestricoes { get; private set; }

        public Arquivo? LaudoMedico { get; private set; }

        private Matricula(
            int id,
            Aluno aluno,
            MatriculaPlano plano,
            DateOnly dataInicio,
            DateOnly dataFim,
            string objetivo,
            MatriculaRestricoes restricoesMedicas,
            Arquivo? laudoMedico,
            string? observacoesRestricoes)
            : base(id)
        {
            Aluno = aluno;
            Plano = plano;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Objetivo = objetivo;
            RestricoesMedicas = restricoesMedicas;
            LaudoMedico = laudoMedico;
            ObservacoesRestricoes = observacoesRestricoes;
        }

        public static Result<Matricula> Criar(
            int id,
            Aluno aluno,
            MatriculaPlano plano,
            DateOnly dataInicio,
            string objetivo,
            MatriculaRestricoes restricoesMedicas,
            Arquivo? laudoMedico = null,
            string? observacoesRestricoes = null)
        {
            var notifications = new List<Notification>();

            if (aluno == null)
            {
                notifications.Add(
                    new Notification(
                        "Aluno",
                        "ALUNO_OBRIGATORIO"));
            }

            if (dataInicio == default)
            {
                notifications.Add(
                    new Notification(
                        "DataInicio",
                        "DATA_INICIO_OBRIGATORIA"));
            }

            if (string.IsNullOrWhiteSpace(objetivo))
            {
                notifications.Add(
                    new Notification(
                        "Objetivo",
                        "OBJETIVO_OBRIGATORIO"));
            }

            DateOnly dataFim = dataInicio;

            if (plano == MatriculaPlano.Mensal)
            {
                dataFim = dataInicio.AddMonths(1);
            }
            else if (plano == MatriculaPlano.Trimestral)
            {
                dataFim = dataInicio.AddMonths(3);
            }
            else if (plano == MatriculaPlano.Semestral)
            {
                dataFim = dataInicio.AddMonths(6);
            }
            else if (plano == MatriculaPlano.Anual)
            {
                dataFim = dataInicio.AddYears(1);
            }

            if (notifications.Count > 0)
            {
                return Result<Matricula>.Failure(notifications);
            }

            var matricula = new Matricula(
                id,
                aluno!,
                plano,
                dataInicio,
                dataFim,
                objetivo,
                restricoesMedicas,
                laudoMedico,
                observacoesRestricoes);

            return Result<Matricula>.Success(matricula);
        }

        public void AlterarObjetivo(string objetivo)
        {
            if (!string.IsNullOrWhiteSpace(objetivo))
            {
                Objetivo = objetivo;
            }
        }

        public void AlterarRestricoes(
            MatriculaRestricoes restricoesMedicas,
            Arquivo? laudoMedico,
            string? observacoesRestricoes)
        {
            RestricoesMedicas = restricoesMedicas;
            LaudoMedico = laudoMedico;
            ObservacoesRestricoes = observacoesRestricoes;
        }

        public void AlterarPlano(MatriculaPlano plano)
        {
            Plano = plano;

            if (plano == MatriculaPlano.Mensal)
            {
                DataFim = DataInicio.AddMonths(1);
            }
            else if (plano == MatriculaPlano.Trimestral)
            {
                DataFim = DataInicio.AddMonths(3);
            }
            else if (plano == MatriculaPlano.Semestral)
            {
                DataFim = DataInicio.AddMonths(6);
            }
            else if (plano == MatriculaPlano.Anual)
            {
                DataFim = DataInicio.AddYears(1);
            }
        }
    }
}