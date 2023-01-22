namespace CIEO.Models.EDocs.Documento.DTO
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Classe retornada ao Reservar Espaço no E-Docs para envio do Documento.(1º Etapa)
    /// </summary>
    public class DocumentoArquivo
    {
        /// <summary>
        /// Url que irá receber o aquivo.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Identificador gerado para o arquivo que será enviado.
        /// Identificador do arquivo temporário na nuvem do E-Docs no formato "AAAA/M/D/{Guid}_{Guid}.temp". Na prática esse será o nome interno do arquivo salvo na nuvem do E-Docs.
        /// </summary>

        public string IdentificadorTemporarioArquivoNaNuvem { get; set; }

        /// <summary>
        /// Valores que variam conforme o caso, maiores informações na documentação da MS.
        /// </summary>
        public IDictionary<string, string> Body { get; set; }
    }
}