using CIEO.DB;
using CIEO.Servicos;
using Newtonsoft.Json;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace CIEO.Models.AcessoCidadao
{
    /// <summary>
    /// Encapsula tarefas executadas utilizando Token de Sistema do Acesso Cidadão.
    /// </summary>
    internal class Aplicacao
    {
        /// <summary>
        /// Validade do Token Gerado.
        /// </summary>
        private static DateTime ValideACToken { get; set; }
        private static string _acTokenSistema;

        /// <summary>
        /// Token de Sistema usado para recuperar dados no Acesso Cidadão e no E-Docs.
        /// </summary>
        internal static string AcTokenSistema
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_acTokenSistema) || ValideACToken.CompareTo(DateTime.Now) <= 0)
                {
                    CriarTokenSistema();
                }

                return _acTokenSistema;
            }
        }

        /// <summary>
        /// Conecta no Acesso Cidadão para Pegar um novo token de aplicação para o OnBase. ClientCredencials.
        /// Cuidado DETRAN e SESA possuem dados diferentes de login.
        /// </summary>
        private static void CriarTokenSistema()
        {
            try
            {
                // Detran
                //string clientID = "";
                //string clientSecret = "";

                // SESA
                string clientID = "";
                string clientSecret = "";


                string autoriValor = $"{clientID}:{clientSecret}";
                autoriValor = Convert.ToBase64String(Encoding.UTF8.GetBytes(autoriValor));

                var cliente = new RestClient("");
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Authorization", $"Basic {autoriValor}");
                request.AddParameter("grant_type", "client_credentials");

                //Detran:
                //request.AddParameter("scope", "");

                //SESA:
                request.AddParameter("scope", "");

                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
                    _acTokenSistema = json["access_token"].Trim();

                    var jwt = new JwtSecurityTokenHandler();
                    if (jwt.CanReadToken(_acTokenSistema))
                    {
                        var valores = jwt.ReadJwtToken(_acTokenSistema);
                        ValideACToken = valores.ValidTo.ToLocalTime();
                    }
                    else
                    {
                        throw new Exception("O Token de Aplicação recuperado está ilegível");
                    }
                }
                else
                {
                    if (response.Content.ToUpper().Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do Acesso Cidadão. Código:<<{response.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do Acesso Cidadão. Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Acesso Cidadão. Aplicação. Criar Token.", e);
            }
        }
    }
}
