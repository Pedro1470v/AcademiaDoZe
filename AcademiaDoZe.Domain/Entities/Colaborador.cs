// Pedro Vinicius

using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Colaborador : Pessoa, IAggregateRoot
    {
        public DateOnly DataNascimento { get; protected set; }

        public DateOnly DataAdmissao { get; protected set; }

        public Senha Senha { get; protected set; }

        public Arquivo? Foto { get; protected set; }

        public EColaboradorTipo Tipo { get; protected set; }

        public EColaboradorVinculo Vinculo { get; protected set; }

        private Colaborador(
            int id,
            string nome,
            Cpf cpf,
            Email email,
            Telefone telefone,
            Endereco endereco,
            DateOnly dataNascimento,
            DateOnly dataAdmissao,
            Senha senha,
            Arquivo? foto,
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
            DataNascimento = dataNascimento;
            DataAdmissao = dataAdmissao;
            Senha = senha;
            Foto = foto;
            Tipo = tipo;
            Vinculo = vinculo;
        }

        // Criação de um novo colaborador.
        public static Result<Colaborador> Criar(
            string nome,
            Cpf cpf,
            Email email,
            Telefone telefone,
            Endereco endereco,
            DateOnly dataNascimento,
            DateOnly dataAdmissao,
            Senha senha,
            Arquivo? foto,
            EColaboradorTipo tipo,
            EColaboradorVinculo vinculo)
        {
            return Criar(
                0,
                nome,
                cpf,
                email,
                telefone,
                endereco,
                dataNascimento,
                dataAdmissao,
                senha,
                foto,
                tipo,
                vinculo);
        }

        // Criação preservando o ID para consultas e atualizações.
        public static Result<Colaborador> Criar(
            int id,
            string nome,
            Cpf cpf,
            Email email,
            Telefone telefone,
            Endereco endereco,
            DateOnly dataNascimento,
            DateOnly dataAdmissao,
            Senha senha,
            Arquivo? foto,
            EColaboradorTipo tipo,
            EColaboradorVinculo vinculo)
        {
            var notificacoes = new List<Notification>();

            if (id < 0)
            {
                notificacoes.Add(
                    new Notification(
                        "Id",
                        "O ID não pode ser negativo."));
            }

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

            if (dataNascimento == default)
            {
                notificacoes.Add(
                    new Notification(
                        "DataNascimento",
                        "A data de nascimento é obrigatória."));
            }

            if (dataAdmissao == default)
            {
                notificacoes.Add(
                    new Notification(
                        "DataAdmissao",
                        "A data de admissão é obrigatória."));
            }

            if (senha == null)
            {
                notificacoes.Add(
                    new Notification(
                        "Senha",
                        "A senha é obrigatória."));
            }

            if (notificacoes.Count > 0)
            {
                return Result<Colaborador>.Failure(notificacoes);
            }

            var colaborador = new Colaborador(
                id,
                nome.Trim(),
                cpf,
                email,
                telefone,
                endereco,
                dataNascimento,
                dataAdmissao,
                senha,
                foto,
                tipo,
                vinculo);

            return Result<Colaborador>.Success(colaborador);
        }
    }
}