using System;
using System.Collections.Generic;
using System.Linq;

namespace CTT.Models
{
    public class OrdemServico
    {
        
        public DateTime Data { get; set; }
        public string Id { get; set; }
        public string ProjetoId { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public IList<Activity> Atividades { get; set; }
        public bool Aprovado { get; set; }
        public string UserId { get; set; }
        public IEnumerable<TempoDeServico> Totais(IEnumerable<Service> references )
        {
            var retval = new Dictionary<string,TempoDeServico>();
            foreach (var atividade in Atividades)
            {
                
                if (retval.ContainsKey(atividade.ServiceId))
                {
                    retval[atividade.ServiceId].Tempo = retval[atividade.ServiceId].Tempo.Add(atividade.TotalTime);
                }
                else
                {
                    var service = references.FirstOrDefault(x => x.Id == atividade.ServiceId);
                    retval.Add(atividade.ServiceId, new TempoDeServico() { Service = service, Tempo = atividade.TotalTime });
                }
            }
            return retval.Values;
        }
    }
    
    public class TempoDeServico
    {
        public Service Service { get; set; }
        public TimeSpan Tempo { get; set; }
    }
}