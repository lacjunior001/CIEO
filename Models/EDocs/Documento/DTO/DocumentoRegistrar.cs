namespace CIEO.Models.EDocs.Documento.DTO
{
    /// <summary>
    /// Classe pai dos tipos de Envio de Documento (3º Etapa).
    /// </summary>
    public abstract class DocumentoRegistrar
    {
        /// <summary>
        /// Identificador do arquivo temporário na nuvem do E-Docs no formato "AAAA/M/D/{Guid}_{Guid}.temp". Na prática esse será o nome interno do arquivo salvo na nuvem do E-Docs
        /// </summary>
        public string IdentificadorTemporarioArquivoNaNuvem { get; private set; }

        /// <summary>
        /// Nome do arquivo que o E-Docs sugerirá para download, substituindo o nome interno na nuvem do E-Docs, no formato do identificador.
        /// </summary>
        public string NomeArquivo { get; private set; }

        /// <summary>
        /// Se o capturador de ser credenciado para visualizar o documento
        /// </summary>
        public bool CredenciarCapturador { get; private set; }

        /// <summary>
        /// Dados de restrição de acesso.
        /// </summary>
        public Models.EDocs.DTO.RestricaoAcesso RestricaoAcesso { get; private set; }

        protected DocumentoRegistrar(Models.EDocs.Documento.Documento documento, Models.EDocs.Documento.DTO.DocumentoArquivo documentoArquivo)
        {
            if (string.IsNullOrWhiteSpace(documentoArquivo.IdentificadorTemporarioArquivoNaNuvem))
            {
                throw new Exception("O identificador temporário do arquivo na nuvem não pode ser nulo ou vazio.");
            }
            else
            {
                this.IdentificadorTemporarioArquivoNaNuvem = documentoArquivo.IdentificadorTemporarioArquivoNaNuvem;
            }

            this.NomeArquivo = documento.NomeDocumento;

            if (documento.CapturadorCredenciar == null)
            {
                throw new Exception("É necessário informar se o capturador será Credenciado no documento ou não.");
            }
            else
            {
                this.CredenciarCapturador = documento.CapturadorCredenciar.Value;
            }

            if (documento.RestricaoTipo == null)
            {
                throw new Exception("Todo documento registrado no E-Docs deve ter uma restrição de acesso.");
            }
            else
            {
                this.RestricaoAcesso = new Models.EDocs.DTO.RestricaoAcesso(documento.RestricaoTipo.Value, null, null);
            }
        }
    }
}