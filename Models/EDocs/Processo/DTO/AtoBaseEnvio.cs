using System.Text.Json.Serialization;

namespace CIEO.Models.EDocs.Processo.DTO
{
    /// <summary>
    /// Classe base dos atos de processo no E-Docs. 
    /// </summary>
    public abstract class AtoBaseEnvio
    {
        /// <summary>
        /// Id do papel do servidor responsável pelo ato - o papel deve ter essa permissão de autuar na Organização.
        /// </summary>
        public Guid IdPapelResponsavel { get; set; }

        /// <summary>
        /// Id do processo onde o ato será praticado.
        /// </summary>
        public Guid IdProcesso { get; set; }

        /// <summary>
        /// Lista dos ids de documentos que serão entranhados.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Guid> IdsDocumentosEntranhados { get; set; }

        /// <summary>
        /// Informações sobre quem pode ou não visualizar um documento.
        /// </summary>
        public Models.EDocs.DTO.RestricaoAcesso? RestricaoAcesso { get; set; }
    }
}
