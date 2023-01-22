namespace CIEO.Models.EDocs.Processo
{
    /// <summary>
    /// Relaciona um documento a um ato e informa a ordem de exibição do documento no ato.
    /// </summary>
    public class AtoXDocumento
    {
        /// <summary>
        /// Id do Relacionamento (CIEO).
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Posição do documento no ato.
        /// </summary>
        public short? Posicao { get; set; }

        /// <summary>
        /// Ato que possui este documento.
        /// </summary>
        public Ato? Ato { get; set; }

        /// <summary>
        /// Documento que está neste ato.
        /// </summary>
        public Documento.Documento? Documento { get; set; }
    }
}
