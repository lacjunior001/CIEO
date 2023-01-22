using CIEO.Extension;
using CIEO.Models.AcessoCidadao;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace CIEO.Models.EDocs
{
    /// <summary>
    /// Fundamentos utilizados para justificar a restrição de acesso.
    /// </summary>
    public class FundamentoLegal
    {
        /// <summary>
        /// GUID Informada pelo E-Docs para o fundamento legal.
        /// </summary>
        public Guid? Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }

        /// <summary>
        /// Ordem na lista de exibição definida pelo E-Docs.
        /// </summary>
        public int? Ordenacao { get; set; }

        /// <summary>
        /// Tipo de Restricao de acesso onde este fundamento pode ser utilizado.
        /// </summary>
        public RestricaoTipoEnum? NivelAcesso { get; set; }

        /// <summary>
        /// Agente que possui permissão para usar o Fundamento.
        /// </summary>
        public AgenteTipoEnum? Agente { get; set; }

        /// <summary>
        /// Em qual Patriarca esta Id pode ser utilizada.
        /// </summary>
        public AcessoCidadao.Local? Patriarca { get; set; }

        /// <summary>
        /// Indica se parou ou não de vir este fundamento do endpoint do E-Docs.
        /// </summary>
        public bool Ativo { get; set; }

        /// <summary>
        /// Contrói o Fundamento a partir do DTO recebido do Acesso Cidadão.
        /// </summary>
        /// <param name="fundamento">Fundamento Recebido.</param>
        /// <param name="tipoAgente">Endpoint onde foi consultado.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FundamentoLegal(Models.EDocs.DTO.FundamentoLegal fundamento, AgenteTipoEnum tipoAgente)
        {
            if (fundamento == null)
            {
                throw new ArgumentNullException("DTO.Fundamento é nulo");
            }

            Id = fundamento.Id;
            Nome = fundamento.Nome;
            Descricao = fundamento.Descricao;
            Ordenacao = fundamento.Ordenacao;
            NivelAcesso = fundamento.NivelAcesso.RestricaoTipoToEnum();
            Agente = tipoAgente;

            if (fundamento.IdPatriarca == null)
            {
                Patriarca = null;
            }
            else
            {
                Patriarca = new AcessoCidadao.Local(fundamento.IdPatriarca.Value);
            }

            Ativo = true;
        }

        public FundamentoLegal()
        {

        }

        /// <summary>
        /// Baixa os Fundamentos Legais do E-Docs.
        /// </summary>
        /// <param name="token">Cidadão ou Sistema</param>
        /// <param name="patriarcaId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static List<FundamentoLegal> Baixar(string token, Guid patriarcaId)
        {
            try
            {
                var fundamentosCidadao = BaixarCidadao(token);
                var fundamentosPatriarca = BaixarPatriarca(token, patriarcaId);

                for (int i = 0; i < fundamentosPatriarca.Count; i++)
                {
                    if (!fundamentosCidadao.Exists(p => p.Id.Equals(fundamentosPatriarca[i])))
                    {
                        fundamentosCidadao.Add(fundamentosPatriarca[i]);
                    }
                }

                return fundamentosCidadao;
            }
            catch (Exception e)
            {
                throw new Exception("E-Docs. Fundamentos Legais. Baixar.", e);
            }
        }

        /// <summary>
        /// Baixa os fundamentos do Endpoint de Cidadão.
        /// </summary>
        /// <param name="acTokenSistema"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static List<Models.EDocs.FundamentoLegal> BaixarCidadao(string token)
        {
            try
            {
                var fundamentos = new List<Models.EDocs.FundamentoLegal>();
                var cliente = new RestClient($"{Program.UrlBaseEdocs}fundamentos-legais");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var fundamentosDTO = JsonConvert.DeserializeObject<List<DTO.FundamentoLegal>>(response.Content);
                    for (int i = 0; i < fundamentosDTO.Count; i++)
                    {
                        fundamentos.Add(new Models.EDocs.FundamentoLegal(fundamentosDTO[i], AgenteTipoEnum.Cidadao));
                    }
                }
                else
                {
                    if (response.Content.ToUpper().Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>. Conteúdo:<<Retornou um Html>>.");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>. Conteúdo:<<{response.Content}>>.");
                    }
                }

                return fundamentos;
            }
            catch (Exception e)
            {
                throw new Exception("Cidadão. ", e);
            }
        }

        /// <summary>
        /// Baixa fundamentos do Endpoint de patriarca.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="patriarcaId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static List<Models.EDocs.FundamentoLegal> BaixarPatriarca(string token, Guid? patriarcaId)
        {
            try
            {
                if (patriarcaId == null)
                {
                    throw new Exception("A Id do Governo do estado não foi recebida.");
                }

                var fundamentos = new List<Models.EDocs.FundamentoLegal>();

                var cliente = new RestClient($"{Program.UrlBaseEdocs}fundamentos-legais/{patriarcaId}");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var fundamentosDTO = JsonConvert.DeserializeObject<List<DTO.FundamentoLegal>>(response.Content);

                    for (int i = 0; i < fundamentosDTO.Count; i++)
                    {
                        fundamentos.Add(new Models.EDocs.FundamentoLegal(fundamentosDTO[i], AgenteTipoEnum.NaoIdentificado));
                    }
                }
                else
                {
                    if (response.Content.ToUpper().Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>. Conteúdo:<<Retornou um Html>>.");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>. Conteúdo:<<{response.Content}>>.");
                    }
                }
                return fundamentos;
            }
            catch (Exception e)
            {
                throw new Exception("Patriarca. ", e);
            }
        }
    }

}
