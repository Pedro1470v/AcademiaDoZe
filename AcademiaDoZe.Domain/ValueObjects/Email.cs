//Pedro Vinicius
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    internal class Email : ValueObject
    {
        public string Endereco { get; }

        private Email(string endereco)
        {
            Endereco = endereco;
        }

        public static Result<Email> Criar(string endereco)
        {
            var notifications = new List<Notification>();

            if (string.IsNullOrWhiteSpace(endereco))
            {
                notifications.Add(
                    new Notification(
                        "Endereco",
                        "EMAIL_OBRIGATORIO"));
            }
            else
            {
                endereco = endereco.Trim();

                if (!endereco.Contains("@") || !endereco.Contains("."))
                {
                    notifications.Add(
                        new Notification(
                            "Endereco",
                            "EMAIL_INVALIDO"));
                }
            }

            if (notifications.Count != 0)
            {
                return Result<Email>.Failure(notifications);
            }

            return Result<Email>.Success(
                new Email(endereco));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Endereco;
        }

        public override string ToString()
        {
            return Endereco;
        }
    }
}