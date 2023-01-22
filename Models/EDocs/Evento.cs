using Hyland.Public;
using Newtonsoft.Json;
using RestSharp;

namespace CIEO.Models.EDocs
{
    /// <summary>
    /// Cada ato ou documentos enviados ao E-Docs geram um Evento.<br/>
    /// Um evento deve ser consultado para saber se o ato foi praticado/documento recebido.
    /// </summary>
    public class Evento
    {
        /// <summary>
        /// Identificador do evento (CIEO).
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Identificador do evento (E-Docs)
        /// </summary>
        public Guid? IdEDocs { get; set; }

        /// <summary>
        /// Ato ao qual este evento está relacionado.
        /// </summary>
        public Processo.Ato? AtoRelacionado { get; set; }

        /// <summary>
        /// Documento ao qual este evento está relacionado.
        /// </summary>
        public EDocs.Documento.Documento? DocumentoRelacionado { get; set; }

        /// <summary>
        /// Situação do Envento.<br/>
        /// 1-29 - E-Docs. <br/>
        /// 30 + - CIEO
        /// </summary>
        public EventoSituacaoEnum? Situacao { get; set; }

        /// <summary>
        /// Consulta a situação de um evento no E-Docs.
        /// </summary>
        /// <param name="token">Cidadão ou Sistema</param>
        /// <returns>True = Evento atualizado.</returns>
        /// <exception cref="Exception"></exception>
        internal bool Consultar(string token)
        {
            bool retorno = false;
            try
            {
                var cliente = new RestClient($"{Program.UrlBaseEdocs}eventos/{this.IdEDocs}");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Authorization", $"Bearer {token}");
                var resposta = cliente.Execute(request);

                if (resposta.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var eventoDTO = JsonConvert.DeserializeObject<DTO.Evento>(resposta.Content);

                    if (this.Situacao.Equals(eventoDTO.Situacao))
                    {
                        /** Motivo:
                         * Caso a situação de um evento seja igual significa que nada deve ser feito no banco.
                         */
                    }
                    else
                    {
                        this.Situacao = eventoDTO.Situacao;

                        if (this.Situacao.Equals(Models.EDocs.EventoSituacaoEnum.Concluido))
                        {
                            /** Motivos:
                             *  Um evento só pode estar relacionado a um ato ou a um documento.
                             *  Nunca a ambos ao mesmo tempo.
                             */

                            if (this.AtoRelacionado == null)
                            {
                                this.DocumentoRelacionado.EDocsId = eventoDTO.IdDocumento;
                                //this.DocumentoRelacionado.Situacao = SituacaoEnum.EnviadoParaEDocs;
                                this.DocumentoRelacionado.Baixar(token);
                            }
                            else
                            {
                                this.AtoRelacionado.EDocsGUID = eventoDTO.IdAto;
                                this.AtoRelacionado.Processo.EdocsGUID = eventoDTO.IdProcesso;
                                //this.AtoRelacionado.Situacao = SituacaoEnum.EnviadoParaEDocs;
                                this.AtoRelacionado.Baixar(token);
                            }
                        }
                        retorno = true;
                    }
                }
                else
                {
                    if (resposta.Content.ToUpper().Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{resposta.StatusCode}>>. Conteúdo:<<Retornou um Html>>.");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{resposta.StatusCode}>>. Conteúdo:<<{resposta.Content}>>.");
                    }
                }

                return retorno;
            }
            catch (Exception e)
            {
                this.Situacao = EventoSituacaoEnum.ErroConsulta;
                if (this.AtoRelacionado == null)
                {
                    this.DocumentoRelacionado.Situacao = SituacaoEnum.ErroConsultarEvento;
                }
                else
                {
                    this.AtoRelacionado.Situacao = SituacaoEnum.ErroConsultarEvento;
                }

                throw new Exception("E-Docs. Evento. Consulta. ", e);
            }
        }
    }
}
