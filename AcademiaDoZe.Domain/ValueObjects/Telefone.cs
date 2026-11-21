// Pedro Vinicius

using System.Collections.Generic;
using System.Linq;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Telefone : ValueObject
    {
        public string Valor { get; }

        private Telefone(string valor)
        {
            Valor = valor;
        }

        public static Result<Telefone> Criar(string valor)
        {
            var notificacoes = new List<Notification>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                notificacoes.Add(
                    new Notification(
                        "Telefone",
                        "O telefone é obrigatório."
                    )
                );

                return Result<Telefone>.Failure(notificacoes);
            }

            valor = new string(
                valor.Where(char.IsDigit).ToArray()
            );

            if (valor.Length != 10 && valor.Length != 11)
            {
                notificacoes.Add(
                    new Notification(
                        "Telefone",
                        "O telefone deve possuir 10 ou 11 dígitos."
                    )
                );

                return Result<Telefone>.Failure(notificacoes);
            }

            return Result<Telefone>.Success(
                new Telefone(valor)
            );
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }
    }
}