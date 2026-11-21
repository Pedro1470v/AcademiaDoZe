
// Pedro Vinicius

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Services;

public class MatriculaService : IMatriculaService
{
    private readonly Func<IMatriculaRepository> _matriculaRepoFactory;
    private readonly Func<IAlunoRepository> _alunoRepoFactory;

    public MatriculaService(
        Func<IMatriculaRepository> matriculaRepoFactory,
        Func<IAlunoRepository> alunoRepoFactory)
    {
        _matriculaRepoFactory =
            matriculaRepoFactory
            ?? throw new ArgumentNullException(
                nameof(matriculaRepoFactory));

        _alunoRepoFactory =
            alunoRepoFactory
            ?? throw new ArgumentNullException(
                nameof(alunoRepoFactory));
    }

    public async Task<MatriculaDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var matricula =
            await _matriculaRepoFactory()
                .ObterPorId(id, cancellationToken);

        if (matricula == null)
            return null;

        var aluno =
            await _alunoRepoFactory()
                .ObterPorId(matricula.AlunoId, cancellationToken);

        if (aluno == null)
        {
            throw new InvalidOperationException(
                $"Aluno associado à matrícula {matricula.ID} não encontrado.");
        }

        return ConverterParaDto(matricula, aluno);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterTodasAsync(
        CancellationToken cancellationToken = default)
    {
        var matriculas =
            await _matriculaRepoFactory()
                .ObterTodos(cancellationToken);

        return await ConverterListaParaDto(
            matriculas,
            cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterPorAlunoIdAsync(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var aluno =
            await _alunoRepoFactory()
                .ObterPorId(alunoId, cancellationToken);

        if (aluno == null)
        {
            throw new InvalidOperationException(
                $"Aluno com ID {alunoId} não encontrado.");
        }

        var matriculas =
            await _matriculaRepoFactory()
                .ObterPorAluno(alunoId, cancellationToken);

        var resultado = new List<MatriculaDto>();

        foreach (var matricula in matriculas)
        {
            resultado.Add(
                ConverterParaDto(matricula, aluno));
        }

        return resultado;
    }

    public async Task<MatriculaDto?> ObterMatriculaAtivaPorAlunoAsync(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var matricula =
            await _matriculaRepoFactory()
                .ObterMatriculaAtivaPorAluno(
                    alunoId,
                    cancellationToken);

        if (matricula == null)
            return null;

        var aluno =
            await _alunoRepoFactory()
                .ObterPorId(alunoId, cancellationToken);

        if (aluno == null)
        {
            throw new InvalidOperationException(
                $"Aluno com ID {alunoId} não encontrado.");
        }

        return ConverterParaDto(matricula, aluno);
    }

    public async Task<bool> PossuiMatriculaAtivaAsync(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        return await _matriculaRepoFactory()
            .PossuiMatriculaAtiva(
                alunoId,
                cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterAtivasAsync(
        int alunoId = 0,
        CancellationToken cancellationToken = default)
    {
        var matriculas =
            await _matriculaRepoFactory()
                .ObterAtivas(cancellationToken);

        if (alunoId > 0)
        {
            matriculas =
                matriculas.Where(x => x.AlunoId == alunoId);
        }

        return await ConverterListaParaDto(
            matriculas,
            cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterVencendoEmDiasAsync(
        int dias,
        CancellationToken cancellationToken = default)
    {
        var matriculas =
            await _matriculaRepoFactory()
                .ObterVencendoEmDias(dias, cancellationToken);

        return await ConverterListaParaDto(
            matriculas,
            cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterPorPlanoAsync(
        MatriculaPlano plano,
        CancellationToken cancellationToken = default)
    {
        var matriculas =
            await _matriculaRepoFactory()
                .ObterPorPlano(plano, cancellationToken);

        return await ConverterListaParaDto(
            matriculas,
            cancellationToken);
    }

    public async Task<MatriculaDto> AdicionarAsync(
        MatriculaDto matriculaDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(matriculaDto);

        if (matriculaDto.AlunoMatricula == null)
        {
            throw new ArgumentException(
                "O aluno deve ser informado.");
        }

        if (matriculaDto.AlunoMatricula.Id <= 0)
        {
            throw new ArgumentException(
                "O ID do aluno deve ser válido.");
        }

        var aluno =
            await _alunoRepoFactory()
                .ObterPorId(
                    matriculaDto.AlunoMatricula.Id,
                    cancellationToken);

        if (aluno == null)
        {
            throw new KeyNotFoundException(
                $"Aluno com ID {matriculaDto.AlunoMatricula.Id} não encontrado.");
        }

        var possuiMatriculaAtiva =
            await _matriculaRepoFactory()
                .PossuiMatriculaAtiva(
                    aluno.ID,
                    cancellationToken);

        if (possuiMatriculaAtiva)
        {
            throw new InvalidOperationException(
                "O aluno já possui uma matrícula ativa.");
        }

        bool menorDe16 =
            aluno.DataNascimento >
            DateOnly.FromDateTime(DateTime.Today.AddYears(-16));

        bool possuiLaudo =
            matriculaDto.LaudoMedico?.Conteudo != null &&
            matriculaDto.LaudoMedico.Conteudo.Length > 0;

        if (menorDe16 && !possuiLaudo)
        {
            throw new InvalidOperationException(
                "Alunos menores de 16 anos devem apresentar laudo médico autorizando a prática de atividades físicas.");
        }

        if (matriculaDto.RestricoesMedicas !=
            (MatriculaRestricoes)0 && !possuiLaudo)
        {
            throw new InvalidOperationException(
                "Alunos com restrições médicas devem apresentar parecer médico autorizando a prática de atividades físicas.");
        }

        Arquivo? laudoMedico = null;

        if (possuiLaudo)
        {
            var arquivoResult =
                Arquivo.Criar(matriculaDto.LaudoMedico!.Conteudo!);

            if (arquivoResult.IsFailure)
            {
                throw new ArgumentException(
                    string.Join(
                        ", ",
                        arquivoResult.Notifications.Select(
                            x => x.Mensagem)));
            }

            laudoMedico = arquivoResult.Value;
        }

        var resultado =
            Matricula.Criar(
                matriculaDto.Id,
                aluno,
                matriculaDto.Plano,
                matriculaDto.DataInicio,
                matriculaDto.Objetivo,
                matriculaDto.RestricoesMedicas,
                laudoMedico,
                matriculaDto.ObservacoesRestricoes ?? string.Empty);

        if (resultado.IsFailure)
        {
            throw new ArgumentException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(
                        x => x.Mensagem)));
        }

        var matricula = resultado.Value!;

        var adicionada =
            await _matriculaRepoFactory()
                .Adicionar(matricula, cancellationToken);

        return ConverterParaDto(adicionada, aluno);
    }

    public async Task<MatriculaDto> AtualizarAsync(
        MatriculaDto matriculaDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(matriculaDto);

        var existente =
            await _matriculaRepoFactory()
                .ObterPorId(matriculaDto.Id, cancellationToken);

        if (existente == null)
        {
            throw new KeyNotFoundException(
                $"Matrícula com ID {matriculaDto.Id} não encontrada.");
        }

        var aluno =
            await _alunoRepoFactory()
                .ObterPorId(existente.AlunoId, cancellationToken);

        if (aluno == null)
        {
            throw new InvalidOperationException(
                $"Aluno associado à matrícula {matriculaDto.Id} não encontrado.");
        }

        bool menorDe16 =
            aluno.DataNascimento >
            DateOnly.FromDateTime(DateTime.Today.AddYears(-16));

        bool possuiLaudo =
            matriculaDto.LaudoMedico?.Conteudo != null &&
            matriculaDto.LaudoMedico.Conteudo.Length > 0;

        if (!possuiLaudo && existente.LaudoMedico != null)
            possuiLaudo = true;

        if (menorDe16 && !possuiLaudo)
        {
            throw new InvalidOperationException(
                "Alunos menores de 16 anos devem apresentar laudo médico autorizando a prática de atividades físicas.");
        }

        if (matriculaDto.RestricoesMedicas !=
            (MatriculaRestricoes)0 && !possuiLaudo)
        {
            throw new InvalidOperationException(
                "Alunos com restrições médicas devem apresentar parecer médico autorizando a prática de atividades físicas.");
        }

        Arquivo? laudoMedico = existente.LaudoMedico;

        if (matriculaDto.LaudoMedico?.Conteudo != null)
        {
            var arquivoResult =
                Arquivo.Criar(matriculaDto.LaudoMedico.Conteudo);

            if (arquivoResult.IsFailure)
            {
                throw new ArgumentException(
                    string.Join(
                        ", ",
                        arquivoResult.Notifications.Select(
                            x => x.Mensagem)));
            }

            laudoMedico = arquivoResult.Value;
        }

        var resultado =
            Matricula.Criar(
                existente.ID,
                aluno,
                matriculaDto.Plano,
                matriculaDto.DataInicio,
                matriculaDto.Objetivo,
                matriculaDto.RestricoesMedicas,
                laudoMedico,
                matriculaDto.ObservacoesRestricoes
                    ?? existente.ObservacoesRestricoes);

        if (resultado.IsFailure)
        {
            throw new ArgumentException(
                string.Join(
                    ", ",
                    resultado.Notifications.Select(
                        x => x.Mensagem)));
        }

        var atualizada =
            await _matriculaRepoFactory()
                .Atualizar(resultado.Value!, cancellationToken);

        return ConverterParaDto(atualizada, aluno);
    }

    public async Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var matricula =
            await _matriculaRepoFactory()
                .ObterPorId(id, cancellationToken);

        if (matricula == null)
            return false;

        return await _matriculaRepoFactory()
            .Remover(id, cancellationToken);
    }

    // CONVERTER LISTA DE MATRÍCULAS PARA DTO
    private async Task<IEnumerable<MatriculaDto>> ConverterListaParaDto(
        IEnumerable<Matricula> matriculas,
        CancellationToken cancellationToken)
    {
        var resultado = new List<MatriculaDto>();

        foreach (var matricula in matriculas)
        {
            var aluno =
                await _alunoRepoFactory()
                    .ObterPorId(
                        matricula.AlunoId,
                        cancellationToken);

            if (aluno == null)
            {
                throw new InvalidOperationException(
                    $"Aluno associado à matrícula {matricula.ID} não encontrado.");
            }

            resultado.Add(
                ConverterParaDto(matricula, aluno));
        }

        return resultado;
    }

    // CONVERTER MATRÍCULA PARA DTO, INCLUINDO A FOTO DO ALUNO
    private static MatriculaDto ConverterParaDto(
        Matricula matricula,
        Aluno aluno)
    {
        ArquivoDto? arquivoDto = null;

        if (matricula.LaudoMedico != null)
        {
            arquivoDto = new ArquivoDto
            {
                Conteudo = matricula.LaudoMedico.Conteudo
            };
        }

        ArquivoDto? fotoDto = null;

        if (aluno.Foto != null)
        {
            fotoDto = new ArquivoDto
            {
                Conteudo = aluno.Foto.Conteudo
            };
        }

        return new MatriculaDto
        {
            Id = matricula.ID,

            AlunoMatricula = new AlunoDto
            {
                Id = aluno.ID,
                Nome = aluno.Nome,
                Cpf = aluno.Cpf.Valor,
                DataNascimento = aluno.DataNascimento,
                Telefone = aluno.Telefone.Valor,
                Email = aluno.Email.Valor,
                Numero = aluno.Numero,
                Foto = fotoDto
            },

            Plano = matricula.Plano,
            DataInicio = matricula.DataInicio,
            DataFim = matricula.DataFim,
            Objetivo = matricula.Objetivo,
            RestricoesMedicas = matricula.RestricoesMedicas,
            ObservacoesRestricoes = matricula.ObservacoesRestricoes,
            LaudoMedico = arquivoDto
        };
    }
}
