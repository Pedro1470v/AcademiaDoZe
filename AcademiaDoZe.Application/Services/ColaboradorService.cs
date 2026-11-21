
// Pedro Vinicius

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Services;

public class ColaboradorService : IColaboradorService
{
    private readonly Func<IColaboradorRepository> _repoFactory;

    public ColaboradorService(
        Func<IColaboradorRepository> repoFactory)
    {
        _repoFactory = repoFactory
            ?? throw new ArgumentNullException(nameof(repoFactory));
    }

    public async Task<bool> CpfJaExisteAsync(
        string cpf,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cpfResult = Cpf.Criar(cpf);

        if (cpfResult.IsFailure)
            return false;

        var repository = _repoFactory();

        return await repository.CpfJaExiste(
            cpfResult.Value!,
            id,
            cancellationToken);
    }

    public async Task<bool> EmailJaExisteAsync(
        string email,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailResult = Email.Criar(email);

        if (emailResult.IsFailure)
            return false;

        var repository = _repoFactory();

        return await repository.EmailJaExiste(
            emailResult.Value!,
            id,
            cancellationToken);
    }

    public async Task<ColaboradorDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var repository = _repoFactory();

        var colaborador = await repository.ObterPorId(
            id,
            cancellationToken);

        return colaborador?.ToDto();
    }

    public async Task<IEnumerable<ColaboradorDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var repository = _repoFactory();

        var colaboradores = await repository.ObterTodos(
            cancellationToken);

        return colaboradores.Select(
            colaborador => colaborador.ToDto());
    }

    public async Task<ColaboradorDto?> ObterPorCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            throw new ArgumentException(
                "CPF não pode ser vazio.",
                nameof(cpf));
        }

        var cpfResult = Cpf.Criar(cpf);

        if (cpfResult.IsFailure)
        {
            throw new ArgumentException(
                $"CPF inválido: {string.Join(
                    ", ",
                    cpfResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(cpf));
        }

        var repository = _repoFactory();

        var colaborador = await repository.ObterPorCpf(
            cpfResult.Value!,
            cancellationToken);

        return colaborador?.ToDto();
    }

    public async Task<ColaboradorDto?> ObterPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email não pode ser vazio.",
                nameof(email));
        }

        var emailResult = Email.Criar(email);

        if (emailResult.IsFailure)
        {
            throw new ArgumentException(
                $"Email inválido: {string.Join(
                    ", ",
                    emailResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(email));
        }

        var repository = _repoFactory();

        var colaborador = await repository.ObterPorEmail(
            emailResult.Value!,
            cancellationToken);

        return colaborador?.ToDto();
    }

    public async Task<IEnumerable<ColaboradorDto>> ObterPorTipoAsync(
        EColaboradorTipo tipo,
        CancellationToken cancellationToken = default)
    {
        var repository = _repoFactory();

        var colaboradores = await repository.ObterTodos(
            cancellationToken);

        return colaboradores
            .Where(c => c.Tipo == tipo)
            .Select(c => c.ToDto());
    }

    public async Task<IEnumerable<ColaboradorDto>> ObterPorNomeAsync(
        string nome,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException(
                "Nome não pode ser vazio.",
                nameof(nome));
        }

        var repository = _repoFactory();

        var colaboradores = await repository.ObterPorNome(
            nome.Trim(),
            cancellationToken);

        return colaboradores.Select(
            colaborador => colaborador.ToDto());
    }

    public async Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var repository = _repoFactory();

        var colaborador = await repository.ObterPorId(
            id,
            cancellationToken);

        if (colaborador == null)
            return false;

        return await repository.Remover(
            id,
            cancellationToken);
    }

    public async Task<ColaboradorDto> AdicionarAsync(
        ColaboradorDto colaboradorDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(colaboradorDto);

        var repository = _repoFactory();

        // Valida o CPF.
        var cpfResult = Cpf.Criar(colaboradorDto.Cpf);

        if (cpfResult.IsFailure)
        {
            throw new ArgumentException(
                $"CPF inválido: {string.Join(
                    ", ",
                    cpfResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(colaboradorDto));
        }

        // Verifica CPF duplicado.
        if (await repository.CpfJaExiste(
                cpfResult.Value!,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe um colaborador cadastrado com o CPF {colaboradorDto.Cpf}.");
        }

        // Valida o e-mail.
        var emailResult = Email.Criar(
            colaboradorDto.Email ?? string.Empty);

        if (emailResult.IsFailure)
        {
            throw new ArgumentException(
                $"Email inválido: {string.Join(
                    ", ",
                    emailResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(colaboradorDto));
        }

        // Verifica e-mail duplicado.
        if (await repository.EmailJaExiste(
                emailResult.Value!,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe um colaborador cadastrado com o Email {colaboradorDto.Email}.");
        }

        // Converte o DTO para a entidade de domínio.
        // O mapeamento também converte o endereço para o Value Object Endereco.
        var colaborador = colaboradorDto.ToEntity();

        // Persiste no SQLite.
        var colaboradorSalvo = await repository.Adicionar(
            colaborador,
            cancellationToken);

        // Retorna o colaborador com o ID gerado pelo banco.
        return colaboradorSalvo.ToDto();
    }

    public async Task<ColaboradorDto> AtualizarAsync(
        ColaboradorDto colaboradorDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(colaboradorDto);

        var repository = _repoFactory();

        // Verifica se o colaborador existe.
        var colaboradorExistente = await repository.ObterPorId(
            colaboradorDto.Id,
            cancellationToken);

        if (colaboradorExistente == null)
        {
            throw new KeyNotFoundException(
                $"Colaborador com ID {colaboradorDto.Id} não encontrado.");
        }

        // Valida o CPF.
        var cpfResult = Cpf.Criar(colaboradorDto.Cpf);

        if (cpfResult.IsFailure)
        {
            throw new ArgumentException(
                $"CPF inválido: {string.Join(
                    ", ",
                    cpfResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(colaboradorDto));
        }

        // Verifica CPF duplicado, ignorando o próprio colaborador.
        if (await repository.CpfJaExiste(
                cpfResult.Value!,
                colaboradorDto.Id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe outro colaborador cadastrado com o CPF {colaboradorDto.Cpf}.");
        }

        // Valida o e-mail.
        var emailResult = Email.Criar(
            colaboradorDto.Email ?? string.Empty);

        if (emailResult.IsFailure)
        {
            throw new ArgumentException(
                $"Email inválido: {string.Join(
                    ", ",
                    emailResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(colaboradorDto));
        }

        // Verifica e-mail duplicado, ignorando o próprio colaborador.
        if (await repository.EmailJaExiste(
                emailResult.Value!,
                colaboradorDto.Id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe outro colaborador cadastrado com o Email {colaboradorDto.Email}.");
        }

        // Se nenhuma nova senha foi informada, preserva a senha existente.
        string? senhaInformada = colaboradorDto.Senha;

        if (string.IsNullOrWhiteSpace(senhaInformada))
        {
            colaboradorDto.Senha =
                colaboradorExistente.Senha.Valor;
        }

        try
        {
            // Converte o DTO para a entidade de domínio.
            // O ID original é preservado pelo mapeamento.
            var colaboradorAtualizado = colaboradorDto.ToEntity();

            // Atualiza no SQLite.
            var resultado = await repository.Atualizar(
                colaboradorAtualizado,
                cancellationToken);

            return resultado.ToDto();
        }
        finally
        {
            // Não mantém a senha armazenada no DTO da interface.
            colaboradorDto.Senha = senhaInformada;
        }
    }
}
