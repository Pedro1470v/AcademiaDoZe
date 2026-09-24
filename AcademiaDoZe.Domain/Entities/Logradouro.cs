// Pedro Vinicius

using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities
{
    public class Logradouro : Entidade, IAggregateRoot
    {
        public string Cep { get; }
        public string Nome { get; }
        public string Numero { get; }
        public string Bairro { get; }
        public string Cidade { get; }
        public string Estado { get; }

        private Logradouro(
            int id,
            string cep,
            string nome,
            string numero,
            string bairro,
            string cidade,
            string estado)
            : base(id)
        {
            Cep = cep;
            Nome = nome;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
        }

        public static Result<Logradouro> Criar(
            string cep,
            string nome,
            string bairro,
            string cidade,
            string estado)
        {
            return CriarInterno(
                0,
                cep,
                nome,
                string.Empty,
                bairro,
                cidade,
                estado);
        }

        public static Result<Logradouro> Criar(
            string cep,
            string nome,
            string numero,
            string bairro,
            string cidade,
            string estado)
        {
            return CriarInterno(
                0,
                cep,
                nome,
                numero,
                bairro,
                cidade,
                estado);
        }

        // Usado pelo Repository quando o ID já existe no banco.
        public static Result<Logradouro> Criar(
            int id,
            string cep,
            string nome,
            string bairro,
            string cidade,
            string estado)
        {
            return CriarInterno(
                id,
                cep,
                nome,
                string.Empty,
                bairro,
                cidade,
                estado);
        }

        // Usado pelo Repository quando o ID e o número já existem no banco.
        public static Result<Logradouro> Criar(
            int id,
            string cep,
            string nome,
            string numero,
            string bairro,
            string cidade,
            string estado)
        {
            return CriarInterno(
                id,
                cep,
                nome,
                numero,
                bairro,
                cidade,
                estado);
        }

        private static Result<Logradouro> CriarInterno(
            int id,
            string cep,
            string nome,
            string numero,
            string bairro,
            string cidade,
            string estado)
        {
            var notificacoes = new List<Notification>();

            if (id < 0)
            {
                notificacoes.Add(
                    new Notification(
                        "Id",
                        "O ID não pode ser negativo."));
            }

            if (string.IsNullOrWhiteSpace(cep))
            {
                notificacoes.Add(
                    new Notification(
                        "Cep",
                        "O CEP é obrigatório."));
            }
            else if (cep.Trim().Length != 8)
            {
                notificacoes.Add(
                    new Notification(
                        "Cep",
                        "O CEP deve possuir 8 caracteres."));
            }

            if (string.IsNullOrWhiteSpace(nome))
            {
                notificacoes.Add(
                    new Notification(
                        "Nome",
                        "O nome do logradouro é obrigatório."));
            }

            if (string.IsNullOrWhiteSpace(bairro))
            {
                notificacoes.Add(
                    new Notification(
                        "Bairro",
                        "O bairro é obrigatório."));
            }

            if (string.IsNullOrWhiteSpace(cidade))
            {
                notificacoes.Add(
                    new Notification(
                        "Cidade",
                        "A cidade é obrigatória."));
            }

            if (string.IsNullOrWhiteSpace(estado))
            {
                notificacoes.Add(
                    new Notification(
                        "Estado",
                        "O estado é obrigatório."));
            }

            if (notificacoes.Count > 0)
            {
                return Result<Logradouro>.Failure(
                    notificacoes);
            }

            return Result<Logradouro>.Success(
                new Logradouro(
                    id,
                    cep.Trim(),
                    nome.Trim(),
                    numero?.Trim() ?? string.Empty,
                    bairro.Trim(),
                    cidade.Trim(),
                    estado.Trim()));
        }
    }
}