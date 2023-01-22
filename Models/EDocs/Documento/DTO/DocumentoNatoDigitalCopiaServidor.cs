using Newtonsoft.Json;

namespace CIEO.Models.EDocs.Documento.DTO
{
    public class DocumentoNatoDigitalCopiaServidor : DocumentoRegistrar
    {
        [JsonProperty("idPapelCapturador")]
        public Guid IdPapelCapturador { get; set; }
    }
}