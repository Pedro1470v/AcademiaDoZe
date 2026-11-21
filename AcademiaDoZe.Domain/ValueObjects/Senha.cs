//Pedro Vinicius
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Senha : ValueObject
    {
        public string Valor { get; }

        private Senha(string valor)
        {
            Valor = valor;
        }

        public static Result<Senha> Criar(string valor)
        {
            var notifications = new List<Notification>();

            if (string.IsNullOrWhiteSpace(valor))
            {
                notifications.Add(
                    new Notification(
                        "Valor",
                        "SENHA_OBRIGATORIA"));
            }
            else
            {
                valor = valor.Trim();

                if (valor.Length < 6)
                {
                    notifications.Add(
                        new Notification(
                            "Valor",
                            "SENHA_MINIMO_6_CARACTERES"));
                }
            }

            if (notifications.Count != 0)
            {
                return Result<Senha>.Failure(notifications);
            }

            return Result<Senha>.Success(
                new Senha(valor));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valor;
        }

        public override string ToString()
        {
            return "********";
        }
    }
}