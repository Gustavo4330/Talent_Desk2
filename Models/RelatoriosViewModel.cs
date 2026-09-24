using TalentDesk.Repositories;

namespace TalentDesk.Models
{
    public class RelatoriosViewModel
    {
        public EstatisticasDashboard Estatisticas { get; set; } = new();
        public List<ItemContagem> PorArea { get; set; } = new();
        public List<ItemContagem> PorStatus { get; set; } = new();
        public List<ItemContagem> PorCidade { get; set; } = new();
        public List<ItemContagem> PorFormacao { get; set; } = new();
    }
}
