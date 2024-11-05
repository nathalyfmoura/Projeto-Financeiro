using Projeto.Fintec.Model;

namespace Projeto.Fintec.Servico.Interface
{
    public interface IFinanceiroServico
    {        
        Task<EmpresaRetorno> AnteciparNotasFiscaisAsync(string cnpjEmpresa, List<int> numerosNotas);
        Task<List<NotaFiscal>> BuscarNotasFiscaisAsync(string cnpj);
        Task<string> CadastrarNotaFiscalAsync(NotaFiscal notaFiscal);        
        Task<string> ExcluirNotaFiscalAsync(int numero);
    }
}
