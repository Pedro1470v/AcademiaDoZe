// Pedro Vinicius

using System.Collections.Generic;
using System.Text.RegularExpressions;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        public string Valor { get; }

        private Email(string valor)
        {
            Valor = valor;
        }

        public static Result<Email> Criar(string valor)
        {
            var notificacoes = new List<Notification>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                notificacoes.Add(
                    new Notification(
                        "Email",
                        "O e-mail é obrigatório."
                    )
                );
            }
            else
            {
                valor = valor.Trim();

                if (!Regex.IsMatch(
                    valor,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    notificacoes.Add(
                        new Notification(
                            "Email",
                            "O e-mail informado é inválido."
                        )
                    );
                }
            }

            if (notificacoes.Count > 0)
            {
                return Result<Email>.Failure(notificacoes);
            }

            return Result<Email>.Success(
                new Email(valor)
            );
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }
    }
}