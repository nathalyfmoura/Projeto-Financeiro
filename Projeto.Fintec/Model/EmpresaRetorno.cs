using System.Diagnostics.CodeAnalysis;

namespace Projeto.Fintec.Model
{
    [ExcludeFromCodeCoverage]
    public class EmpresaRetorno
    {
        public required string Nome { get; set; }
        public required string Cnpj { get; set; }
        public decimal LimiteCredito { get; set; }
        public List<NotaFiscalRetorno> NotasFiscais { get; set; } = new();
        public decimal TotalBruto { get; set; }
        public decimal TotalLiquido { get; set; }
    }

    public class NotaFiscalRetorno
    {
        public int Numero { get; set; }
        public decimal ValorBruto { get; set; }
        public decimal ValorLiquido { get; set; }
    }

}

