using Moq;
using Projeto.Fintec.Model;
using Projeto.Fintec.Repositorio.Interface;
using Projeto.Fintec.Servico;

namespace Projeto.Fintec.Testes.Servico
{
    public class EmpresaServicoTests
    {
        private readonly Mock<IEmpresaRepositorio> _empresaRepositorioMock;
        private readonly EmpresaServico _empresaServico;

        public EmpresaServicoTests()
        {
            _empresaRepositorioMock = new Mock<IEmpresaRepositorio>();
            _empresaServico = new EmpresaServico(_empresaRepositorioMock.Object);
        }

        [Fact]
        public async Task InserirEmpresaAsync_ThrowsArgumentNullException_WhenEmpresaIsNull()
        {
            // Arrange
            Empresa empresa = null;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _empresaServico.InserirEmpresaAsync(empresa));
            Assert.Equal("A empresa não pode ser nula. (Parameter 'empresa')", exception.Message);
        }

        [Fact]
        public async Task InserirEmpresaAsync_ThrowsArgumentNullException_WhenNomeIsEmpty()
        {
            // Arrange
            var empresa = new Empresa { Cnpj = "12345678000195", Nome = "", Ramo_id = 1, Faturamento_Mensal = 10000 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _empresaServico.InserirEmpresaAsync(empresa));
            Assert.Equal("O nome da empresa é obrigatório. (Parameter 'Nome')", exception.Message);
        }

        [Fact]
        public async Task InserirEmpresaAsync_ThrowsInvalidOperationException_WhenCnpjIsInvalid()
        {
            // Arrange
            var empresa = new Empresa { Cnpj = "1234567800019X", Nome = "Test Company", Ramo_id = 1, Faturamento_Mensal = 10000 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _empresaServico.InserirEmpresaAsync(empresa));
            Assert.Equal("O CNPJ 1234567800019 é inválido.", exception.Message);
        }

        [Fact]
        public async Task InserirEmpresaAsync_ThrowsInvalidOperationException_WhenEmpresaExists()
        {
            // Arrange
            var empresa = new Empresa { Cnpj = "12345678000195", Nome = "Test Company", Ramo_id = 1, Faturamento_Mensal = 10000 };
            _empresaRepositorioMock.Setup(x => x.ObterPorCnpjAsync(empresa.Cnpj)).ReturnsAsync(empresa);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _empresaServico.InserirEmpresaAsync(empresa));
            Assert.Equal($"Uma empresa com o CNPJ {empresa.Cnpj} já está cadastrada.", exception.Message);
        }

        [Fact]
        public async Task InserirEmpresaAsync_CallsRepositorioMethod_WhenInputIsValid()
        {
            // Arrange
            var empresa = new Empresa { Cnpj = "12345678000195", Nome = "Test Company", Ramo_id = 1, Faturamento_Mensal = 10000 };
            _empresaRepositorioMock.Setup(x => x.ObterPorCnpjAsync(empresa.Cnpj)).ReturnsAsync((Empresa)null);

            // Act
            await _empresaServico.InserirEmpresaAsync(empresa);

            // Assert
            _empresaRepositorioMock.Verify(x => x.InserirEmpresaAsync(empresa), Times.Once);
        }

        [Fact]
        public async Task AtualizarEmpresaAsync_ThrowsInvalidOperationException_WhenEmpresaDoesNotExist()
        {
            // Arrange
            var empresa = new Empresa { Cnpj = "12345678000195", Nome = "Updated Company", Ramo_id = 1, Faturamento_Mensal = 10000 };
            _empresaRepositorioMock.Setup(x => x.ObterPorCnpjAsync(empresa.Cnpj)).ReturnsAsync((Empresa)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _empresaServico.AtualizarEmpresaAsync(empresa));
            Assert.Equal($"Empresa com CNPJ {empresa.Cnpj} não encontrada.", exception.Message);
        }

        [Fact]
        public async Task AtualizarEmpresaAsync_CallsRepositorioMethod_WhenInputIsValid()
        {
            // Arrange
            var empresa = new Empresa { Cnpj = "12345678000195", Nome = "Updated Company", Ramo_id = 1, Faturamento_Mensal = 10000 };
            _empresaRepositorioMock.Setup(x => x.ObterPorCnpjAsync(empresa.Cnpj)).ReturnsAsync(empresa);

            // Act
            await _empresaServico.AtualizarEmpresaAsync(empresa);

            // Assert
            _empresaRepositorioMock.Verify(x => x.AtualizarAsync(empresa), Times.Once);
        }

        [Fact]
        public async Task ExcluirAsync_CallsRepositorioMethod_WhenCnpjIsValid()
        {
            // Arrange
            var cnpj = "12345678000195";

            // Act
            await _empresaServico.ExcluirAsync(cnpj);

            // Assert
            _empresaRepositorioMock.Verify(x => x.ExcluirAsync(cnpj), Times.Once);
        }

        [Fact]
        public async Task ObterPorCnpjAsync_ThrowsArgumentException_WhenCnpjIsEmpty()
        {
            // Arrange
            string cnpj = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _empresaServico.ObterPorCnpjAsync(cnpj));
            Assert.Equal("CNPJ não pode ser nulo ou vazio. (Parameter 'cnpj')", exception.Message);
        }

        [Fact]
        public async Task ObterPorCnpjAsync_ThrowsArgumentException_WhenCnpjIsInvalid()
        {
            // Arrange
            var cnpj = "1234567800019X";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _empresaServico.ObterPorCnpjAsync(cnpj));
            Assert.Equal("CNPJ inválido. (Parameter 'cnpj')", exception.Message);
        }

        [Fact]
        public async Task ObterPorCnpjAsync_ReturnsEmpresa_WhenCnpjIsValid()
        {
            // Arrange
            var cnpj = "12345678000195";
            var empresa = new Empresa { Cnpj = cnpj, Nome = "Test Company", Ramo_id = 1, Faturamento_Mensal = 10000 };
            _empresaRepositorioMock.Setup(x => x.ObterPorCnpjAsync(cnpj)).ReturnsAsync(empresa);

            // Act
            var result = await _empresaServico.ObterPorCnpjAsync(cnpj);

            // Assert
            Assert.Equal(empresa, result);
        }
    }
}
