using CIEO.Models.AcessoCidadao;

namespace CIEO.Models.PGB
{
    /// <summary>
    /// Usuario do OnBase.
    /// </summary>
    public class UsuarioOB
    {
        public long? Id { get; set; }
        public string? UserName { get; set; }
        public bool? Disable { get; set; }
        public UsuOBXUsuAC? UsuarioAC { get; set; }
        public List<UserGroupOB>? UserGroups { get; set; }
    }
}