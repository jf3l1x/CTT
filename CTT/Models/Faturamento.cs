using System;
using System.Collections.Generic;

namespace CTT.Models
{
    public class Faturamento
    {
        public string Id { get; set; }
        public DateTime Data { get; set; }
        public string Cliente { get; set; }
        public DateTime Pagamento { get; set; }
        public decimal Total { get; set; }
        public IList<OrdemServico> OrdemServicos { get; set; }
    }
}