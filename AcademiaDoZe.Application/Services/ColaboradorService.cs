// Pedro Vinicius

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
        _repoFactory =
            repoFactory
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

        return await _repoFactory().CpfJaExiste(
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

        return await _repoFactory().EmailJaExiste(
            emailResult.Value!,
            id,
            cancellationToken);
    }

    public async Task<ColaboradorDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var colaborador =
            await _repoFactory().ObterPorId(
                id,
                cancellationToken);

        return colaborador?.ToDto();
    }

    public async Task<IEnumerable<ColaboradorDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var colaboradores =
            await _repoFactory().ObterTodos(
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

        var colaborador =
            await _repoFactory().ObterPorCpf(
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

        var colaborador =
            await _repoFactory().ObterPorEmail(
                emailResult.Value!,
                cancellationToken);

        return colaborador?.ToDto();
    }

    public async Task<IEnumerable<ColaboradorDto>> ObterPorTipoAsync(
        EColaboradorTipo tipo,
        CancellationToken cancellationToken = default)
    {
        var colaboradores =
            await _repoFactory().ObterTodos(
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

        var colaboradores =
            await _repoFactory().ObterPorNome(
                nome.Trim(),
                cancellationToken);

        return colaboradores.Select(
            colaborador => colaborador.ToDto());
    }

    public async Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var colaborador =
            await _repoFactory().ObterPorId(
                id,
                cancellationToken);

        if (colaborador == null)
            return false;

        return await _repoFactory().Remover(
            id,
            cancellationToken);
    }

    public async Task<ColaboradorDto> AdicionarAsync(
        ColaboradorDto colaboradorDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(colaboradorDto);

        var cpfResult =
            Cpf.Criar(colaboradorDto.Cpf);

        if (cpfResult.IsFailure)
        {
            throw new ArgumentException(
                $"CPF inválido: {string.Join(
                    ", ",
                    cpfResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(colaboradorDto));
        }

        if (await _repoFactory().CpfJaExiste(
                cpfResult.Value!,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe um colaborador cadastrado com o CPF {colaboradorDto.Cpf}.");
        }

        var emailResult =
            Email.Criar(colaboradorDto.Email);

        if (emailResult.IsFailure)
        {
            throw new ArgumentException(
                $"Email inválido: {string.Join(
                    ", ",
                    emailResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(colaboradorDto));
        }

        if (await _repoFactory().EmailJaExiste(
                emailResult.Value!,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe um colaborador cadastrado com o Email {colaboradorDto.Email}.");
        }

        throw new NotSupportedException(
            "Para adicionar um Colaborador é necessário informar o Endereco, " +
            "pois a entidade Colaborador exige um Endereco.");
    }

    public async Task<ColaboradorDto> AtualizarAsync(
        ColaboradorDto colaboradorDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(colaboradorDto);

        var colaborador =
            await _repoFactory().ObterPorId(
                colaboradorDto.Id,
                cancellationToken);

        if (colaborador == null)
        {
            throw new KeyNotFoundException(
                $"Colaborador com ID {colaboradorDto.Id} não encontrado.");
        }

        var cpfResult =
            Cpf.Criar(colaboradorDto.Cpf);

        if (cpfResult.IsFailure)
        {
            throw new ArgumentException(
                $"CPF inválido: {string.Join(
                    ", ",
                    cpfResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(colaboradorDto));
        }

        if (await _repoFactory().CpfJaExiste(
                cpfResult.Value!,
                colaboradorDto.Id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe outro colaborador cadastrado com o CPF {colaboradorDto.Cpf}.");
        }

        var emailResult =
            Email.Criar(colaboradorDto.Email);

        if (emailResult.IsFailure)
        {
            throw new ArgumentException(
                $"Email inválido: {string.Join(
                    ", ",
                    emailResult.Notifications.Select(
                        n => n.Mensagem))}",
                nameof(colaboradorDto));
        }

        if (await _repoFactory().EmailJaExiste(
                emailResult.Value!,
                colaboradorDto.Id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"Já existe outro colaborador cadastrado com o Email {colaboradorDto.Email}.");
        }

        throw new NotSupportedException(
            "A atualização do Colaborador exige o Endereco completo, " +
            "pois a entidade Colaborador possui Endereco obrigatório.");
    }
}