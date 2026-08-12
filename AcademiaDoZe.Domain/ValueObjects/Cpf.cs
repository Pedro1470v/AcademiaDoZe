//Pedro Vinicius
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    internal class Cpf : ValueObject
    {
        public string Numero { get; }

        private Cpf(string numero)
        {
            Numero = numero;
        }

        public static Result<Cpf> Criar(string numero)
        {
            var notifications = new List<Notification>();

            if (string.IsNullOrWhiteSpace(numero))
            {
                notifications.Add(
                    new Notification(
                        "Numero",
                        "CPF_OBRIGATORIO"));
            }
            else
            {
                numero = numero
                    .Replace(".", "")
                    .Replace("-", "")
                    .Trim();

                if (numero.Length != 11)
                {
                    notifications.Add(
                        new Notification(
                            "Numero",
                            "CPF_DEVE_CONTER_11_DIGITOS"));
                }
            }

            if (notifications.Count != 0)
            {
                return Result<Cpf>.Failure(notifications);
            }

            return Result<Cpf>.Success(
                new Cpf(numero));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Numero;
        }

        public override string ToString()
        {
            return Numero;
        }
    }
}