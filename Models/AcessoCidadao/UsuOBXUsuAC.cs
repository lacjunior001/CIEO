using CIEO.Models.PGB;

namespace CIEO.Models.AcessoCidadao
{
    public class UsuOBXUsuAC
    {
        public long? Id { get; set; }
        public Usuario? UsuarioAC { get; set; }
        public UsuarioOB? UsuarioOB { get; set; }
    }
}