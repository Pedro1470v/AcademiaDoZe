//Pedro Vinicius
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    internal class Aluno : Pessoa
    {
        private Aluno(
            int id,
            string nomeCompleto,
            Cpf cpf,
            DateOnly dataNascimento,
            Telefone telefone,
            Email email,
            Logradouro endereco,
            string numero,
            string complemento,
            Senha senha,
            Arquivo foto)
            : base(
                id,
                nomeCompleto,
                cpf,
                dataNascimento,
                telefone,
                email,
                endereco,
                numero,
                complemento,
                senha,
                foto)
        {
        }

        public static Result<Aluno> Criar(
            int id,
            string nomeCompleto,
            string cpf,
            DateOnly dataNascimento,
            string telefone,
            string email,
            Logradouro endereco,
            string numero,
            string complemento,
            string senha,
            Arquivo foto)
        {
            var notifications = new List<Notification>();

            if (string.IsNullOrWhiteSpace(nomeCompleto))
            {
                notifications.Add(
                    new Notification(
                        "Nome",
                        "NOME_OBRIGATORIO"));
            }
            else
            {
                nomeCompleto = nomeCompleto.Trim();
            }

            if (dataNascimento == default)
            {
                notifications.Add(
                    new Notification(
                        "DataNascimento",
                        "DATA_NASCIMENTO_OBRIGATORIA"));
            }

            if (endereco == null)
            {
                notifications.Add(
                    new Notification(
                        "Endereco",
                        "ENDERECO_OBRIGATORIO"));
            }

            if (string.IsNullOrWhiteSpace(numero))
            {
                notifications.Add(
                    new Notification(
                        "Numero",
                        "NUMERO_OBRIGATORIO"));
            }
            else
            {
                numero = numero.Trim();
            }

            var cpfResult = Cpf.Criar(cpf);

            if (cpfResult.IsFailure)
                notifications.AddRange(cpfResult.Notifications);

            var telefoneResult = Telefone.Criar(telefone);

            if (telefoneResult.IsFailure)
                notifications.AddRange(telefoneResult.Notifications);

            var emailResult = Email.Criar(email);

            if (emailResult.IsFailure)
                notifications.AddRange(emailResult.Notifications);

            var senhaResult = Senha.Criar(senha);

            if (senhaResult.IsFailure)
                notifications.AddRange(senhaResult.Notifications);

            if (notifications.Count > 0)
            {
                return Result<Aluno>.Failure(notifications);
            }

            return Result<Aluno>.Success(
                new Aluno(
                    id,
                    nomeCompleto,
                    cpfResult.Value,
                    dataNascimento,
                    telefoneResult.Value,
                    emailResult.Value,
                    endereco,
                    numero,
                    complemento?.Trim(),
                    senhaResult.Value,
                    foto));
        }
    }
}