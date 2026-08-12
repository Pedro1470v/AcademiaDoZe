//Pedro Vinicius
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    internal class Colaborador : Pessoa
    {
        public DateOnly DataAdmissao { get; }
        public EColaboradorTipo Tipo { get; }
        public EColaboradorVinculo Vinculo { get; }

        private Colaborador(
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
            Arquivo foto,
            DateOnly dataAdmissao,
            EColaboradorTipo tipo,
            EColaboradorVinculo vinculo)
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
            DataAdmissao = dataAdmissao;
            Tipo = tipo;
            Vinculo = vinculo;
        }

        public static Result<Colaborador> Criar(
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
            Arquivo foto,
            DateOnly dataAdmissao,
            EColaboradorTipo tipo,
            EColaboradorVinculo vinculo)
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
            else if (
                dataNascimento >
                DateOnly.FromDateTime(DateTime.Today.AddYears(-12)))
            {
                notifications.Add(
                    new Notification(
                        "DataNascimento",
                        "IDADE_MINIMA_12_ANOS"));
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

            if (dataAdmissao == default)
            {
                notifications.Add(
                    new Notification(
                        "DataAdmissao",
                        "DATA_ADMISSAO_OBRIGATORIA"));
            }
            else if (
                dataAdmissao >
                DateOnly.FromDateTime(DateTime.Today))
            {
                notifications.Add(
                    new Notification(
                        "DataAdmissao",
                        "DATA_ADMISSAO_NAO_PODE_SER_FUTURA"));
            }

            if (!Enum.IsDefined(typeof(EColaboradorTipo), tipo))
            {
                notifications.Add(
                    new Notification(
                        "Tipo",
                        "TIPO_COLABORADOR_INVALIDO"));
            }

            if (!Enum.IsDefined(typeof(EColaboradorVinculo), vinculo))
            {
                notifications.Add(
                    new Notification(
                        "Vinculo",
                        "VINCULO_INVALIDO"));
            }

            if (tipo == EColaboradorTipo.Administrador &&
                vinculo != EColaboradorVinculo.CLT)
            {
                notifications.Add(
                    new Notification(
                        "Vinculo",
                        "ADMINISTRADOR_DEVE_SER_CLT"));
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
                return Result<Colaborador>.Failure(notifications);
            }

            return Result<Colaborador>.Success(
                new Colaborador(
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
                    foto,
                    dataAdmissao,
                    tipo,
                    vinculo));
        }
    }
}