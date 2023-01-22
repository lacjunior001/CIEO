namespace CIEO.Models.EDocs.Processo.DTO
{
    public class AtoResposta
    {
        public Guid Id { get; set; }
        public DateTime DataHora { get; set; }
        public AtoRespostaTipoEnum Tipo { get; set; }
    }
}
