using CIEO.Models.AcessoCidadao.DTO;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace CIEO.Models.EDocs
{
    /// <summary>
    /// Classe de um Documento ou Processo no E-Docs.<br/>
    /// Identifica o tipo do documento ou processo.
    /// </summary>
    public class Classe
    {
        public long? Id { get; set; }

        /// <summary>
        /// Id da Classe no E-Docs.
        /// </summary>
        public Guid? AcessoCidadaoId { get; set; }

        /// <summary>
        /// Código da Classe no E-Docs.
        /// </summary>
        public string? Codigo { get; set; }

        /// <summary>
        /// Nome da Classe no E-Docs.
        /// </summary>
        public string? Nome { get; set; }

        public string? Observacao { get; set; }

        /// <summary>
        /// Caso deixe de vir nas atualizações semanais de informação deverá virar false.
        /// </summary>
        public bool? Ativo { get; set; }

        public Classe()
        {
        }

        public Classe(DTO.Classe dtoClasse)
        {
            this.AcessoCidadaoId = dtoClasse.Id;
            this.Ativo = dtoClasse.Ativo;
            if (this.Ativo == null)
            {
                this.Ativo = true;
            }

            this.Codigo = dtoClasse.Codigo;
            this.Nome = dtoClasse.Nome;
            this.Observacao = dtoClasse.Observacao;
        }

        /// <summary>
        /// Baixa a lista de classes disponíveis no E-Docs.
        /// </summary>
        /// <param name="token">Cidadão ou Sistema</param>
        /// <param name="patriarcaID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static List<Classe> Baixar(string token, Guid patriarcaID)
        {
            try
            {
                var classes = new List<Models.EDocs.Classe>();
                var cliente = new RestClient($"{Program.UrlBaseEdocs}classificacao-documental/{patriarcaID}/planos-ativos");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var planoClasses = JsonConvert.DeserializeObject<List<DTO.PlanoClasse>>(response.Content);
                    for (int i = 0; i < planoClasses.Count; i++)
                    {
                        var clienteDois = new RestClient($"{Program.UrlBaseEdocs}classificacao-documental/{planoClasses[i].Id}/classes-ativas");
                        var requestDois = new RestRequest();
                        requestDois.Method = Method.Get;
                        requestDois.AddHeader("Content-Type", "application/json");
                        requestDois.AddHeader("Authorization", $"Bearer  {token}");
                        var responseDois = clienteDois.Execute(requestDois);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var classesPlano = JsonConvert.DeserializeObject<List<DTO.Classe>>(responseDois.Content);
                            foreach (var item in classesPlano)
                            {
                                classes.Add(new Classe(item));
                            }
                        }
                        else
                        {
                            if (response.Content.ToUpper().Contains("<HTML>"))
                            {
                                throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>. Conteúdo:<<Retornou um Html>>. Id do Plano<<{planoClasses[i].Id}>>");
                            }
                            else
                            {
                                throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>. Conteúdo:<<{response.Content}>>. Id do Plano<<{planoClasses[i].Id}>>");
                            }
                        }
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

                return classes;
            }
            catch (Exception e)
            {
                throw new Exception("E-Docs. Classe. Baixar. ", e);
            }
        }
    }
}
