using CIEO.Models.EDocs.Documento;
using CIEO.Models.EDocs;
using CIEO.Models.PGB;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CIEO.Models.DTOFront.Manager
{
    public class DocConficPad
    {
        [Display(Name = "Id")]
        public long? Id { get; set; }

        [Display(Name = "Tipo Documental")]
        [MaxLength(100)]
        [Required]
        public string TipoDocumental { get; set; }
        public List<string> TiposDocumentais { get; set; }

        [Display(Name = "Categoria do Anexo")]
        [MaxLength(255)]
        public string? CategoriaAnexo { get; set; }

        [Display(Name = "Nome Documento no E-Docs")]
        [MaxLength(100)]
        [Required]
        public string Nome { get; set; }

        [Display(Name = "Extensao")]
        [MaxLength(5)]
        [Required]
        public string? Extensao { get; set; }

        [Display(Name = "Credenciar o Capturador")]
        [Required]
        public bool? Credenciar { get; set; }

        [Display(Name = "Tipo de Assinatura")]
        [Required]
        public short AssinaturaTipoId { get; set; }
        public List<SelectListItem> AssinaturaTipo { get; set; }
        public List<Assinante> Assinantes { get; set; }

        [Display(Name = "Tipo de Documento")]
        [Required]
        public short DocumentoTipoId { get; set; }
        public List<SelectListItem> DocumentoTipo { get; set; }

        [Display(Name = "Tipo de Restrição de Acesso")]
        [Required]
        public short RestricaoTipoId { get; set; }
        public List<SelectListItem> RestricaoTipo { get; set; }

        [Display(Name = "Valor Legal")]
        public short ValorLegalId { get; set; }
        public List<SelectListItem> ValorLegal { get; set; }

        [Display(Name = "Classe do Documento")]
        public long? ClasseId { get; set; }
        public List<Classe> Classes { get; set; }

        [Display(Name = "Ativa")]
        public bool Ativo { get; set; }
    }
}
