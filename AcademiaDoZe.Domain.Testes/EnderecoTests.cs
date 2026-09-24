using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Testes
{
    public class EnderecoTests
    {
        [Fact]
        public void Deve_Criar_Endereco_Com_Dados_Validos()
        {
            var resultado = Endereco.Criar(
                "01001000",
                "Rua das Flores",
                "123",
                "",
                "Centro",
                "São Paulo",
                "SP");

            Assert.True(resultado.IsSuccess);

            var endereco = resultado.Value;

            Assert.Equal("01001000", endereco.Cep);
            Assert.Equal("Rua das Flores", endereco.Logradouro);
            Assert.Equal("123", endereco.Numero);
            Assert.Equal("Centro", endereco.Bairro);
            Assert.Equal("São Paulo", endereco.Cidade);
            Assert.Equal("SP", endereco.Estado);
        }
    }
}