// Pedro Vinicius

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public class LogradouroService : ILogradouroService
{
    private readonly Func<ILogradouroRepository> _repoFactory;

    public LogradouroService(
        Func<ILogradouroRepository> repoFactory)
    {
        _repoFactory =
            repoFactory
            ?? throw new ArgumentNullException(nameof(repoFactory));
    }

    public async Task<LogradouroDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var logradouro =
            await _repoFactory().ObterPorId(
                id,
                cancellationToken);

        return logradouro == null
            ? null
            : ToDto(logradouro);
    }

    public async Task<IEnumerable<LogradouroDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var logradouros =
            await _repoFactory().ObterTodos(
                cancellationToken);

        return logradouros.Select(ToDto);
    }

    public async Task<LogradouroDto?> ObterPorCepAsync(
        string cep,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cep))
        {
            throw new ArgumentException(
                "CEP não pode ser vazio.",
                nameof(cep));
        }

        var logradouros =
            await _repoFactory().ObterTodos(
                cancellationToken);

        var logradouro =
            logradouros.FirstOrDefault(x =>
                string.Equals(
                    x.Cep,
                    cep.Trim(),
                    StringComparison.OrdinalIgnoreCase));

        return logradouro == null
            ? null
            : ToDto(logradouro);
    }

    public async Task<bool> CepJaExisteAsync(
        string cep,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return false;

        var logradouros =
            await _repoFactory().ObterTodos(
                cancellationToken);

        return logradouros.Any(x =>
            string.Equals(
                x.Cep,
                cep.Trim(),
                StringComparison.OrdinalIgnoreCase)
            &&
            (!id.HasValue || x.ID != id.Value));
    }

    public async Task<IEnumerable<LogradouroDto>> ObterPorCidadeAsync(
        string cidade,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cidade))
        {
            throw new ArgumentException(
                "Cidade não pode ser vazia.",
                nameof(cidade));
        }

        var logradouros =
            await _repoFactory().ObterTodos(
                cancellationToken);

        return logradouros
            .Where(x =>
                x.Cidade.Contains(
                    cidade.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            .Select(ToDto);
    }

    public async Task<IEnumerable<LogradouroDto>> ObterPorBairroAsync(
        string cidade,
        string bairro,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cidade))
        {
            throw new ArgumentException(
                "Cidade não pode ser vazia.",
                nameof(cidade));
        }

        if (string.IsNullOrWhiteSpace(bairro))
        {
            throw new ArgumentException(
                "Bairro não pode ser vazio.",
                nameof(bairro));
        }

        var logradouros =
            await _repoFactory().ObterTodos(
                cancellationToken);

        return logradouros
            .Where(x =>
                x.Cidade.Contains(
                    cidade.Trim(),
                    StringComparison.OrdinalIgnoreCase)
                &&
                x.Bairro.Contains(
                    bairro.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            .Select(ToDto);
    }

    public async Task<LogradouroDto> AdicionarAsync(
        LogradouroDto logradouroDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logradouroDto);

        if (await CepJaExisteAsync(
                logradouroDto.Cep,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe um logradouro cadastrado com o CEP {logradouroDto.Cep}.");
        }

        var resultado =
            Logradouro.Criar(
                logradouroDto.Cep,
                logradouroDto.Nome,
                logradouroDto.Numero,
                logradouroDto.Bairro,
                logradouroDto.Cidade,
                logradouroDto.Estado);

        if (resultado.IsFailure)
        {
            throw new ArgumentException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(
                        x => x.Mensagem)));
        }

        var adicionado =
            await _repoFactory().Adicionar(
                resultado.Value!,
                cancellationToken);

        return ToDto(adicionado);
    }

    public async Task<LogradouroDto> AtualizarAsync(
        LogradouroDto logradouroDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logradouroDto);

        var existente =
            await _repoFactory().ObterPorId(
                logradouroDto.Id,
                cancellationToken);

        if (existente == null)
        {
            throw new KeyNotFoundException(
                $"Logradouro com ID {logradouroDto.Id} não encontrado.");
        }

        if (await CepJaExisteAsync(
                logradouroDto.Cep,
                logradouroDto.Id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe outro logradouro cadastrado com o CEP {logradouroDto.Cep}.");
        }

        var resultado =
            Logradouro.Criar(
                logradouroDto.Id,
                logradouroDto.Cep,
                logradouroDto.Nome,
                logradouroDto.Numero,
                logradouroDto.Bairro,
                logradouroDto.Cidade,
                logradouroDto.Estado);

        if (resultado.IsFailure)
        {
            throw new ArgumentException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(
                        x => x.Mensagem)));
        }

        var atualizado =
            await _repoFactory().Atualizar(
                resultado.Value!,
                cancellationToken);

        return ToDto(atualizado);
    }

    public async Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _repoFactory().Remover(
            id,
            cancellationToken);
    }

    private static LogradouroDto ToDto(
        Logradouro logradouro)
    {
        return new LogradouroDto
        {
            Id = logradouro.ID,
            Cep = logradouro.Cep,
            Nome = logradouro.Nome,
            Numero = logradouro.Numero,
            Bairro = logradouro.Bairro,
            Cidade = logradouro.Cidade,
            Estado = logradouro.Estado
        };
    }
}