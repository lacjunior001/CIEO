using System.ComponentModel.DataAnnotations.Schema;

namespace CIEO.Models.EDocs.Documento
{
    public class TipoDocumento
    {
        public short? Id { get; set; }
        public string? Nome { get; set; }
    }
}