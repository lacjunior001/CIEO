namespace CIEO.Models.EDocs.DTO
{
    /// <summary>
    /// DTO do Fundamento utilizados para justificar a restrição de acesso.<br/>
    /// Recebido ao consultar fundamentos.
    /// </summary>
    public class FundamentoLegal
    {
        public Guid? Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public int? Ordenacao { get; set; }
        public string? NivelAcesso { get; set; }
        public Guid? IdPatriarca { get; set; }
    }

}
