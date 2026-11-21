// Pedro Vinicius

using System;
using System.Linq;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class ColaboradorMapping
{
    public static Colaborador ToEntity(this ColaboradorDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.Endereco == null)
        {
            throw new ArgumentException(
                "Informe o endereço do colaborador.");
        }

        // Endereço manual
        var enderecoResult = Endereco.Criar(
            dto.Endereco.Cep,
            dto.Endereco.Nome,
            dto.Numero,
            dto.Complemento ?? string.Empty,
            dto.Endereco.Bairro,
            dto.Endereco.Cidade,
            dto.Endereco.Estado);

        if (enderecoResult.IsFailure)
        {
            throw new ArgumentException(
                string.Join(", ",
                    enderecoResult.Notifications.Select(n => n.Mensagem)));
        }

        // CPF
        var cpfResult = Cpf.Criar(dto.Cpf);

        if (cpfResult.IsFailure)
        {
            throw new ArgumentException(
                string.Join(", ",
                    cpfResult.Notifications.Select(n => n.Mensagem)));
        }

        // E-mail
        var emailResult = Email.Criar(dto.Email ?? string.Empty);

        if (emailResult.IsFailure)
        {
            throw new ArgumentException(
                string.Join(", ",
                    emailResult.Notifications.Select(n => n.Mensagem)));
        }

        // Telefone
        var telefoneResult = Telefone.Criar(dto.Telefone);

        if (telefoneResult.IsFailure)
        {
            throw new ArgumentException(
                string.Join(", ",
                    telefoneResult.Notifications.Select(n => n.Mensagem)));
        }

        // Senha
        var senhaResult = Senha.Criar(dto.Senha ?? string.Empty);

        if (senhaResult.IsFailure)
        {
            throw new ArgumentException(
                string.Join(", ",
                    senhaResult.Notifications.Select(n => n.Mensagem)));
        }

        // Foto
        Arquivo? foto = null;

        if (dto.Foto?.Conteudo != null &&
            dto.Foto.Conteudo.Length > 0)
        {
            var fotoResult = Arquivo.Criar(dto.Foto.Conteudo);

            if (fotoResult.IsFailure)
            {
                throw new ArgumentException(
                    string.Join(", ",
                        fotoResult.Notifications.Select(n => n.Mensagem)));
            }

            foto = fotoResult.Value;
        }

        // Colaborador
        var resultado = Colaborador.Criar(
            dto.Id,
            dto.Nome,
            cpfResult.Value!,
            emailResult.Value!,
            telefoneResult.Value!,
            enderecoResult.Value!,
            dto.DataNascimento,
            dto.DataAdmissao,
            senhaResult.Value!,
            foto,
            dto.Tipo,
            dto.Vinculo);

        if (resultado.IsFailure)
        {
            throw new ArgumentException(
                string.Join(", ",
                    resultado.Notifications.Select(n => n.Mensagem)));
        }

        return resultado.Value!;
    }

    public static ColaboradorDto ToDto(this Colaborador colaborador)
    {
        ArgumentNullException.ThrowIfNull(colaborador);

        return new ColaboradorDto
        {
            Id = colaborador.ID,
            Nome = colaborador.Nome,
            Cpf = colaborador.Cpf.Valor,
            Email = colaborador.Email?.Valor,
            Telefone = colaborador.Telefone.Valor,

            DataNascimento = colaborador.DataNascimento,
            DataAdmissao = colaborador.DataAdmissao,

            // Não devolver a senha para a interface.
            Senha = null,

            Foto = colaborador.Foto == null
                ? null
                : new ArquivoDto
                {
                    Conteudo = colaborador.Foto.Conteudo
                },

            Numero = colaborador.Endereco.Numero,
            Complemento = colaborador.Endereco.Complemento,

            Endereco = new LogradouroDto
            {
                Id = 0,
                Cep = colaborador.Endereco.Cep,
                Nome = colaborador.Endereco.Logradouro,
                Numero = colaborador.Endereco.Numero,
                Bairro = colaborador.Endereco.Bairro,
                Cidade = colaborador.Endereco.Cidade,
                Estado = colaborador.Endereco.Estado,
                Pais = "Brasil"
            },

            Tipo = colaborador.Tipo,
            Vinculo = colaborador.Vinculo,
            Ativo = true
        };
    }
}