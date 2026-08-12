//Pedro Vinicius
using System;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    internal abstract class Pessoa : Entidade
    {
        public string Nome { get; }
        public Cpf Cpf { get; }
        public DateOnly DataNascimento { get; }
        public Telefone Telefone { get; }
        public Email Email { get; }
        public Logradouro Endereco { get; }
        public string Numero { get; }
        public string Complemento { get; }
        public Senha Senha { get; }
        public Arquivo Foto { get; }

        protected Pessoa(
            int id,
            string nome,
            Cpf cpf,
            DateOnly dataNascimento,
            Telefone telefone,
            Email email,
            Logradouro endereco,
            string numero,
            string complemento,
            Senha senha,
            Arquivo foto)
            : base(id)
        {
            Nome = nome;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            Email = email;
            Endereco = endereco;
            Numero = numero;
            Complemento = complemento;
            Senha = senha;
            Foto = foto;
        }
    }
}