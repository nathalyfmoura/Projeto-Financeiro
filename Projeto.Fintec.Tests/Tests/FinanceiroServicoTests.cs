using Moq;
using Projeto.Fintec.Model;
using Projeto.Fintec.Repositorio.Interface;
using Projeto.Fintec.Servico;
using Projeto.Fintec.Servico.Interface;

namespace Projeto.Fintec.Testes.Servico
{
    public class FinanceiroServicoTests
    {
        private readonly Mock<IFinanceiroRepositorio> _financeiroRepositorioMock;
        private readonly Mock<IEmpresaServico> _empresaServicoMock;
        private readonly Mock<IEmpresaRepositorio> _empresaRepositorioMock;
        private readonly FinanceiroServico _financeiroServico;

        public FinanceiroServicoTests()
        {
            _financeiroRepositorioMock = new Mock<IFinanceiroRepositorio>();
            _empresaServicoMock = new Mock<IEmpresaServico>();
            _empresaRepositorioMock = new Mock<IEmpresaRepositorio>();
            _financeiroServico = new FinanceiroServico(_financeiroRepositorioMock.Object, _empresaServicoMock.Object, _empresaRepositorioMock.Object);
        }

        [Fact]
        public async Task CadastrarNotaFiscalAsync_ValidData_InsertsNotaFiscal()
        {
            // Arrange
            var notaFiscal = new NotaFiscal { Cnpj = "valid-cnpj", Numero = 1, ValorBruto = 1000, DataVencimento = DateTime.Today.AddDays(10) };
            _empresaServicoMock.Setup(s => s.ObterPorCnpjAsync(notaFiscal.Cnpj)).
                ReturnsAsync(new Empresa() { Cnpj = "12345678000195", Nome = "Teste", Faturamento_Mensal = 100, Ramo_id = 1 });
            _financeiroRepositorioMock.Setup(s => s.InserirNotaFiscalAsync(notaFiscal)).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroServico.CadastrarNotaFiscalAsync(notaFiscal);

            // Assert
            Assert.Equal("Nota Fiscal inserida com Sucesso!", result);
        }

        [Fact]
        public async Task CadastrarNotaFiscalAsync_InvalidNumero_ThrowsArgumentException()
        {
            // Arrange
            var notaFiscal = new NotaFiscal { Cnpj = "valid-cnpj", Numero = 0, ValorBruto = 1000, DataVencimento = DateTime.Today.AddDays(10) };
            _empresaServicoMock.Setup(s => s.ObterPorCnpjAsync(notaFiscal.Cnpj)).
                ReturnsAsync(new Empresa() { Cnpj = "12345678000195", Nome = "Teste", Faturamento_Mensal = 100, Ramo_id = 1 });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroServico.CadastrarNotaFiscalAsync(notaFiscal));
            Assert.Equal("O número da nota fiscal é obrigatório e deve ser maior que zero.", exception.Message);
        }

        [Fact]
        public async Task CadastrarNotaFiscalAsync_InvalidValorBruto_ThrowsArgumentException()
        {
            // Arrange
            var notaFiscal = new NotaFiscal { Cnpj = "valid-cnpj", Numero = 1, ValorBruto = 0, DataVencimento = DateTime.Today.AddDays(10) };
            _empresaServicoMock.Setup(s => s.ObterPorCnpjAsync(notaFiscal.Cnpj)).
                ReturnsAsync(new Empresa() { Cnpj = "12345678000195", Nome = "Teste", Faturamento_Mensal = 100, Ramo_id = 1 });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroServico.CadastrarNotaFiscalAsync(notaFiscal));
            Assert.Equal("O valor da nota fiscal deve ser maior que zero.", exception.Message);
        }

        [Fact]
        public async Task CadastrarNotaFiscalAsync_InvalidDataVencimento_ThrowsArgumentException()
        {
            // Arrange
            var notaFiscal = new NotaFiscal { Cnpj = "valid-cnpj", Numero = 1, ValorBruto = 1000, DataVencimento = DateTime.Today.AddDays(-1) };
            _empresaServicoMock.Setup(s => s.ObterPorCnpjAsync(notaFiscal.Cnpj)).
                ReturnsAsync(new Empresa() { Cnpj = "12345678000195", Nome = "Teste", Faturamento_Mensal = 100, Ramo_id = 1 });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroServico.CadastrarNotaFiscalAsync(notaFiscal));
            Assert.Equal("A data de vencimento deve ser maior que a data atual.", exception.Message);
        }

        [Fact]
        public async Task BuscarNotasFiscaisAsync_ValidCnpj_ReturnsNotasFiscais()
        {
            // Arrange
            var cnpj = "valid-cnpj";
            var notasFiscais = new List<NotaFiscal>
            {
                new NotaFiscal { Numero = 1, Cnpj = cnpj, ValorBruto = 1000, DataVencimento = DateTime.Today.AddDays(10) }
            };
            _financeiroRepositorioMock.Setup(s => s.ObterNotasFiscaisPorCnpjAsync(cnpj)).ReturnsAsync(notasFiscais);

            // Act
            var result = await _financeiroServico.BuscarNotasFiscaisAsync(cnpj);

            // Assert
            Assert.Equal(notasFiscais.Count, result.Count);
        }

        [Fact]
        public async Task BuscarNotasFiscaisAsync_EmptyCnpj_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroServico.BuscarNotasFiscaisAsync(string.Empty));
            Assert.Equal("O CNPJ não pode ser vazio.", exception.Message);
        }

        [Fact]
        public async Task ExcluirNotaFiscalAsync_ValidNumero_ReturnsSuccessMessage()
        {
            // Arrange
            var numero = 1;
            _financeiroRepositorioMock.Setup(s => s.ExcluirNotaFiscalAsync(numero)).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroServico.ExcluirNotaFiscalAsync(numero);

            // Assert
            Assert.Equal("Nota Fiscal excluída com sucesso.", result);
        }

        [Fact]
        public async Task ExcluirNotaFiscalAsync_InvalidNumero_ThrowsArgumentException()
        {
            // Arrange
            var numero = 1;
            _financeiroRepositorioMock.Setup(s => s.ExcluirNotaFiscalAsync(numero)).ThrowsAsync(new InvalidOperationException("Nota não encontrada."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroServico.ExcluirNotaFiscalAsync(numero));
            Assert.Equal("Nota não encontrada.", exception.Message);
        }
    }
}