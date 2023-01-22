using CIEO.Models.PGB;
using System.ComponentModel.DataAnnotations;

namespace CIEO.Models.DTOFront.Manager
{
    public class RelacionarContas
    {
        [Display(Name = "CPF Acesso Cidadão")]
        [MaxLength(11)]
        public string CpfAcessoCidadao { get; set; }

        [Display(Name = "Nome de usuario no OnBase")]
        [MaxLength(30)]
        public string UserNameOnBase { get; set; }
    }
}
