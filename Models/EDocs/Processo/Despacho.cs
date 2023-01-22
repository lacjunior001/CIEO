using CIEO.Models.AcessoCidadao;
using CIEO.Models.EDocs.Processo.DTO;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CIEO.Models.EDocs.Processo
{
    public class Despacho : Ato
    {
        public Local Destino { get; set; }
        public Papel Despachante { get; set; }
        public string Mensagem { get; set; }

        internal void DespacharProcesso(string? acessToken)
        {
            string? evento = null;
            try
            {
                var cliente = new RestClient($"{Program.UrlBaseEdocs}processos/despachar");
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Authorization", $"Bearer {acessToken}");
                request.AddHeader("Content-Type", "application/json-patch+json");

                var despacho = new DTO.DespacharEnvio();

                despacho.IdProcesso = Processo.EdocsGUID.Value;
                despacho.IdDestino = this.Destino.AcessoCidadaoId.Value;
                despacho.IdPapelResponsavel = this.PapelPraticante.AcessoCidadaoId.Value;
                despacho.RestricaoAcesso = new EDocs.DTO.RestricaoAcesso(RestricaoTipoEnum.Organizacional, null, null);
                despacho.Mensagem = "";

                request.AddJsonBody(despacho);

                var response = cliente.Execute(request);
                if (response.IsSuccessful)
                {
                    evento = response.Content;
                }
                else
                {
                    throw new Exception($"Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Despacho.DespacharProcesso.", e);
            }
        }
    }
}
