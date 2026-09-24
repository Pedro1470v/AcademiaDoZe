// Pedro Vinicius

using System;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Tests
{
    public class AlunoTests
    {
        [Fact]
        public void Deve_Criar_Aluno_Quando_Dados_Validos()
        {
            var logradouroResult = Logradouro.Criar(
                "01001000",
                "Rua das Flores",
                "Centro",
                "São Paulo",
                "SP");

            Assert.True(logradouroResult.IsSuccess);

            var alunoResult = Aluno.Criar(
                1,
                "Pedro Vinicius",
                "52998224725",
                new DateOnly(2000, 1, 1),
                "11912345678",
                "pedro@email.com",
                logradouroResult.Value!,
                "123",
                "",
                "Senha1",
                null!);

            Assert.True(alunoResult.IsSuccess);
            Assert.NotNull(alunoResult.Value);
        }

        [Fact]
        public void Deve_Falhar_Quando_Nome_Nao_For_Informado()
        {
            var logradouroResult = Logradouro.Criar(
                "01001000",
                "Rua das Flores",
                "Centro",
                "São Paulo",
                "SP");

            Assert.True(logradouroResult.IsSuccess);

            var alunoResult = Aluno.Criar(
                1,
                "",
                "52998224725",
                new DateOnly(2000, 1, 1),
                "11912345678",
                "pedro@email.com",
                logradouroResult.Value!,
                "123",
                "",
                "Senha1",
                null!);

            Assert.True(alunoResult.IsFailure);

            Assert.Contains(
                alunoResult.Notifications,
                n => n.Mensagem == "NOME_OBRIGATORIO");
        }

        [Fact]
        public void Deve_Falhar_Quando_Data_Nascimento_Nao_For_Informada()
        {
            var logradouroResult = Logradouro.Criar(
                "01001000",
                "Rua das Flores",
                "Centro",
                "São Paulo",
                "SP");

            Assert.True(logradouroResult.IsSuccess);

            var alunoResult = Aluno.Criar(
                1,
                "Pedro Vinicius",
                "52998224725",
                default,
                "11912345678",
                "pedro@email.com",
                logradouroResult.Value!,
                "123",
                "",
                "Senha1",
                null!);

            Assert.True(alunoResult.IsFailure);

            Assert.Contains(
                alunoResult.Notifications,
                n => n.Mensagem == "DATA_NASCIMENTO_OBRIGATORIA");
        }

        [Fact]
        public void Deve_Falhar_Quando_Numero_Nao_For_Informado()
        {
            var logradouroResult = Logradouro.Criar(
                "01001000",
                "Rua das Flores",
                "Centro",
                "São Paulo",
                "SP");

            Assert.True(logradouroResult.IsSuccess);

            var alunoResult = Aluno.Criar(
                1,
                "Pedro Vinicius",
                "52998224725",
                new DateOnly(2000, 1, 1),
                "11912345678",
                "pedro@email.com",
                logradouroResult.Value!,
                "",
                "",
                "Senha1",
                null!);

            Assert.True(alunoResult.IsFailure);

            Assert.Contains(
                alunoResult.Notifications,
                n => n.Mensagem == "NUMERO_OBRIGATORIO");
        }
    }
}