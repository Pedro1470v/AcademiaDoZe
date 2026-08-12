//Pedro Vinicius
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities
{
    internal class Logradouro : Entidade
    {
        public string Cep { get; }
        public string Nome { get; }
        public string Bairro { get; }
        public string Cidade { get; }
        public string Estado { get; }
        public string Pais { get; }

        private Logradouro(
            int id,
            string cep,
            string nome,
            string bairro,
            string cidade,
            string estado,
            string pais)
            : base(id)
        {
            Cep = cep;
            Nome = nome;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Pais = pais;
        }

        public static Result<Logradouro> Criar(
            int id,
            string cep,
            string nome,
            string bairro,
            string cidade,
            string estado,
            string pais)
        {
            var notifications = new List<Notification>();

            if (string.IsNullOrWhiteSpace(cep))
            {
                notifications.Add(
                    new Notification(
                        "Cep",
                        "CEP_OBRIGATORIO"));
            }
            else
            {
                cep = cep
                    .Replace("-", "")
                    .Trim();

                if (cep.Length != 8)
                {
                    notifications.Add(
                        new Notification(
                            "Cep",
                            "CEP_INVALIDO"));
                }
            }

            if (string.IsNullOrWhiteSpace(nome))
            {
                notifications.Add(
                    new Notification(
                        "Nome",
                        "NOME_OBRIGATORIO"));
            }
            else
            {
                nome = nome.Trim();
            }

            if (string.IsNullOrWhiteSpace(bairro))
            {
                notifications.Add(
                    new Notification(
                        "Bairro",
                        "BAIRRO_OBRIGATORIO"));
            }
            else
            {
                bairro = bairro.Trim();
            }

            if (string.IsNullOrWhiteSpace(cidade))
            {
                notifications.Add(
                    new Notification(
                        "Cidade",
                        "CIDADE_OBRIGATORIA"));
            }
            else
            {
                cidade = cidade.Trim();
            }

            if (string.IsNullOrWhiteSpace(estado))
            {
                notifications.Add(
                    new Notification(
                        "Estado",
                        "ESTADO_OBRIGATORIO"));
            }
            else
            {
                estado = estado.Trim().ToUpper();
            }

            if (string.IsNullOrWhiteSpace(pais))
            {
                notifications.Add(
                    new Notification(
                        "Pais",
                        "PAIS_OBRIGATORIO"));
            }
            else
            {
                pais = pais.Trim();
            }

            if (notifications.Count > 0)
            {
                return Result<Logradouro>.Failure(notifications);
            }

            return Result<Logradouro>.Success(
                new Logradouro(
                    id,
                    cep,
                    nome,
                    bairro,
                    cidade,
                    estado,
                    pais));
        }
    }
}