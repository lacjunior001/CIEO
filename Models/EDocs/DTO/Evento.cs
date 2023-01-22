namespace CIEO.Models.EDocs.DTO
{
    /// <summary>
    /// DTO do Evento (Recebido do E-Docs ao Consultar.)
    /// </summary>
    public class Evento
    {
        public Guid Id { get; set; }

        public Guid IdCidadao { get; set; }

        public Models.EDocs.EventoSituacaoEnum Situacao { get; set; }

        public DateTime Criacao { get; set; }

        public DateTime Conclusao { get; set; }

        public EDocs.DTO.EventoTipoEnum Tipo { get; set; }

        public Guid IdProcesso { get; set; }

        public Guid IdAto { get; set; }

        public Guid IdTermo { get; set; }

        public Guid IdEncaminhamento { get; set; }

        public Guid IdDocumento { get; set; }
    }
}
