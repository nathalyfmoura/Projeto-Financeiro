using Projeto.Fintec.Model;
using Projeto.Fintec.Servico.Interface;
using Projeto.Fintec.Repositorio.Interface;

namespace Projeto.Fintec.Servico
{
    public class FinanceiroServico : IFinanceiroServico
    {
        private readonly IFinanceiroRepositorio _financeiroRepositorio;
        private readonly IEmpresaRepositorio _empresaRepositorio;
        private readonly IEmpresaServico _empresaServico;

        private readonly List<Empresa> _empresas = new();

        public FinanceiroServico(IFinanceiroRepositorio financeiroRepositorio, IEmpresaServico empresaServico, IEmpresaRepositorio empresaRepositorio)
        {
            _financeiroRepositorio = financeiroRepositorio;
            _empresaServico = empresaServico;
            _empresaRepositorio = empresaRepositorio;
        }
        public async Task<string> CadastrarNotaFiscalAsync(NotaFiscal notaFiscal)
        {
            var empresaExistente = await _empresaServico.ObterPorCnpjAsync(notaFiscal.Cnpj);

            notaFiscal.Cnpj = empresaExistente.Cnpj;

            if (empresaExistente == null)
            {
                throw new ArgumentException($"A empresa com CNPJ {notaFiscal.Cnpj} não está cadastrada.");
            }

            if (notaFiscal.Numero <= 0)
            {
                throw new ArgumentException("O número da nota fiscal é obrigatório e deve ser maior que zero.");
            }

            if (notaFiscal.ValorBruto <= 0)
            {
                throw new ArgumentException("O valor da nota fiscal deve ser maior que zero.");
            }

            if (notaFiscal.DataVencimento <= DateTime.Today)
            {
                throw new ArgumentException("A data de vencimento deve ser maior que a data atual.");
            }

            await _financeiroRepositorio.InserirNotaFiscalAsync(notaFiscal);
            return "Nota Fiscal inserida com Sucesso!";
        }
        public async Task<List<NotaFiscal>> BuscarNotasFiscaisAsync(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                throw new ArgumentException("O CNPJ não pode ser vazio.");
            }

            return await _financeiroRepositorio.ObterNotasFiscaisPorCnpjAsync(cnpj);
        }
        public async Task<string> ExcluirNotaFiscalAsync(int numero)
        {
            try
            {
                await _financeiroRepositorio.ExcluirNotaFiscalAsync(numero);
                return "Nota Fiscal excluída com sucesso.";
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir nota fiscal: " + ex.Message);
            }
        }

        public async Task<EmpresaRetorno> AnteciparNotasFiscaisAsync(string cnpjEmpresa, List<int> numerosNotas)
        {

            // Busca da empresa pelo CNPJ e validação do CNPJ
            var empresa = await _empresaServico.ObterPorCnpjAsync(cnpjEmpresa);
            if (empresa == null)
            {
                throw new InvalidOperationException("Empresa não encontrada.");
            }

            // Verificação de duplicidade nas notas fiscais
            if (numerosNotas.Count != numerosNotas.Distinct().Count())
            {
                throw new ArgumentException("A lista de números de notas fiscais contém duplicidades.");
            }

            // Cálculo do limite de crédito e validação
            decimal limiteCredito = CalcularLimiteCredito(empresa);

            if (limiteCredito < 5000.00m)
            {
                throw new InvalidOperationException(
                    "Limite de crédito insuficiente. Para solicitar antecipação, o faturamento mensal da sua empresa deve ser superior a R$ 10.000,00.");
            }

            // Busca das notas fiscais selecionadas
            List<NotaFiscal> notasSelecionadas = await ObterNotasSelecionadasAsync(empresa.Cnpj, numerosNotas);

            if (notasSelecionadas == null || notasSelecionadas.Count == 0)
            {
                throw new InvalidOperationException("Nenhuma nota fiscal encontrada para os números fornecidos.");
            }
            foreach (var nota in notasSelecionadas)
            {
                if (nota.ValorBruto <= 0)
                {
                    throw new InvalidOperationException($"Nota fiscal com número {nota.Numero} possui valor bruto inválido.");
                }

                if (nota.DataVencimento <= DateTime.Today)
                {
                    throw new InvalidOperationException($"Nota fiscal com número {nota.Numero} possui data de vencimento expirada.");
                }
            }

            VerificarLimiteCredito(notasSelecionadas, limiteCredito);
            CalcularValorLiquidoDasNotas(notasSelecionadas);

            return MontarResultadoAntecipacao(empresa, notasSelecionadas, limiteCredito);
        }

        private async Task<Empresa> ObterEmpresaPorCnpjAsync(string cnpjEmpresa)
        {
            var empresa = await _empresaRepositorio.ObterPorCnpjAsync(cnpjEmpresa);
            if (empresa == null)
            {
                throw new Exception("Empresa não encontrada.");
            }
            return empresa;
        }

        private decimal CalcularLimiteCredito(Empresa empresa)
        {
            if (empresa.Faturamento_Mensal <= 10000)
            {
                throw new ArgumentException("O faturamento mensal é insuficiente para antecipação de crédito.");
            }
            else if (empresa.Faturamento_Mensal >= 10001 && empresa.Faturamento_Mensal <= 50000)
            {
                return empresa.Faturamento_Mensal * 0.50m;
            }
            else if (empresa.Faturamento_Mensal >= 50001 && empresa.Faturamento_Mensal <= 100000)
            {
                return empresa.Ramo_id == 1 ? empresa.Faturamento_Mensal * 0.55m : empresa.Faturamento_Mensal * 0.60m;
            }
            else // Faturamento acima de R$100.001,00
            {
                return empresa.Ramo_id == 1 ? empresa.Faturamento_Mensal * 0.60m : empresa.Faturamento_Mensal * 0.65m;
            }
        }

        private async Task<List<NotaFiscal>> ObterNotasSelecionadasAsync(string cnpjEmpresa, List<int> numerosNotas)
        {
            if (numerosNotas == null || !numerosNotas.Any())
            {
                throw new ArgumentException("A lista de números de notas está vazia.");
            }

            List<NotaFiscal> notasSelecionadas = await _financeiroRepositorio.ObterNotasFiscaisPorNumerosAsync(cnpjEmpresa, numerosNotas);

            if (!notasSelecionadas.Any())
            {
                throw new InvalidOperationException("Nenhuma nota fiscal encontrada para os números fornecidos.");
            }

            return notasSelecionadas;
        }

        private void VerificarLimiteCredito(List<NotaFiscal> notasSelecionadas, decimal LimiteCredito)
        {
            var totalBruto = notasSelecionadas.Sum(nf => nf.ValorBruto);
            if (totalBruto > LimiteCredito)
            {
                throw new Exception("Valor das notas excede o limite de crédito.");
            }
        }

        private decimal CalcularValorLiquidoDasNotas(List<NotaFiscal> notasSelecionadas)
        {
            var taxa = 0.0465m;
            decimal valorTotalAntecipado = 0;

            foreach (var notaFiscal in notasSelecionadas)
            {
                int prazo = (notaFiscal.DataVencimento - DateTime.Today).Days;

                decimal fatorDesconto = (decimal)Math.Pow((double)(1 + taxa), (double)prazo / 30);
                decimal desagio = notaFiscal.ValorBruto - (notaFiscal.ValorBruto / fatorDesconto);

                notaFiscal.ValorLiquido = Math.Round(notaFiscal.ValorBruto - desagio, 2);

                valorTotalAntecipado += notaFiscal.ValorLiquido;
            }

            return Math.Round(valorTotalAntecipado, 2);
        }

        private EmpresaRetorno MontarResultadoAntecipacao(Empresa empresa, List<NotaFiscal> notasSelecionadas, decimal limiteCredito)
        {
            var totalBruto = notasSelecionadas.Sum(nf => nf.ValorBruto);
            var totalLiquido = notasSelecionadas.Sum(nf => nf.ValorLiquido);

            return new EmpresaRetorno
            {
                Nome = empresa.Nome,
                Cnpj = empresa.Cnpj,
                LimiteCredito = limiteCredito,
                NotasFiscais = notasSelecionadas.Select(nf => new NotaFiscalRetorno
                {
                    Numero = nf.Numero,
                    ValorBruto = nf.ValorBruto,
                    ValorLiquido = nf.ValorLiquido
                }).ToList(),
                TotalBruto = totalBruto,
                TotalLiquido = totalLiquido
            };
        }

    }
}
