using System.Diagnostics.CodeAnalysis;

namespace Projeto.Fintec.Model
{
    [ExcludeFromCodeCoverage]
    public class Empresa
    {
        public required string Cnpj { get; set; }
        public required string Nome { get; set; }
        public required decimal Faturamento_Mensal { get; set; } 
        public required int Ramo_id { get; set; } // 01 = Serviços ou  2 = Produtos
    }
}

