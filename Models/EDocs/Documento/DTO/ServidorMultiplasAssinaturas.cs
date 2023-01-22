namespace CIEO.Models.EDocs.Documento.DTO
{
    public class ServidorMultiplasAssinaturas : DocumentoRegistrar
    {
        public Guid IdPapelCapturador { get; private set; }
        public Guid? IdClasse { get; private set; }
        public List<Guid> Assinantes { get; private set; }

        internal ServidorMultiplasAssinaturas(Documento documento, DocumentoArquivo documentoArquivo) : base(documento, documentoArquivo)
        {
            if (documento.Classe != null)
            {
                if (documento.Classe.Id == null)
                {
                    throw new Exception("A classe foi informada porem a Id desta Classe está ausente.");
                }
                else
                {
                    IdClasse = documento.Classe.AcessoCidadaoId;
                }
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

            /**
            if (documento.Assinantes == null || documento.Assinantes.Count == 0)
            {
                throw new Exception("Assinantes não informados.");
            }
            else
            {
                Assinantes = new List<Guid>(documento.Assinantes.Count);

                foreach (var assinante in documento.Assinantes)
                {
                    if (assinante.AgentePublico == null && assinante.Cidadao == null)
                    {
                        throw new Exception("A Id deste Assinante não foi informada.");
                    }
                    else if (assinante.AgentePublico == null)
                    {
                        //Se agente publico é null então o assinante é um cidadão.
                        Assinantes.Add(assinante.Cidadao.Id.Value);
                    }
                    else
                    {
                        Assinantes.Add(assinante.AgentePublico.Id.Value);
                    }
                }

            }
            */
        }
    }
}
