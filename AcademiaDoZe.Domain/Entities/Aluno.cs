// Pedro Vinicius

using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Aluno : Pessoa, IAggregateRoot
    {
        public DateOnly DataNascimento { get; private set; }
        public Logradouro Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Complemento { get; private set; }
        public Senha Senha { get; private set; }
        public Arquivo Foto { get; private set; }

        private Aluno(
            int id,
            string nomeCompleto,
            Cpf cpf,
            DateOnly dataNascimento,
            Telefone telefone,
            Email email,
            Logradouro logradouro,
            string numero,
            string complemento,
            Senha senha,
            Arquivo foto,
            Endereco endereco)
            : base(
                id,
                nomeCompleto,
                cpf,
                email,
                telefone,
                endereco)
        {
            DataNascimento = dataNascimento;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Senha = senha;
            Foto = foto;
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

            // Nome
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

            // Data de nascimento
            if (dataNascimento == default)
            {
                notifications.Add(
                    new Notification(
                        "DataNascimento",
                        "DATA_NASCIMENTO_OBRIGATORIA"));
            }

            // Endereço
            if (endereco == null)
            {
                notifications.Add(
                    new Notification(
                        "Endereco",
                        "ENDERECO_OBRIGATORIO"));
            }

            // Número
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

            // CPF
            var cpfResult = Cpf.Criar(cpf);

            if (cpfResult.IsFailure)
            {
                notifications.AddRange(cpfResult.Notifications);
            }

            // Telefone
            var telefoneResult = Telefone.Criar(telefone);

            if (telefoneResult.IsFailure)
            {
                notifications.AddRange(telefoneResult.Notifications);
            }

            // E-mail
            var emailResult = Email.Criar(email);

            if (emailResult.IsFailure)
            {
                notifications.AddRange(emailResult.Notifications);
            }

            // Senha
            var senhaResult = Senha.Criar(senha);

            if (senhaResult.IsFailure)
            {
                notifications.AddRange(senhaResult.Notifications);
            }

            // Se houver erros
            if (notifications.Count > 0)
            {
                return Result<Aluno>.Failure(notifications);
            }

            // Cria o Endereco a partir do Logradouro
            var enderecoResult = Endereco.Criar(
                endereco.Cep,
                endereco.Nome,
                numero,
                complemento?.Trim() ?? string.Empty,
                endereco.Bairro,
                endereco.Cidade,
                endereco.Estado);

            if (enderecoResult.IsFailure)
            {
                notifications.AddRange(enderecoResult.Notifications);

                return Result<Aluno>.Failure(notifications);
            }

            // Cria o Aluno
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
                    complemento?.Trim() ?? string.Empty,
                    senhaResult.Value,
                    foto,
                    enderecoResult.Value));
        }
    }
}