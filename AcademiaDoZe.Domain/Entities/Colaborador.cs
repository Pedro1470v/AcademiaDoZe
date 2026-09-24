
// Pedro Vinicius

using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Colaborador : Pessoa, IAggregateRoot
    {
        public EColaboradorTipo Tipo { get; protected set; }

        public EColaboradorVinculo Vinculo { get; protected set; }

        private Colaborador(
            int id,
            string nome,
            Cpf cpf,
            Email email,
            Telefone telefone,
            Endereco endereco,
            EColaboradorTipo tipo,
            EColaboradorVinculo vinculo)
            : base(
                id,
                nome,
                cpf,
                email,
                telefone,
                endereco)
        {
            Tipo = tipo;
            Vinculo = vinculo;
        }

        public static Result<Colaborador> Criar(
            string nome,
            Cpf cpf,
            Email email,
            Telefone telefone,
            Endereco endereco,
            EColaboradorTipo tipo,
            EColaboradorVinculo vinculo)
        {
            var notificacoes = new List<Notification>();

            if (string.IsNullOrWhiteSpace(nome))
            {
                notificacoes.Add(
                    new Notification(
                        "Nome",
                        "O nome é obrigatório."));
            }

            if (cpf == null)
            {
                notificacoes.Add(
                    new Notification(
                        "Cpf",
                        "O CPF é obrigatório."));
            }

            if (email == null)
            {
                notificacoes.Add(
                    new Notification(
                        "Email",
                        "O e-mail é obrigatório."));
            }

            if (telefone == null)
            {
                notificacoes.Add(
                    new Notification(
                        "Telefone",
                        "O telefone é obrigatório."));
            }

            if (endereco == null)
            {
                notificacoes.Add(
                    new Notification(
                        "Endereco",
                        "O endereço é obrigatório."));
            }

            if (notificacoes.Count > 0)
            {
                return Result<Colaborador>.Failure(notificacoes);
            }

            var colaborador = new Colaborador(
                0,
                nome.Trim(),
                cpf,
                email,
                telefone,
                endereco,
                tipo,
                vinculo);

            return Result<Colaborador>.Success(colaborador);
        }
    }
}
