using Dapper;
using Projeto.Fintec.Model;
using Projeto.Fintec.Repositorio.Interface;
using System.Data;

namespace Projeto.Fintec.Repositorio
{
    public class EmpresaRepositorio : IEmpresaRepositorio
    {
        private readonly IDbConnection _dbConnection;

        public EmpresaRepositorio(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task InserirEmpresaAsync(Empresa empresa)
        {
            var query = @"
                INSERT INTO Empresa (Cnpj, Ramo_id, Faturamento_Mensal, Nome)
                VALUES (@Cnpj, @Ramo_id, @Faturamento_Mensal, @Nome);";

            await _dbConnection.ExecuteAsync(query, empresa);
        }

        public async Task<Empresa> ObterPorCnpjAsync(string cnpj)
        {
            var query = "SELECT * FROM Empresa WHERE Cnpj = @Cnpj;";
          
             return await _dbConnection.QueryFirstOrDefaultAsync<Empresa>(query, new { Cnpj = cnpj });
        }

        public async Task AdicionarAsync(Empresa empresa)
        {
            var query = @"
                INSERT INTO Empresa (Cnpj, Ramo_id, Faturamento_Mensal, Nome)
                VALUES (@Cnpj, @Ramo_id, @Faturamento_Mensal, @Nome);";

            await _dbConnection.ExecuteAsync(query, empresa);
        }

        public async Task AtualizarAsync(Empresa empresa)
        {
            var query = @"
                UPDATE Empresa
                SET Ramo_id = @Ramo_id,
                    Faturamento_Mensal = @Faturamento_Mensal,
                    Nome = @Nome
                WHERE Cnpj = @Cnpj;";

            var rowsAffected = await _dbConnection.ExecuteAsync(query, empresa);

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException("A empresa não foi encontrada ou não foi atualizada.");
            }
        }

        public async Task ExcluirAsync(string cnpj)
        {
            var query = "DELETE FROM Empresa WHERE Cnpj = @Cnpj;";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Cnpj = cnpj });

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Empresa com CNPJ {cnpj} não encontrada.");
            }
        }
    }
}
