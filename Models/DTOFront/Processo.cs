namespace CIEO.Models.DTOFront
{
    public abstract class Processo
    {
        public string? OnBaseID { get; set; }
        public List<Models.DTOFront.Documento>? Documentos { get; set; }
    }
}
