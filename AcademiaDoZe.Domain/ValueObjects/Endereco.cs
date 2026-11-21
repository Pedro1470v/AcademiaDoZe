using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Endereco : ValueObject
    {
        public string Cep { get; }
        public string Logradouro { get; }
        public string Numero { get; }
        public string Complemento { get; }
        public string Bairro { get; }
        public string Cidade { get; }
        public string Estado { get; }

        private Endereco(
            string cep,
            string logradouro,
            string numero,
            string complemento,
            string bairro,
            string cidade,
            string estado)
        {
            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
        }

        public static Result<Endereco> Criar(
            string cep,
            string logradouro,
            string numero,
            string complemento,
            string bairro,
            string cidade,
            string estado)
        {
            return Result<Endereco>.Success(
                new Endereco(
                    cep,
                    logradouro,
                    numero,
                    complemento,
                    bairro,
                    cidade,
                    estado
                )
            );
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Cep;
            yield return Logradouro;
            yield return Numero;
            yield return Complemento;
            yield return Bairro;
            yield return Cidade;
            yield return Estado;
        }
    }
}