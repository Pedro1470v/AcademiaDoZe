//Pedro Vinicius
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities
{
    internal class Matricula : Entidade
    {
        public Aluno Aluno { get; private set; }
        public DateOnly DataMatricula { get; private set; }
        public bool Ativa { get; private set; }

        private Matricula(
            int id,
            Aluno aluno,
            DateOnly dataMatricula,
            bool ativa)
            : base(id)
        {
            Aluno = aluno;
            DataMatricula = dataMatricula;
            Ativa = ativa;
        }

        public static Result<Matricula> Criar(
            int id,
            Aluno aluno,
            DateOnly dataMatricula,
            bool ativa)
        {
            var notifications = new List<Notification>();

            // Validação do aluno
            if (aluno == null)
            {
                notifications.Add(
                    new Notification(
                        "Aluno",
                        "ALUNO_OBRIGATORIO"));
            }

            // Validação da data da matrícula
            if (dataMatricula == default)
            {
                notifications.Add(
                    new Notification(
                        "DataMatricula",
                        "DATA_MATRICULA_OBRIGATORIA"));
            }
            else if (dataMatricula > DateOnly.FromDateTime(DateTime.Today))
            {
                notifications.Add(
                    new Notification(
                        "DataMatricula",
                        "DATA_MATRICULA_MAIOR_QUE_DATA_ATUAL"));
            }

            if (notifications.Count != 0)
            {
                return Result<Matricula>.Failure(notifications);
            }

            // Criação da matrícula
            var matricula = new Matricula(
                id,
                aluno!,
                dataMatricula,
                ativa);

            return Result<Matricula>.Success(matricula);
        }

        public void Ativar()
        {
            Ativa = true;
        }

        public void Desativar()
        {
            Ativa = false;
        }
    }
}