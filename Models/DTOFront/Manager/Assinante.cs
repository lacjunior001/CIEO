using CIEO.Models.EDocs.Documento;
using CIEO.Models.EDocs;
using CIEO.Models.PGB;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using CIEO.Models.AcessoCidadao;

namespace CIEO.Models.DTOFront.Manager
{
    public class Assinante
    {
        [Display(Name = "Id")]
        public long? Id { get; set; }

        [Display(Name = "Id da configuração padrão do documento")]
        [Required]
        public long DocConfigPadId { get; set; }

        [Display(Name = "CPF")]
        [Required]
        public string CPFAssinante { get; set; }

        [Display(Name = "GUID do Assinante")]
        public Guid? GUIDAssinante { get; set; }

        [Display(Name = "Papel Indicado do Assinante")]
        public long? PapelId { get; set; }
        public List<Papel> Papeis { get; set; }

        [Display(Name = "Ativa")]
        public bool Ativo { get; set; }
    }
}
