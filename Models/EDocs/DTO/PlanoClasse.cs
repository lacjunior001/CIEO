namespace CIEO.Models.EDocs.DTO
{
    /// <summary>
    /// DTO do Plano de Classificação (Recebido do E-Docs ao Consultar.)
    /// </summary>
    public class PlanoClasse
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public List<Classe> Classes { get; set; }
    }
}
