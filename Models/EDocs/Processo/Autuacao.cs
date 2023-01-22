using CIEO.Models.AcessoCidadao;
using CIEO.Models.AcessoCidadao.DTO;
using CIEO.Models.EDocs.Processo.DTO;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Text.Json;

namespace CIEO.Models.EDocs.Processo
{
    /// <summary>
    /// Ato utilizado para criar um novo processo no E-Docs.
    /// </summary>
    public class Autuacao : Ato
    {
        /// <summary>
        /// Local onde o processo será autuado.
        /// </summary>
        public Local? LocalAutuacao { get; set; }

        /// <summary>
        /// Lista de Documentos que serão juntados em uma Autuação.
        /// </summary>
        public List<AtoXDocumento>? Documentos { get; set; }

        /// <summary>
        /// Gera uma autuação sem nenhum parâmetro de entrada.
        /// </summary>
        public Autuacao()
        {

        }

        /// <summary>
        /// Gera um objeto de Autuação a partir de um objeto Processo no OnBase.<br/>
        /// APENAS NOVOS PROCESSOS.
        /// </summary>
        /// <param name="processo">Processo ao qual está vinculado a Autuação.</param>
        /// <param name="papel">Papel que irá realizar a autuação.</param>
        /// <param name="local">Local onde a autuação irá ocorrer.</param>
        internal Autuacao(Processo processo, Models.AcessoCidadao.Papel papel, Local local)
        {
            this.Id = Guid.NewGuid();
            this.Processo = processo;
            this.PapelPraticante = papel;
            this.LocalAutuacao = local;
            this.Situacao = SituacaoEnum.AguardandoEnvioEDocs;
        }

        /// <summary>
        /// Autua um novo processo no E-Docs.
        /// </summary>
        /// <param name="token">Token do Servidor Autorizado.</param>
        /// <returns>GUID de Evento</returns>
        /// <exception cref="Exception"></exception>
        internal Guid RegistrarEdocs(string token)
        {
            try
            {
                var cliente = new RestClient($"{Program.UrlBaseEdocs}processos/autuar");
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddHeader("Content-Type", "application/json-patch+json");

                var pedidoAutuacao = new AutuarEnvio(this);

                request.AddJsonBody(pedidoAutuacao);

                var response = cliente.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resposta = JsonSerializer.Deserialize<string>(response.Content);

                    if (string.IsNullOrWhiteSpace(resposta))
                    {
                        throw new Exception($"Nenhum valor retornado. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>.");
                    }
                    else
                    {
                        if (Guid.TryParse(resposta, out var guidEveto))
                        {
                            return guidEveto;
                        }
                        else
                        {
                            throw new Exception($"Não foi possível ler a GUID de Evento recebida. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                        }
                    }
                }
                else
                {
                    throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Autuação. Registrar no E-Docs.", e);
            }
        }
    }
}
