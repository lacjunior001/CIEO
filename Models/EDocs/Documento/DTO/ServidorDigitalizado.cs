using Newtonsoft.Json;
namespace CIEO.Models.EDocs.Documento.DTO
{
    /// <summary>
    /// Dados para documento digitalizado capturados por servidor. (3º Etapa)
    /// </summary>
    public class ServidorDigitalizado : DocumentoRegistrar
    {
        public ValorLegalEnum ValorLegalDocumentoConferencia { get; private set; }
        public Guid IdPapelCapturador { get; private set; }

        internal ServidorDigitalizado(Documento documento, DocumentoArquivo documentoArquivo) : base(documento, documentoArquivo)
        {
            if (documento.ValorLegal == null || documento.ValorLegal == ValorLegalEnum.Erro)
            {
                throw new Exception("Valor legal não informado.");
            }

            if (documento.AgentePublico == null)
            {
                throw new Exception("Agente público não informado.");
            }
            else if (documento.AgentePublico.Id == null)
            {
                throw new Exception("A Id deste Agente público não foi informada.");
            }
            else
            {
                IdPapelCapturador = documento.AgentePublico.AcessoCidadaoId.Value;
            }
        }
    }
}