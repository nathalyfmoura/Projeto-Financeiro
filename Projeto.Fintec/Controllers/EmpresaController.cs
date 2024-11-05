using Microsoft.AspNetCore.Mvc;
using Projeto.Fintec.Model;
using Projeto.Fintec.Servico.Interface;

namespace Projeto.Fintec.Controllers
{

    /// <summary>
    /// Controller responsável por gerenciar informações da empresa como: cadastro, edição de dados e exclusão.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EmpresaController : ControllerBase
    {
        private readonly IEmpresaServico _empresaServico;


        public EmpresaController(IEmpresaServico empresaServico)
        {
            _empresaServico = empresaServico;
        }

        /// <summary>
        /// Cadastro de empresas.
        /// </summary>
        [HttpPost]
        [Route("Cadastrar")]
        public async Task<IActionResult> InserirEmpresa([FromBody] Empresa empresa)
        {
            if (empresa == null)
            {
                return BadRequest("Dados da empresa não podem ser nulos.");
            }
            try
            {
                await _empresaServico.InserirEmpresaAsync(empresa);
                return CreatedAtAction(nameof(ObterPorCnpj), new { cnpj = empresa.Cnpj }, empresa);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Edição de dados da empresas.
        /// </summary>
        [HttpPut]
        [Route("Editar")]
        public async Task<IActionResult> AtualizarEmpresa([FromBody] Empresa empresa)
        {
            if (empresa == null)
            {
                return BadRequest("Dados da empresa não podem ser nulos.");
            }

            try
            {
                var mensagem = await _empresaServico.AtualizarEmpresaAsync(empresa);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Exclusão de empresas.
        /// </summary>
        [HttpDelete]
        [Route("Deletar")]
        public async Task<IActionResult> ExcluirEmpresa(string cnpj)
        {
            try
            {
                await _empresaServico.ExcluirAsync(cnpj);
                return NoContent(); 
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Busca de empresa e seus dados.
        /// </summary>
        [HttpGet]
        [Route("Obter/empresa")]
        public async Task<IActionResult> ObterPorCnpj([FromQuery] string cnpj)
        {
            try
            {
                var empresa = await _empresaServico.ObterPorCnpjAsync(cnpj);
                return Ok(empresa); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); 
            }
        }
    }
}
