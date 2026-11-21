
// Pedro Vinicius

using System;
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

        private static Senha CriarSenha()
        {
            return Senha.Criar("Senha123").Value!;
        }

        private static Colaborador CriarColaborador(
            string nome = "Pedro Vinicius",
            Cpf? cpf = null,
            Email? email = null,
            Telefone? telefone = null,
            Endereco? endereco = null,
            EColaboradorTipo tipo = EColaboradorTipo.Instrutor,
            EColaboradorVinculo vinculo = EColaboradorVinculo.CLT)
        {
            var resultado = Colaborador.Criar(
                nome,
                cpf ?? CriarCpf(),
                email ?? CriarEmail(),
                telefone ?? CriarTelefone(),
                endereco ?? CriarEndereco(),
                new DateOnly(2000, 1, 1),
                new DateOnly(2025, 1, 1),
                CriarSenha(),
                null,
                tipo,
                vinculo);

            return resultado.Value!;
        }

        [Fact]
        public void Colaborador_Criar_Sucesso()
        {
            var colaborador = CriarColaborador();

            Assert.NotNull(colaborador);
        }

        [Fact]
        public void Colaborador_Criar_ComEstagio_Sucesso()
        {
            var colaborador = CriarColaborador(
                vinculo: EColaboradorVinculo.Estagio);

            Assert.Equal(
                EColaboradorVinculo.Estagio,
                colaborador.Vinculo);
        }

        [Fact]
        public void Colaborador_Criar_SalvaTipo()
        {
            var colaborador = CriarColaborador(
                tipo: EColaboradorTipo.Instrutor);

            Assert.Equal(
                EColaboradorTipo.Instrutor,
                colaborador.Tipo);
        }

        [Fact]
        public void Colaborador_Criar_SalvaVinculoCLT()
        {
            var colaborador = CriarColaborador(
                vinculo: EColaboradorVinculo.CLT);

            Assert.Equal(
                EColaboradorVinculo.CLT,
                colaborador.Vinculo);
        }

        [Fact]
        public void Colaborador_Criar_SalvaVinculoEstagio()
        {
            var colaborador = CriarColaborador(
                vinculo: EColaboradorVinculo.Estagio);

            Assert.Equal(
                EColaboradorVinculo.Estagio,
                colaborador.Vinculo);
        }

        [Fact]
        public void Colaborador_Criar_SemNome_Falha()
        {
            var resultado = Colaborador.Criar(
                "",
                CriarCpf(),
                CriarEmail(),
                CriarTelefone(),
                CriarEndereco(),
                new DateOnly(2000, 1, 1),
                new DateOnly(2025, 1, 1),
                CriarSenha(),
                null,
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
            var resultado = Colaborador.Criar(
                "Pedro Vinicius",
                null!,
                CriarEmail(),
                CriarTelefone(),
                CriarEndereco(),
                new DateOnly(2000, 1, 1),
                new DateOnly(2025, 1, 1),
                CriarSenha(),
                null,
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
            var resultado = Colaborador.Criar(
                "Pedro Vinicius",
                CriarCpf(),
                null!,
                CriarTelefone(),
                CriarEndereco(),
                new DateOnly(2000, 1, 1),
                new DateOnly(2025, 1, 1),
                CriarSenha(),
                null,
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
            var resultado = Colaborador.Criar(
                "Pedro Vinicius",
                CriarCpf(),
                CriarEmail(),
                null!,
                CriarEndereco(),
                new DateOnly(2000, 1, 1),
                new DateOnly(2025, 1, 1),
                CriarSenha(),
                null,
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
            var resultado = Colaborador.Criar(
                "Pedro Vinicius",
                CriarCpf(),
                CriarEmail(),
                CriarTelefone(),
                null!,
                new DateOnly(2000, 1, 1),
                new DateOnly(2025, 1, 1),
                CriarSenha(),
                null,
                EColaboradorTipo.Instrutor,
                EColaboradorVinculo.CLT);

            Assert.True(resultado.IsFailure);

            Assert.Contains(
                resultado.Notifications,
                x => x.Property == "Endereco");
        }
    }
}
