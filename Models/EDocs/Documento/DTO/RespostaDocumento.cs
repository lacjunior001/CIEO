namespace CIEO.Models.EDocs.Documento.DTO
{
    public class RespostaDocumento
    {
        public Guid Id { get; set; }
        public string Registro { get; set; }
        public string Nome { get; set; }
        public string Extensao { get; set; }
        public int NivelAcesso { get; set; }
    }
}
