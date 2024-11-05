using Projeto.Fintec.Model;

namespace Projeto.Fintec.Repositorio.Interface
{
    public interface IFinanceiroRepositorio
    {
        Task ExcluirNotaFiscalAsync(int numero);
        Task InserirNotaFiscalAsync(NotaFiscal notaFiscal);
        Task<List<NotaFiscal>> ObterNotasFiscaisPorCnpjAsync(string cnpj);
        Task<List<NotaFiscal>> ObterNotasFiscaisPorNumerosAsync(string cnpjEmpresa, List<int> numerosNotas);
    }
}
