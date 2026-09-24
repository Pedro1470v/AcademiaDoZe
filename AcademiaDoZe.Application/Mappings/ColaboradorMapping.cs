// Pedro Vinicius

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Application.Mappings;

public static class ColaboradorMapping
{
    public static ColaboradorDto ToDto(
        this Colaborador colaborador)
    {
        return new ColaboradorDto
        {
            Id = colaborador.ID,
            Nome = colaborador.Nome,
            Cpf = colaborador.Cpf.Valor,
            Email = colaborador.Email.Valor,
            Telefone = colaborador.Telefone.Valor,
            Tipo = colaborador.Tipo.ToString(),
            Ativo = true
        };
    }
}