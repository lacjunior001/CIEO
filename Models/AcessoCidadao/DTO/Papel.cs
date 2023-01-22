using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace CIEO.Models.AcessoCidadao.DTO
{
    public class Papel
    {
        public Guid? Guid { get; set; }
        public string? Nome { get; set; }
        public string? Tipo { get; set; }
        public Guid? LotacaoGuid { get; set; }
        public Guid? AgentePublicoSub { get; set; }
        public string? AgentePublicoNome { get; set; }
        public bool? Prioritario { get; set; }

        /// <summary>
        /// Baixa a lista de papeis de um cidadão no Acesso Cidadão.
        /// </summary>
        /// <param name="token">Cidadão ou Sistema</param>
        /// <param name="cidadaoId"></param>
        /// <returns></returns>
        internal static List<DTO.Papel> Baixar(string token, Guid cidadaoId)
        {
            var lista = new List<DTO.Papel>(10);

            try
            {
                var cliente = new RestClient($"https://sistemas.es.gov.br/prodest/acessocidadao.webapi/api/agentepublico/{cidadaoId}/papeis");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lista = JsonConvert.DeserializeObject<List<Models.AcessoCidadao.DTO.Papel>>(response.Content);
                }
                else
                {
                    if (response.Content.Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta do Acesso Cidadão. Código:<<{response.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Resposta do Acesso Cidadão. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Acesso Cidadão. PapelDTO. Baixar. {cidadaoId}. ", e);
            }

            return lista;
        }
    }
}
