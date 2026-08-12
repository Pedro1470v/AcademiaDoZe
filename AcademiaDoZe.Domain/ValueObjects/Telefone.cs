//Pedro Vinicius
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    internal class Telefone : ValueObject
    {
        public string Numero { get; }

        private Telefone(string numero)
        {
            Numero = numero;
        }

        public static Result<Telefone> Criar(string numero)
        {
            var notifications = new List<Notification>();

            if (string.IsNullOrWhiteSpace(numero))
            {
                notifications.Add(
                    new Notification(
                        "Numero",
                        "TELEFONE_OBRIGATORIO"));
            }
            else
            {
                numero = numero
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("-", "")
                    .Replace(" ", "")
                    .Trim();

                if (numero.Length < 10 || numero.Length > 11)
                {
                    notifications.Add(
                        new Notification(
                            "Numero",
                            "TELEFONE_INVALIDO"));
                }
            }

            if (notifications.Count != 0)
            {
                return Result<Telefone>.Failure(notifications);
            }

            return Result<Telefone>.Success(
                new Telefone(numero));
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