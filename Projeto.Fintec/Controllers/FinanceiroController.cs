using Microsoft.AspNetCore.Mvc;
using Projeto.Fintec.Model;
using Projeto.Fintec.Servico.Interface;

namespace Projeto.Fintec.Controllers
{
    /// <summary>
    /// Controller responsável por operações financeiras, como cadastro e busca de notas fiscais.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    // [Authorize]
    public class FinanceiroController : ControllerBase
    {
        private readonly IFinanceiroServico _financeiroServico;


        public FinanceiroController(IFinanceiroServico AntecipacaoServico)
        {
            _financeiroServico = AntecipacaoServico;
        }

        /// <summary>
        /// Cadastra uma nova nota fiscal.
        /// </summary>
        /// <param name="nota">Dados da nota fiscal a ser cadastrada.</param>
        /// <returns>Objeto de confirmação do cadastro.</returns>
        /// <response code="200">Nota fiscal cadastrada com sucesso.</response>
        /// <response code="400">Erro de validação no cadastro da nota fiscal.</response>

        [HttpPost]
        [Route("CadastrarNota")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CadastrarNotaFiscal([FromBody] NotaFiscal nota)
        {
            try
            {
                var resultado = await _financeiroServico.CadastrarNotaFiscalAsync(nota);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Busca notas fiscais pelo CNPJ da empresa.
        /// </summary>
        /// <param name="cnpj">CNPJ da empresa para a qual as notas fiscais serão buscadas.</param>
        /// <returns>Retorna uma lista de notas fiscais associadas ao CNPJ.</returns>
        [HttpGet]
        [Route("Obter/nota-fiscal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BuscarNotasFiscais(string cnpj)
        {
            try
            {
                var notasFiscais = await _financeiroServico.BuscarNotasFiscaisAsync(cnpj);
                return Ok(notasFiscais);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao excluir nota fiscal: " + ex.Message);
            }
        }

        /// <summary>
        /// Exclui uma nota fiscal pelo número.
        /// </summary>
        /// <param name="numero">Número da nota fiscal a ser excluída.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpDelete]
        [Route("Deletar/nota-fiscal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExcluirNotaFiscal([FromQuery] int numero)
        {
            try
            {
                var mensagem = await _financeiroServico.ExcluirNotaFiscalAsync(numero);
                return Ok(mensagem);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao excluir nota fiscal: " + ex.Message);
            }
        }

        /// <summary>
        /// Calcula a antecipação de recebíveis com base nas notas fiscais fornecidas.
        /// </summary>
        /// <param name="numerosNotas">Lista de números das notas fiscais a serem antecipadas.</param>
        /// <param name="cnpjEmpresa">CNPJ da empresa solicitando a antecipação.</param>
        /// <returns>Retorna o resultado do cálculo de antecipação.</returns>
        /// <response code="200">Cálculo de antecipação realizado com sucesso.</response>
        /// <response code="400">Erro na requisição para cálculo de antecipação.</response>
        [HttpPost]
        [Route("Obter/calculo-antecipacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CalcularAntecipacao([FromBody] List<int> numerosNotas, [FromQuery] string cnpjEmpresa)
        {
            try
            {
                var resultado = await _financeiroServico.AnteciparNotasFiscaisAsync(cnpjEmpresa, numerosNotas);
            return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao Calcular antecipação. " + ex.Message);
            }
        }
    }
}

