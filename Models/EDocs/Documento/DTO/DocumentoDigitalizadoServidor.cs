using Newtonsoft.Json;
namespace CIEO.Models.EDocs.Documento.DTO
{
    /// <summary>
    /// Dados para documento digitalizado capturados por servidor. (3º Etapa)
    /// </summary>
    public class DocumentoDigitalizadoServidor : DocumentoRegistrar
    {
        public DocumentoValorLegal ValorLegalDocumentoConferencia { get; set; }
        public Guid IdPapelCapturador { get; set; }
    }
}