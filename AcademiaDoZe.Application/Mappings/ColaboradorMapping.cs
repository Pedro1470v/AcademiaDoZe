
// Pedro Vinicius

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.Mappings;

public static class ColaboradorMapping
{
    public static ColaboradorDto ToDto(this Colaborador colaborador)
    {
        return new ColaboradorDto
        {
            Id = colaborador.ID,
            Nome = colaborador.Nome,
            Cpf = colaborador.Cpf.Valor,
            Email = colaborador.Email.Valor,
            Telefone = colaborador.Telefone.Valor,

            DataNascimento = DateOnly.FromDateTime(DateTime.Today),

            Endereco = colaborador.Endereco == null
                ? null
                : new LogradouroDto
                {
                    Id = 0,
                    Cep = colaborador.Endereco.Cep,
                    Nome = colaborador.Endereco.Logradouro,
                    Numero = colaborador.Endereco.Numero,
                    Bairro = colaborador.Endereco.Bairro,
                    Cidade = colaborador.Endereco.Cidade,
                    Estado = colaborador.Endereco.Estado
                },

            Numero = colaborador.Endereco?.Numero ?? string.Empty,
            Complemento = colaborador.Endereco?.Complemento,

            Tipo = colaborador.Tipo,
            Ativo = true
        };
    }
}
