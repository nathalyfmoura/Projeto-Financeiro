using Dapper;
using Projeto.Fintec.Model;
using Projeto.Fintec.Repositorio.Interface;
using System.Data;

namespace Projeto.Fintec.Repositorio
{
    public class FinanceiroRepositorio : IFinanceiroRepositorio
    {
        private readonly IDbConnection _dbConnection;

        public FinanceiroRepositorio(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task InserirNotaFiscalAsync(NotaFiscal notaFiscal)
        {
            var query = @"
                INSERT INTO NotaFiscal (Cnpj, Numero, ValorBruto, DataVencimento)
                VALUES (@Cnpj, @Numero, @ValorBruto, @DataVencimento);";

            await _dbConnection.ExecuteAsync(query, notaFiscal);
        }

        public async Task<List<NotaFiscal>> ObterNotasFiscaisPorCnpjAsync(string cnpj)
        {
            var query = "SELECT * FROM NotaFiscal WHERE Cnpj = @Cnpj;";
            var notasFiscais = await _dbConnection.QueryAsync<NotaFiscal>(query, new { Cnpj = cnpj });
            return notasFiscais.AsList();
        }

        public async Task ExcluirNotaFiscalAsync(int numero)
        {
            var query = "DELETE FROM NotaFiscal WHERE Numero = @Numero;";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Numero = numero });

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Nota Fiscal com número {numero} não encontrada.");
            }
        }

        public async Task<List<NotaFiscal>> ObterNotasFiscaisPorNumerosAsync(string cnpjEmpresa, List<int> numerosNotas)
        {
            var sql = @"
            SELECT *
            FROM NOTAFISCAL
            WHERE Cnpj = @CnpjEmpresa
            AND Numero IN @NumerosNotas";

            var result = await _dbConnection.QueryAsync<NotaFiscal>(sql, new { CnpjEmpresa = cnpjEmpresa, NumerosNotas = numerosNotas });
            return result.ToList();
        }
    }
}
