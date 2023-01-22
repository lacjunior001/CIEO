using CIEO.Models.AcessoCidadao;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;

namespace CIEO.Models.EDocs.Processo
{
    public class Processo
    {
        /// <summary>
        /// GUID do Processo no OnBase PK.
        /// </summary>
        public Guid? OnBaseGUID { get; set; }

        /// <summary>
        /// Id Visual do Processo no OnBase.
        /// </summary>
        public string? OnBaseId { get; set; }

        /// <summary>
        /// GUID do processo no E-Docs.
        /// </summary>
        public Guid? EdocsGUID { get; set; }

        /// <summary>
        /// Id Visual do Processo no E-Docs.
        /// </summary>
        public string? Protocolo { get; set; }

        /// <summary>
        /// Ano em que o processo se iniciou no E-Docs.
        /// </summary>
        public int? Ano { get; set; }

        /// <summary>
        /// Resumo do processo no E-Docs.
        /// </summary>
        public string? Resumo { get; set; }

        /// <summary>
        /// Situação do processo no E-Docs.
        /// </summary>
        public SituacaoProcessoEnum? Situacao { get; set; }

        /// <summary>
        /// Classe do processo no E-Docs.
        /// </summary>
        public Classe? Classe { get; set; }

        /// <summary>
        /// Localização atual do processo no E-Docs.
        /// </summary>
        public Local? Localizacao { get; set; }

        /// <summary>
        /// Lista de interessados no processo no E-Docs.
        /// </summary>
        public List<Interessado>? Interessados { get; set; }

        /// <summary>
        /// Todo processo possui uma lista de atos.
        /// </summary>
        public List<Ato>? Atos { get; set; }

        public Processo()
        {
        }

        /// <summary>
        /// (CNHS) Constrói um processo E-Docs a partir de um Obj Empenho do OnBase.
        /// </summary>
        /// <param name="empenhoObjOnBase"></param>
        public Processo(Hyland.Unity.WorkView.Object empenhoObjOnBase, Classe clase, Local local, Papel papel)
        {
            foreach (var item in empenhoObjOnBase.AttributeValues)
            {
                switch (item.Name)
                {
                    case "AnoExercicio":
                        this.Resumo = $"Empenho {item.AlphanumericValue}";
                        break;
                    case "OnBaseGUID":
                        this.OnBaseGUID = new Guid(item.AlphanumericValue);
                        break;
                    case "OnBaseID":
                        this.OnBaseId = item.AlphanumericValue;
                        break;
                }
            }

            this.Classe = clase;
            this.Localizacao = local;

            this.Atos = new List<Ato>(1);
            var autuacao = new Autuacao(this, papel, local);
            this.Atos.Add(autuacao);

            var fornecedor = empenhoObjOnBase.GetRelatedObject("Fornecedor");

            this.Interessados = Interessado.MontarInteressados(this, fornecedor, local);

            var attValue = fornecedor.AttributeValues.Find("NomeOuRazaoSocial");
            string? razaoFornecedor = attValue.HasValue ? attValue.AlphanumericValue : null;

            attValue = fornecedor.AttributeValues.Find("NomeSocialOuNomeFantasia");
            string? nomeFantasia = attValue.HasValue ? attValue.AlphanumericValue : null;


            if (!string.IsNullOrWhiteSpace(razaoFornecedor))
            {
                this.Resumo = this.Resumo + $" - {razaoFornecedor}";
            }

            if (!string.IsNullOrWhiteSpace(nomeFantasia))
            {
                this.Resumo = this.Resumo + $" - {nomeFantasia}";
            }

            this.Resumo = this.Resumo + $" - CNH SOCIAL";
        }

        /// <summary>
        /// Atualiza os dados desta instancia com os dados do processo no E-Docs.
        /// </summary>
        /// <param name="token">Token de Aplicação ou de Usuário do Acesso Cidadão</param>
        /// <returns>True = Dados Modificados.</returns>
        /// <exception cref="Exception">Erro ao tentar baixar os dados.</exception>
        internal bool Baixar(string token)
        {
            try
            {
                if (!this.EdocsGUID.HasValue)
                {
                    throw new Exception("Este Processo não possui GUID do E-Docs.");
                }

                var cliente = new RestClient($"{Program.UrlBaseEdocs}processos/{this.EdocsGUID}");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Authorization", $"Bearer {token}");
                var resposta = cliente.Execute(request);
                if (resposta.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var respostaProcesso = JsonConvert.DeserializeObject<Models.EDocs.Processo.DTO.ProcessoResposta>(resposta.Content);
                    //this.Protocolo = respostaProcesso.Protocolo;
                    //this.Ano = Convert.ToInt32(respostaProcesso.Ano);
                    //this.Resumo = respostaProcesso.Resumo;
                    //this.Situacao = respostaProcesso.Situacao;

                    return true;
                }
                else
                {
                    if (resposta.Content.Contains("<html>"))
                    {
                        throw new Exception($"Código:<<{resposta.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Código:<<{resposta.StatusCode}>>.Conteúdo:<<{resposta.Content}>>");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Processo.Baixar.{this.EdocsGUID}", e);
            }
        }
    }
}