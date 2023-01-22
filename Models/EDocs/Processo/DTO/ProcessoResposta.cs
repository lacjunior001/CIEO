namespace CIEO.Models.EDocs.Processo.DTO
{
    public class ProcessoResposta
    {
        public Guid Id { get; set; }
        public string? Ano { get; set; }
        public string? Protocolo { get; set; }
        public string? Resumo { get; set; }
        public SituacaoProcessoEnum? Situacao { get; set; }
    }
}