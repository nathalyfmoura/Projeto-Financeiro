using Projeto.Fintec.Model;

namespace Projeto.Fintec.Servico.Interface
{
    public interface IEmpresaServico
    {
        Task InserirEmpresaAsync(Empresa empresa);      
        Task ExcluirAsync(string cnpj);
        Task<Empresa> ObterPorCnpjAsync(string cnpj);        
        Task<string> AtualizarEmpresaAsync(Empresa empresa);
    }
}
