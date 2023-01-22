using System.ComponentModel.DataAnnotations.Schema;

namespace CIEO.Models.EDocs
{
    public class TipoRestricaoAcesso
    {
        public short? Id { get; set; }
        public string? Nome { get; set; }
        public bool? Fundamentos { get; set; }
        public bool? Classificacao { get; set; }
    }
}