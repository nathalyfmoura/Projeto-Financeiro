using System.Text.RegularExpressions;
using Projeto.Fintec.Model;
using Projeto.Fintec.Repositorio.Interface;
using Projeto.Fintec.Servico.Interface;

namespace Projeto.Fintec.Servico
{
    public class EmpresaServico:IEmpresaServico
    {
        private readonly IEmpresaRepositorio _empresaRepositorio;

        public EmpresaServico(IEmpresaRepositorio empresaRepositorio)
        {
            _empresaRepositorio = empresaRepositorio;
        }

        public async Task InserirEmpresaAsync(Empresa empresa)
        {
            if (empresa == null)
            {
                throw new ArgumentNullException(nameof(empresa), "A empresa não pode ser nula.");
            }

            empresa.Cnpj = RemoverCaracteresEspeciais(empresa.Cnpj);

            if (string.IsNullOrWhiteSpace(empresa.Nome))
            {
                throw new ArgumentNullException(nameof(empresa.Nome), "O nome da empresa é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(empresa.Cnpj))
            {
                throw new ArgumentNullException(nameof(empresa.Cnpj), "O CNPJ da empresa é obrigatório.");
            }

            if (!CnpjValido(empresa.Cnpj))
            {
                throw new InvalidOperationException($"O CNPJ {empresa.Cnpj} é inválido.");
            }   

            var empresaExistente = await _empresaRepositorio.ObterPorCnpjAsync(empresa.Cnpj);
            if (empresaExistente != null)
            {
                throw new InvalidOperationException($"Uma empresa com o CNPJ {empresa.Cnpj} já está cadastrada.");
            }

            await _empresaRepositorio.InserirEmpresaAsync(empresa);
        }

        private bool CnpjValido(string cnpj)
        {
            if (cnpj.Length != 14 || !cnpj.All(char.IsDigit))
            {
                return false;
            }

            
            int soma = 0;
            int peso = 5;

            for (int i = 0; i < 12; i++)
            {
                soma += (cnpj[i] - '0') * peso;
                peso = (peso == 2) ? 9 : peso - 1;
            }

            int resto = soma % 11;
            int digito1 = (resto < 2) ? 0 : 11 - resto;

            soma = 0;
            peso = 6;

            for (int i = 0; i < 13; i++)
            {
                soma += (cnpj[i] - '0') * peso;
                peso = (peso == 2) ? 9 : peso - 1;
            }

            resto = soma % 11;
            int digito2 = (resto < 2) ? 0 : 11 - resto;
            
            return cnpj[12] == (char)(digito1 + '0') && cnpj[13] == (char)(digito2 + '0');
        }



        public async Task<string> AtualizarEmpresaAsync(Empresa empresa)
        {
            empresa.Cnpj = RemoverCaracteresEspeciais(empresa.Cnpj);
           
            var empresaExistente = await _empresaRepositorio.ObterPorCnpjAsync(empresa.Cnpj);
            if (empresaExistente == null)
            {
                throw new InvalidOperationException($"Empresa com CNPJ {empresa.Cnpj} não encontrada.");
            }

            if (!CnpjValido(empresa.Cnpj))
            {
                throw new ArgumentException("CNPJ inválido.");
            }
            if (string.IsNullOrWhiteSpace(empresa.Nome))
            {
                throw new ArgumentException("O nome da empresa é obrigatório.");
            }

            await _empresaRepositorio.AtualizarAsync(empresa);

            return "Dados da empresa atualizados com sucesso.";
        }
        private string RemoverCaracteresEspeciais(string entrada)
        {
            return Regex.Replace(entrada, @"[^\d]", string.Empty);
        }


        public async Task ExcluirAsync(string cnpj)
        {
            var cnpjLimpo = RemoverCaracteresEspeciais(cnpj);
            await _empresaRepositorio.ExcluirAsync(cnpjLimpo);
        }

        public async Task<Empresa> ObterPorCnpjAsync(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                throw new ArgumentException("CNPJ não pode ser nulo ou vazio.", nameof(cnpj));
            }

            string cnpjLimpo = RemoverCaracteresEspeciais(cnpj);
           
            if (!CnpjValido(cnpjLimpo))
            {
                throw new ArgumentException("CNPJ inválido.", nameof(cnpj));
            }
           
            var retorno = await _empresaRepositorio.ObterPorCnpjAsync(cnpjLimpo);
            return retorno;
        }

    }
}