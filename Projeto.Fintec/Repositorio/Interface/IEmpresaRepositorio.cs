using Projeto.Fintec.Model;

namespace Projeto.Fintec.Repositorio.Interface
{
    public interface IEmpresaRepositorio
    {
        Task AdicionarAsync(Empresa empresa);
        Task AtualizarAsync(Empresa empresa);
        Task ExcluirAsync(string cnpj);
        Task InserirEmpresaAsync(Empresa empresa);
        Task<Empresa> ObterPorCnpjAsync(string cnpj);
        
    }
}
