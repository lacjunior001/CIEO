namespace CIEO.Models.EDocs.Documento.DTO
{
    public class ServidorICPBrasil : DocumentoRegistrar
    {
        public Guid IdPapelCapturador { get; private set; }

        internal ServidorICPBrasil(Documento documento, DocumentoArquivo documentoArquivo) : base(documento, documentoArquivo)
        {
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
