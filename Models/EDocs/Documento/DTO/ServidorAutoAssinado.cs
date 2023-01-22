namespace CIEO.Models.EDocs.Documento.DTO
{
    /// <summary>
    /// Dados para documento nato-digital assinado via E-Docs capturados por servidor. (3º Etapa)
    /// </summary>
    public class ServidorAutoAssinado : DocumentoRegistrar
    {
        public Guid? IdClasse { get; private set; }
        public Guid IdPapelCapturadorAssinante { get; private set; }

        internal ServidorAutoAssinado(Documento documento, DocumentoArquivo documentoArquivo) : base(documento, documentoArquivo)
        {
            if (documento.Classe == null)
            {
                throw new Exception("Envios como servidor Auto-Assinado, precisam de classe.");
            }
            else
            {
                IdClasse = documento.Classe.AcessoCidadaoId;
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
                IdPapelCapturadorAssinante = documento.AgentePublico.AcessoCidadaoId.Value;
            }
        }
    }
}