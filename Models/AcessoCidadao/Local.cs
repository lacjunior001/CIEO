using CIEO.Models.AcessoCidadao.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace CIEO.Models.AcessoCidadao
{
    /// <summary>
    /// Informações e caracteristicas de um "local" da administração publica.
    /// Informação coletada do E-Docs/Acesso Cidadão.
    /// </summary>
    public class Local
    {
        public long? Id { get; set; }
        public Guid? AcessoCidadaoId { get; set; }
        public Local? LocalPai { get; set; }

        public List<Local>? LocaisFilho { get; set; }

        /// <summary>
        /// False = Quando um determinado local deixa de ser informado nas lista de consulta.
        /// </summary>
        public bool? Ativo { get; set; }
        public string? Nome { get; set; }
        public string? Sigla { get; set; }
        public string? Tipo { get; set; }

        public Local()
        {
        }

        /// <summary>
        /// Utilizado apenas para quando acabou de baixar do E-Docs as Informações.
        /// </summary>
        /// <param name="dtoLocal"></param>
        public Local(DTO.LocalBase dtoLocal)
        {

            this.AcessoCidadaoId = dtoLocal.Id;
            this.Ativo = true;
            this.Nome = dtoLocal.Nome;
            this.Sigla = dtoLocal.Sigla;
            this.Tipo = dtoLocal.Tipo;

            switch (dtoLocal)
            {
                case DTO.Patriarca:
                    this.LocalPai = null;
                    this.Tipo = "Patriarca";
                    break;
                case DTO.Organizacao:
                    var org = (DTO.Organizacao)dtoLocal;
                    this.LocalPai = new Local(org.Patriarca.Id.Value);
                    break;
                case DTO.Setor:
                    var set = (DTO.Setor)dtoLocal;
                    LocalPai = new Local(set.Organizacao.Id.Value);
                    break;
                case DTO.Grupo:
                    var grup = (DTO.Grupo)dtoLocal;
                    LocalPai = new Local(grup.Localizacao.Id.Value);
                    break;
            }
        }

        public Local(Guid? acessoCidadaoId)
        {
            this.AcessoCidadaoId = acessoCidadaoId;
        }

        /// <summary>
        /// Baixa todos os Locais(Agentes) disponíveis para download no E-Docs.
        /// </summary>
        /// <param name="token">Cidadão ou Sistema</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static List<Local> Baixar(string token)
        {
            try
            {
                var listaPatriarcas = BaixarPatriarcas(token);

                var govES = listaPatriarcas.Find(p => "GOVES".Equals(p.Sigla));

                var listaOrganizacoes = BaixarOrganizacoes(token, govES.AcessoCidadaoId.Value);

                var detranOrganizao = listaOrganizacoes.Find(p => p.Sigla.Contains("DETRAN"));

                var listaSetores = BaixarSetores(token, detranOrganizao.AcessoCidadaoId.Value);
                var listaGrupos = BaixarGrupos(token, detranOrganizao.AcessoCidadaoId.Value);
                var listaComicoes = BaixarComissoes(token, detranOrganizao.AcessoCidadaoId.Value);

                for (int i = 0; i < listaOrganizacoes.Count; i++)
                {
                    if (!listaPatriarcas.Exists(p => p.AcessoCidadaoId.Equals(listaOrganizacoes[i].AcessoCidadaoId)))
                    {
                        listaPatriarcas.Add(listaOrganizacoes[i]);
                    }
                }

                for (int i = 0; i < listaSetores.Count; i++)
                {
                    if (!listaPatriarcas.Exists(p => p.AcessoCidadaoId.Equals(listaSetores[i].AcessoCidadaoId)))
                    {
                        listaPatriarcas.Add(listaSetores[i]);
                    }
                }

                for (int i = 0; i < listaGrupos.Count; i++)
                {
                    if (!listaPatriarcas.Exists(p => p.AcessoCidadaoId.Equals(listaGrupos[i].AcessoCidadaoId)))
                    {
                        listaPatriarcas.Add(listaGrupos[i]);
                    }
                }

                for (int i = 0; i < listaComicoes.Count; i++)
                {
                    if (!listaPatriarcas.Exists(p => p.AcessoCidadaoId.Equals(listaComicoes[i].AcessoCidadaoId)))
                    {
                        listaPatriarcas.Add(listaComicoes[i]);
                    }
                }

                return listaPatriarcas;
            }
            catch (Exception e)
            {
                throw new Exception("Acesso Cidadão. Local. Baixar. ", e);
            }
        }

        private static List<Local> BaixarPatriarcas(string token)
        {
            try
            {
                var locais = new List<Local>();
                var cliente = new RestClient($"{Program.UrlBaseEdocs}agente/patriarcas");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var patriarcas = JsonConvert.DeserializeObject<List<Patriarca>>(response.Content);

                    for (int i = 0; i < patriarcas.Count; i++)
                    {
                        locais.Add(new Local(patriarcas[i]));
                    }
                }
                else
                {
                    if (response.Content.ToUpper().Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                    }
                }
                return locais;
            }
            catch (Exception e)
            {
                throw new Exception("Patriarcas. ", e);
            }
        }

        private static List<Local> BaixarOrganizacoes(string token, Guid? patriarcaId)
        {
            try
            {
                if (patriarcaId == null)
                {
                    throw new Exception("A Id do Governo do estado não foi recebida.");
                }

                var locais = new List<Local>();
                var cliente = new RestClient($"{Program.UrlBaseEdocs}agente/{patriarcaId}/organizacoes");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var organizacoes = JsonConvert.DeserializeObject<List<Organizacao>>(response.Content);

                    for (int i = 0; i < organizacoes.Count; i++)
                    {
                        locais.Add(new Local(organizacoes[i]));
                    }
                }
                else
                {
                    if (response.Content.ToUpper().Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                    }
                }

                return locais;
            }
            catch (Exception e)
            {
                throw new Exception("Organizações. ", e);
            }
        }

        private static List<Local> BaixarSetores(string token, Guid? organizacaoId)
        {
            try
            {
                if (organizacaoId == null)
                {
                    throw new Exception("A Id do Governo do estado não foi recebida.");
                }

                var locais = new List<Local>();
                var cliente = new RestClient($"{Program.UrlBaseEdocs}agente/{organizacaoId}/setores");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var setores = JsonConvert.DeserializeObject<List<Setor>>(response.Content);

                    for (int i = 0; i < setores.Count; i++)
                    {
                        locais.Add(new Local(setores[i]));
                    }
                }
                else
                {
                    if (response.Content.ToUpper().Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                    }
                }
                return locais;
            }
            catch (Exception e)
            {
                throw new Exception("Setores. ", e);
            }
        }

        private static List<Local> BaixarGrupos(string token, Guid? organizacaoId)
        {
            try
            {
                if (organizacaoId == null)
                {
                    throw new Exception("A Id do Governo do estado não foi recebida.");
                }

                var locais = new List<Local>();
                var cliente = new RestClient($"{Program.UrlBaseEdocs}agente/{organizacaoId}/grupos-trabalho");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var grupos = JsonConvert.DeserializeObject<List<Grupo>>(response.Content);

                    for (int i = 0; i < grupos.Count; i++)
                    {
                        locais.Add(new Local(grupos[i]));
                    }
                }
                else
                {
                    if (response.Content.ToUpper().Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                    }
                }
                return locais;
            }
            catch (Exception e)
            {
                throw new Exception("Grupo. ", e);
            }
        }

        private static List<Local> BaixarComissoes(string token, Guid? organizacaoId)
        {
            try
            {
                if (organizacaoId == null)
                {
                    throw new Exception("A Id do Governo do estado não foi recebida.");
                }

                var locais = new List<Local>();
                var cliente = new RestClient($"{Program.UrlBaseEdocs}agente/{organizacaoId}/comissoes");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var comicoes = JsonConvert.DeserializeObject<List<Grupo>>(response.Content);

                    for (int i = 0; i < comicoes.Count; i++)
                    {
                        locais.Add(new Local(comicoes[i]));
                    }
                }
                else
                {
                    if (response.Content.ToUpper().Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                    }
                }
                return locais;
            }
            catch (Exception e)
            {
                throw new Exception("Comissões. ", e);
            }
        }
    }
}
