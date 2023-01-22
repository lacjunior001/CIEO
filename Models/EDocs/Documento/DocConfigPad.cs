using CIEO.Models.AcessoCidadao;
using CIEO.Models.AcessoCidadao.DTO;
using CIEO.Servicos;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http.Headers;

namespace CIEO.Models.EDocs.Documento
{
    /// <summary>
    /// Encapsula todas informações pertinentes para enviar ou receber um documento do E-Docs.
    /// </summary>
    public class DocConfigPad
    {
        public long? Id { get; set; }
        public string? TipoDocumental { get; set; }
        public string? CategoriaAnexo { get; set; }
        public string? Nome { get; set; }
        public string? Extensao { get; set; }
        public bool? Credenciar { get; set; }
        public AssinaturaTipoEnum? AssinaturaTipo { get; set; }
        public List<Assinante>? Assinantes { get; set; }
        public DocumentoTipoEnum? DocumentoTipo { get; set; }
        public RestricaoTipoEnum? RestricaoTipo { get; set; }
        public ValorLegalEnum? ValorLegal { get; set; }
        public Classe? Classe { get; set; }
        public bool Ativo { get; set; }
    }
}
