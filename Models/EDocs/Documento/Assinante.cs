using CIEO.Models.AcessoCidadao;

namespace CIEO.Models.EDocs.Documento
{
    /// <summary>
    /// Representa uma assinatura de um documento no E-Docs.
    /// </summary>
    public class Assinante
    {
        public long Id { get; set; }
        public DocConfigPad? DocAssiConfig { get; set; }
        public Usuario? UsuarioIndicado { get; set; }
        public Papel? PapelIndicado { get; set; }
        public bool Ativo { get; set; }
    }
}