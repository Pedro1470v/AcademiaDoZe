// Pedro Vinicius

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using System;
using System.Linq;

namespace AcademiaDoZe.Application.Mappings;

public static class AlunoMapping
{
    public static AlunoDto ToDto(this Aluno aluno)
    {
        return new AlunoDto
        {
            Id = aluno.ID,
            Nome = aluno.Nome,
            Cpf = aluno.Cpf.Valor,
            DataNascimento = aluno.DataNascimento,
            Telefone = aluno.Telefone.Valor,
            Email = aluno.Email?.Valor,
            Numero = aluno.Numero,
            Complemento = aluno.Complemento,
            Senha = aluno.Senha?.Valor,
            Endereco = null,

            Foto = aluno.Foto == null
                ? null
                : new ArquivoDto
                {
                    Conteudo = aluno.Foto.Conteudo
                }
        };
    }

    public static AlunoDto ToDto(
        this Aluno aluno,
        Logradouro? logradouro)
    {
        return new AlunoDto
        {
            Id = aluno.ID,
            Nome = aluno.Nome,
            Cpf = aluno.Cpf.Valor,
            DataNascimento = aluno.DataNascimento,
            Telefone = aluno.Telefone.Valor,
            Email = aluno.Email?.Valor,
            Numero = aluno.Numero,
            Complemento = aluno.Complemento,
            Senha = aluno.Senha?.Valor,
            Endereco = null,

            Foto = aluno.Foto == null
                ? null
                : new ArquivoDto
                {
                    Conteudo = aluno.Foto.Conteudo
                }
        };
    }

    public static Aluno ToEntity(
        this AlunoDto dto,
        Logradouro? logradouro)
    {
        if (logradouro == null)
        {
            throw new ArgumentNullException(
                nameof(logradouro),
                "Logradouro é obrigatório.");
        }

        Arquivo? arquivo = null;

        if (dto.Foto?.Conteudo != null &&
            dto.Foto.Conteudo.Length > 0)
        {
            var arquivoResult =
                Arquivo.Criar(dto.Foto.Conteudo);

            if (arquivoResult.IsFailure)
            {
                throw new InvalidOperationException(
                    string.Join(
                        ", ",
                        arquivoResult.Notifications.Select(
                            n => n.Mensagem)));
            }

            arquivo = arquivoResult.Value!;
        }

        var resultado = Aluno.Criar(
            dto.Id,
            dto.Nome,
            dto.Cpf,
            dto.DataNascimento,
            dto.Telefone,
            dto.Email ?? string.Empty,
            logradouro,
            dto.Numero,
            dto.Complemento ?? string.Empty,
            dto.Senha ?? string.Empty,
            arquivo);

        if (resultado.IsFailure)
        {
            throw new InvalidOperationException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(
                        n => n.Mensagem)));
        }

        return resultado.Value!;
    }

    public static Aluno UpdateFromDto(
        this Aluno aluno,
        AlunoDto dto,
        Logradouro? logradouro)
    {
        Arquivo? arquivo = aluno.Foto;

        if (dto.Foto?.Conteudo != null &&
            dto.Foto.Conteudo.Length > 0)
        {
            var arquivoResult =
                Arquivo.Criar(dto.Foto.Conteudo);

            if (arquivoResult.IsFailure)
            {
                throw new InvalidOperationException(
                    string.Join(
                        ", ",
                        arquivoResult.Notifications.Select(
                            n => n.Mensagem)));
            }

            arquivo = arquivoResult.Value!;
        }

        var resultado = Aluno.Criar(
            aluno.ID,
            dto.Nome,
            dto.Cpf,
            dto.DataNascimento,
            dto.Telefone,
            dto.Email ?? aluno.Email.Valor,
            logradouro ?? aluno.Logradouro,
            dto.Numero,
            dto.Complemento ?? string.Empty,
            dto.Senha ?? aluno.Senha.Valor,
            arquivo);

        if (resultado.IsFailure)
        {
            throw new InvalidOperationException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(
                        n => n.Mensagem)));
        }

        return resultado.Value!;
    }
}