using Newtonsoft.Json;

namespace CIEO.Models.AcessoCidadao.DTO
{
    public abstract class LocalBase
    {
        public Guid? Id { get; set; }

        [JsonProperty("descricaoTipoAgente")]
        public string Tipo { get; set; }
        public string Sigla { get; set; }
        public string Nome { get; set; }
    }
}
