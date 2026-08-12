//Pedro Vinicius
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    internal class Endereco : ValueObject
    {
        public string Rua { get; }
        public string Numero { get; }
        public string Bairro { get; }
        public string Cidade { get; }
        public string Estado { get; }
        public string Cep { get; }

        private Endereco(
            string rua,
            string numero,
            string bairro,
            string cidade,
            string estado,
            string cep)
        {
            Rua = rua;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Cep = cep;
        }

        public static Result<Endereco> Criar(
            string rua,
            string numero,
            string bairro,
            string cidade,
            string estado,
            string cep)
        {
            var notifications = new List<Notification>();

            // Rua
            if (string.IsNullOrWhiteSpace(rua))
            {
                notifications.Add(
                    new Notification(
                        "Rua",
                        "RUA_OBRIGATORIA"));
            }
            else
            {
                rua = rua.Trim();
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

            // Bairro
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

            // Cidade
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

            // Estado
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

            // CEP
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

            // Se houver erros, não cria o endereço
            if (notifications.Count != 0)
            {
                return Result<Endereco>.Failure(notifications);
            }

            return Result<Endereco>.Success(
                new Endereco(
                    rua,
                    numero,
                    bairro,
                    cidade,
                    estado,
                    cep));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Rua;
            yield return Numero;
            yield return Bairro;
            yield return Cidade;
            yield return Estado;
            yield return Cep;
        }

        public override string ToString()
        {
            return $"{Rua}, {Numero} - {Bairro}, {Cidade}/{Estado} - CEP: {Cep}";
        }
    }
}