using System.Diagnostics.CodeAnalysis;

namespace Projeto.Fintec.Model
{
    [ExcludeFromCodeCoverage]
    public class NotaFiscal    {

        public required string Cnpj { get; set; }
        public required int Numero { get; set; }
        public required decimal ValorBruto { get; set; }
        public required DateTime DataVencimento { get; set; }
        public decimal ValorLiquido { get; internal set; }
    }

}
