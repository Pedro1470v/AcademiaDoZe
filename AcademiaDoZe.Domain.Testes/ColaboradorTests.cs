// Pedro Vinicius

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests
{
    public class ColaboradorTests
    {
        private static Cpf CriarCpf()
        {
            return Cpf.Criar("52998224725").Value!;
        }

        private static Email CriarEmail()
        {
            return Email.Criar("pedro@teste.com").Value!;
        }

        private static Telefone CriarTelefone()
        {
            return Telefone.Criar("49999999999").Value!;
        }

        private static Endereco CriarEndereco()
        {
            return Endereco.Criar(
                "88500000",
                "Rua Pedro Vinicius",
                "100",
                "Oliveira",
                "Centro",
                "Lages",
                "SC").Value!;
        }

        [Fact]
        public void Colaborador_Criar_Sucesso()
        {
            var colaborador =
                Colaborador.Criar(
                    "Pedro Vinicius",
                    CriarCpf(),
                    CriarEmail(),
                    CriarTelefone(),
                    CriarEndereco(),
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.CLT);

            Assert.True(colaborador.IsSuccess);
            Assert.NotNull(colaborador.Value);
        }

        [Fact]
        public void Colaborador_Criar_ComEstagio_Sucesso()
        {
            var colaborador =
                Colaborador.Criar(
                    "Pedro Vinicius",
                    CriarCpf(),
                    CriarEmail(),
                    CriarTelefone(),
                    CriarEndereco(),
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.Estagio);

            Assert.True(colaborador.IsSuccess);
            Assert.NotNull(colaborador.Value);
        }

        [Fact]
        public void Colaborador_Criar_SalvaTipo()
        {
            var colaborador =
                Colaborador.Criar(
                    "Pedro Vinicius",
                    CriarCpf(),
                    CriarEmail(),
                    CriarTelefone(),
                    CriarEndereco(),
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.CLT);

            Assert.True(colaborador.IsSuccess);
            Assert.Equal(
                EColaboradorTipo.Instrutor,
                colaborador.Value!.Tipo);
        }

        [Fact]
        public void Colaborador_Criar_SalvaVinculoCLT()
        {
            var colaborador =
                Colaborador.Criar(
                    "Pedro Vinicius",
                    CriarCpf(),
                    CriarEmail(),
                    CriarTelefone(),
                    CriarEndereco(),
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.CLT);

            Assert.True(colaborador.IsSuccess);
            Assert.Equal(
                EColaboradorVinculo.CLT,
                colaborador.Value!.Vinculo);
        }

        [Fact]
        public void Colaborador_Criar_SalvaVinculoEstagio()
        {
            var colaborador =
                Colaborador.Criar(
                    "Pedro Vinicius",
                    CriarCpf(),
                    CriarEmail(),
                    CriarTelefone(),
                    CriarEndereco(),
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.Estagio);

            Assert.True(colaborador.IsSuccess);
            Assert.Equal(
                EColaboradorVinculo.Estagio,
                colaborador.Value!.Vinculo);
        }

        [Fact]
        public void Colaborador_Criar_SemNome_Falha()
        {
            var resultado =
                Colaborador.Criar(
                    "",
                    CriarCpf(),
                    CriarEmail(),
                    CriarTelefone(),
                    CriarEndereco(),
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.CLT);

            Assert.True(resultado.IsFailure);
            Assert.Contains(
                resultado.Notifications,
                x => x.Property == "Nome");
        }

        [Fact]
        public void Colaborador_Criar_SemCpf_Falha()
        {
            var resultado =
                Colaborador.Criar(
                    "Pedro Vinicius",
                    null!,
                    CriarEmail(),
                    CriarTelefone(),
                    CriarEndereco(),
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.CLT);

            Assert.True(resultado.IsFailure);
            Assert.Contains(
                resultado.Notifications,
                x => x.Property == "Cpf");
        }

        [Fact]
        public void Colaborador_Criar_SemEmail_Falha()
        {
            var resultado =
                Colaborador.Criar(
                    "Pedro Vinicius",
                    CriarCpf(),
                    null!,
                    CriarTelefone(),
                    CriarEndereco(),
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.CLT);

            Assert.True(resultado.IsFailure);
            Assert.Contains(
                resultado.Notifications,
                x => x.Property == "Email");
        }

        [Fact]
        public void Colaborador_Criar_SemTelefone_Falha()
        {
            var resultado =
                Colaborador.Criar(
                    "Pedro Vinicius",
                    CriarCpf(),
                    CriarEmail(),
                    null!,
                    CriarEndereco(),
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.CLT);

            Assert.True(resultado.IsFailure);
            Assert.Contains(
                resultado.Notifications,
                x => x.Property == "Telefone");
        }

        [Fact]
        public void Colaborador_Criar_SemEndereco_Falha()
        {
            var resultado =
                Colaborador.Criar(
                    "Pedro Vinicius",
                    CriarCpf(),
                    CriarEmail(),
                    CriarTelefone(),
                    null!,
                    EColaboradorTipo.Instrutor,
                    EColaboradorVinculo.CLT);

            Assert.True(resultado.IsFailure);
            Assert.Contains(
                resultado.Notifications,
                x => x.Property == "Endereco");
        }
    }
}