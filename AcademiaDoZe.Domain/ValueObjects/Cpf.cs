// Pedro Vinicius

using System.Collections.Generic;
using System.Linq;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Cpf : ValueObject
    {
        public string Valor { get; }

        private Cpf(string valor)
        {
            Valor = valor;
        }

        public static Result<Cpf> Criar(string valor)
        {
            var notificacoes = new List<Notification>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                notificacoes.Add(
                    new Notification(
                        "Cpf",
                        "O CPF é obrigatório."
                    )
                );
            }
            else
            {
                valor = new string(
                    valor.Where(char.IsDigit).ToArray()
                );

                if (valor.Length != 11)
                {
                    notificacoes.Add(
                        new Notification(
                            "Cpf",
                            "O CPF deve possuir 11 dígitos."
                        )
                    );
                }
            }

            if (notificacoes.Count > 0)
            {
                return Result<Cpf>.Failure(notificacoes);
            }

            return Result<Cpf>.Success(
                new Cpf(valor)
            );
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }
    }
}