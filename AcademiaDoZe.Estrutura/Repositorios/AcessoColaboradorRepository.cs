// Pedro Vinicius

using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Infrastructure.Data
{
    public class AcessoColaboradorRepository
        : BaseRepository, IAcessoColaboradorRepository
    {
        public AcessoColaboradorRepository(
            string connectionString)
            : base(
                connectionString,
                DatabaseType.Sqlite)
        {
        }

        public async Task<AcessoColaborador?> ObterPorId(
            int id,
            CancellationToken cancellationToken = default)
        {
            using var connection = CriarConexao();

            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT
                    id_acesso_colaborador,
                    colaborador_id,
                    data_hora_entrada,
                    data_hora_saida
                FROM tb_acesso_colaborador
                WHERE id_acesso_colaborador = @id;";

            using var command = connection.CreateCommand();

            command.CommandText = sql;

            var parameter =
                command.CreateParameter();

            parameter.ParameterName = "@id";
            parameter.Value = id;

            command.Parameters.Add(parameter);

            using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            if (!await reader.ReadAsync(
                cancellationToken))
            {
                return null;
            }

            var colaboradorId =
                Convert.ToInt32(
                    reader["colaborador_id"]);

            var colaboradorRepository =
                new ColaboradorRepository(
                    ConnectionString);

            var colaborador =
                await colaboradorRepository.ObterPorId(
                    colaboradorId,
                    cancellationToken);

            if (colaborador == null)
                return null;

            var entrada =
                Convert.ToDateTime(
                    reader["data_hora_entrada"]);

            DateTime? saida = null;

            if (reader["data_hora_saida"] != DBNull.Value)
            {
                saida =
                    Convert.ToDateTime(
                        reader["data_hora_saida"]);
            }

            var resultado =
                AcessoColaborador.Criar(
                    Convert.ToInt32(
                        reader["id_acesso_colaborador"]),
                    colaborador,
                    entrada,
                    saida);

            if (resultado.IsFailure)
                return null;

            return resultado.Value;
        }

        public async Task<IEnumerable<AcessoColaborador>> ObterTodos(
            CancellationToken cancellationToken = default)
        {
            var acessos =
                new List<AcessoColaborador>();

            using var connection = CriarConexao();

            await connection.OpenAsync(
                cancellationToken);

            const string sql = @"
                SELECT
                    id_acesso_colaborador,
                    colaborador_id,
                    data_hora_entrada,
                    data_hora_saida
                FROM tb_acesso_colaborador
                ORDER BY data_hora_entrada;";

            using var command = connection.CreateCommand();

            command.CommandText = sql;

            using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            var colaboradorRepository =
                new ColaboradorRepository(
                    ConnectionString);

            while (await reader.ReadAsync(
                cancellationToken))
            {
                var colaboradorId =
                    Convert.ToInt32(
                        reader["colaborador_id"]);

                var colaborador =
                    await colaboradorRepository.ObterPorId(
                        colaboradorId,
                        cancellationToken);

                if (colaborador == null)
                    continue;

                var entrada =
                    Convert.ToDateTime(
                        reader["data_hora_entrada"]);

                DateTime? saida = null;

                if (reader["data_hora_saida"] != DBNull.Value)
                {
                    saida =
                        Convert.ToDateTime(
                            reader["data_hora_saida"]);
                }

                var resultado =
                    AcessoColaborador.Criar(
                        Convert.ToInt32(
                            reader["id_acesso_colaborador"]),
                        colaborador,
                        entrada,
                        saida);

                if (resultado.IsSuccess)
                {
                    acessos.Add(
                        resultado.Value!);
                }
            }

            return acessos;
        }

        public async Task<AcessoColaborador> Adicionar(
            AcessoColaborador entity,
            CancellationToken cancellationToken = default)
        {
            var tempoTotal =
                await ObterTempoTotalDoDia(
                    entity.Colaborador.ID,
                    entity.DataHoraEntrada,
                    cancellationToken);

            var tempoAtual =
                entity.DataHoraSaida.HasValue
                    ? entity.CalcularTempoPermanencia()
                    : TimeSpan.Zero;

            var tempoDepoisDoRegistro =
                tempoTotal + tempoAtual;

            var validacao =
                entity.ValidarLimiteDiario(
                    tempoDepoisDoRegistro);

            if (validacao.IsFailure)
            {
                throw new InvalidOperationException(
                    validacao.Notifications[0].Mensagem);
            }

            using var connection = CriarConexao();

            await connection.OpenAsync(
                cancellationToken);

            const string sql = @"
                INSERT INTO tb_acesso_colaborador
                (
                    colaborador_id,
                    data_hora_entrada,
                    data_hora_saida
                )
                VALUES
                (
                    @colaborador_id,
                    @data_hora_entrada,
                    @data_hora_saida
                );

                SELECT last_insert_rowid();";

            using var command = connection.CreateCommand();

            command.CommandText = sql;

            var colaboradorId =
                command.CreateParameter();

            colaboradorId.ParameterName =
                "@colaborador_id";

            colaboradorId.Value =
                entity.Colaborador.ID;

            command.Parameters.Add(
                colaboradorId);

            var entrada =
                command.CreateParameter();

            entrada.ParameterName =
                "@data_hora_entrada";

            entrada.Value =
                entity.DataHoraEntrada;

            command.Parameters.Add(
                entrada);

            var saida =
                command.CreateParameter();

            saida.ParameterName =
                "@data_hora_saida";

            saida.Value =
                entity.DataHoraSaida.HasValue
                    ? entity.DataHoraSaida.Value
                    : DBNull.Value;

            command.Parameters.Add(
                saida);

            var id =
                Convert.ToInt32(
                    await command.ExecuteScalarAsync(
                        cancellationToken));

            return AcessoColaborador.Criar(
                id,
                entity.Colaborador,
                entity.DataHoraEntrada,
                entity.DataHoraSaida).Value!;
        }

        public async Task<AcessoColaborador> Atualizar(
            AcessoColaborador entity,
            CancellationToken cancellationToken = default)
        {
            using var connection = CriarConexao();

            await connection.OpenAsync(
                cancellationToken);

            const string sql = @"
                UPDATE tb_acesso_colaborador
                SET
                    colaborador_id = @colaborador_id,
                    data_hora_entrada = @data_hora_entrada,
                    data_hora_saida = @data_hora_saida
                WHERE id_acesso_colaborador = @id;";

            using var command = connection.CreateCommand();

            command.CommandText = sql;

            var id =
                command.CreateParameter();

            id.ParameterName = "@id";
            id.Value = entity.ID;

            command.Parameters.Add(id);

            var colaboradorId =
                command.CreateParameter();

            colaboradorId.ParameterName =
                "@colaborador_id";

            colaboradorId.Value =
                entity.Colaborador.ID;

            command.Parameters.Add(
                colaboradorId);

            var entrada =
                command.CreateParameter();

            entrada.ParameterName =
                "@data_hora_entrada";

            entrada.Value =
                entity.DataHoraEntrada;

            command.Parameters.Add(
                entrada);

            var saida =
                command.CreateParameter();

            saida.ParameterName =
                "@data_hora_saida";

            saida.Value =
                entity.DataHoraSaida.HasValue
                    ? entity.DataHoraSaida.Value
                    : DBNull.Value;

            command.Parameters.Add(
                saida);

            await command.ExecuteNonQueryAsync(
                cancellationToken);

            return entity;
        }

        public async Task<bool> Remover(
            int id,
            CancellationToken cancellationToken = default)
        {
            using var connection = CriarConexao();

            await connection.OpenAsync(
                cancellationToken);

            const string sql = @"
                DELETE FROM tb_acesso_colaborador
                WHERE id_acesso_colaborador = @id;";

            using var command = connection.CreateCommand();

            command.CommandText = sql;

            var parameter =
                command.CreateParameter();

            parameter.ParameterName = "@id";
            parameter.Value = id;

            command.Parameters.Add(parameter);

            var linhas =
                await command.ExecuteNonQueryAsync(
                    cancellationToken);

            return linhas > 0;
        }

        public async Task<TimeSpan> ObterTempoTotalDoDia(
            int colaboradorId,
            DateTime data,
            CancellationToken cancellationToken = default)
        {
            using var connection = CriarConexao();

            await connection.OpenAsync(
                cancellationToken);

            var inicioDia =
                data.Date;

            var fimDia =
                inicioDia.AddDays(1);

            const string sql = @"
                SELECT
                    data_hora_entrada,
                    data_hora_saida
                FROM tb_acesso_colaborador
                WHERE colaborador_id = @colaborador_id
                  AND data_hora_entrada >= @inicio_dia
                  AND data_hora_entrada < @fim_dia
                  AND data_hora_saida IS NOT NULL
                ORDER BY data_hora_entrada;";

            using var command = connection.CreateCommand();

            command.CommandText = sql;

            var colaboradorParameter =
                command.CreateParameter();

            colaboradorParameter.ParameterName =
                "@colaborador_id";

            colaboradorParameter.Value =
                colaboradorId;

            command.Parameters.Add(
                colaboradorParameter);

            var inicioParameter =
                command.CreateParameter();

            inicioParameter.ParameterName =
                "@inicio_dia";

            inicioParameter.Value =
                inicioDia;

            command.Parameters.Add(
                inicioParameter);

            var fimParameter =
                command.CreateParameter();

            fimParameter.ParameterName =
                "@fim_dia";

            fimParameter.Value =
                fimDia;

            command.Parameters.Add(
                fimParameter);

            using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            var tempoTotal =
                TimeSpan.Zero;

            while (await reader.ReadAsync(
                cancellationToken))
            {
                var entrada =
                    Convert.ToDateTime(
                        reader["data_hora_entrada"]);

                var saida =
                    Convert.ToDateTime(
                        reader["data_hora_saida"]);

                if (saida >= entrada)
                {
                    tempoTotal +=
                        saida - entrada;
                }
            }

            return tempoTotal;
        }

        public async Task<bool> PodeRegistrarEntrada(
            Colaborador colaborador,
            DateTime dataHoraEntrada,
            CancellationToken cancellationToken = default)
        {
            var tempoTotal =
                await ObterTempoTotalDoDia(
                    colaborador.ID,
                    dataHoraEntrada,
                    cancellationToken);

            var limite =
                colaborador.Vinculo ==
                    EColaboradorVinculo.Estagio
                    ? TimeSpan.FromHours(6)
                    : TimeSpan.FromHours(8);

            return tempoTotal < limite;
        }
    }
}