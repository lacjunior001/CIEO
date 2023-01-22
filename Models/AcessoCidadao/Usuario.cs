using CIEO.DB;
using CIEO.Models.PGB;
using CIEO.Servicos;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace CIEO.Models.AcessoCidadao
{
    /// <summary>
    /// Informações do usuário que está logado.
    /// </summary>
    public class Usuario
    {
        public long? Id { get; set; }
        public Guid? AcessoCidadaoId { get; set; }
        public long? PFObjectId { get; set; }
        public Token? AccessToken { get; set; }
        public DateTime? TokenGerado { get; set; }
        public DateTime? TokenValidade { get; set; }
        public bool? ContaOnBase { get; set; }
        public List<Papel>? Papeis { get; set; }
        public List<UsuOBXUsuAC>? UsuariosOB { get; set; }

        /// <summary>
        /// Utilizando o CPF de uma PF retorna o Sub que identifica a pessoa no AC.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cpf"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static Guid? BaixarSub(string token, string cpf)
        {
            try
            {
                var cliente = new RestClient($"https://sistemas.es.gov.br/prodest/acessocidadao.webapi/api/cidadao/{cpf}/pesquisaSub");
                var request = new RestRequest();
                request.Method = Method.Put;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                    return Guid.Parse(jsonObj.GetValue("sub").ToString());
                }
                else
                {
                    if (response.Content.Contains("<html>"))
                    {
                        throw new Exception($"Código:<<{response.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Código:<<{response.StatusCode}>>.Conteúdo:<<{response.Content}>>");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Usuario.BaixarSub.{cpf}", e);
            }
        }

        internal bool ExecutarAtualizacaoLogin(string emailAtualAC, string cpf, string apelido)
        {
            try
            {
                using (var contexto = new CIEOContext())
                {
                    var usuarioDB = contexto.ACUsuarios
                                        .Where(p => p.AcessoCidadaoId.Equals(this.AcessoCidadaoId))
                                        .FirstOrDefault();
                    bool salvar = false;

                    if (usuarioDB == null)
                    {
                        //Se usuário não existe no Banco ele deverá ser criado.
                        var papeisDTO = Models.AcessoCidadao.DTO.Papel.Baixar(this.AccessToken.AcessToken, this.AcessoCidadaoId.Value);

                        if (papeisDTO != null && papeisDTO.Count > 0)
                        {
                            this.Papeis = new List<Papel>(papeisDTO.Count);

                            foreach (var papelDTO in papeisDTO)
                            {
                                this.Papeis.Add(
                                    new Papel(papelDTO)
                                    );
                            }
                        }

                        if (this.Papeis != null && this.Papeis.Count > 0)
                        {
                            foreach (var item in this.Papeis)
                            {
                                //No banco só existem os locais cadastrados do Detran
                                //Caso o papel seja de lotação em outro local que não seja no detran
                                //a informação de lotação será ignorada por hora.
                                var lotacao = contexto.ACLocais.Where(p => p.AcessoCidadaoId == item.Lotacao.AcessoCidadaoId).FirstOrDefault();
                                if (lotacao == null)
                                {
                                    item.Lotacao = null;
                                }
                                else
                                {
                                    item.Lotacao = lotacao;
                                }
                            }
                        }

                        //Todo usuário está ligado a uma pessoa Física.
                        //Buscamos se a pessoa física existe ou não no OnBase.
                        var pessoaOnBase = contexto.PGBPessoasFisicas
                                        .AsNoTracking()
                                        .Where(p => p.CPF.Equals(cpf))
                                        .FirstOrDefault();

                        OnBase.ManutencaoConexao();
                        var wvApp = OnBase.App.WorkView.Applications.Find("PGB - Parametrização Global (Novo)");
                        if (pessoaOnBase == null)
                        {
                            pessoaOnBase = new PessoaFisica
                            {
                                Nome = apelido,
                                CPF = cpf
                            };

                            //Se a pessoa física não existe.
                            pessoaOnBase.Criar(wvApp);
                            pessoaOnBase.PadraoAC(emailAtualAC, wvApp);
                        }
                        else
                        {
                            //Se a pessoa física existe.
                            pessoaOnBase.Contatos = contexto.PGBContatos.Where(p =>
                                    p.PessoaFisica == pessoaOnBase)
                                    .AsNoTracking()
                                    .Include(p => p.OrigemInformacao)
                                    .AsNoTracking()
                                    .Include(p => p.TipoContato)
                                    .AsNoTracking()
                                    .Include(p => p.PessoaFisica)
                                    .AsNoTracking()
                                    .ToList();

                            pessoaOnBase.PadraoAC(emailAtualAC, wvApp);
                        }

                        this.PFObjectId = pessoaOnBase.Id;

                        contexto.ACUsuarios.Add(this);

                        salvar = true;
                    }
                    else
                    {
                        var travaHora = usuarioDB.TokenGerado.Value.AddMinutes(20);
                        if (travaHora.CompareTo(DateTime.Now) < 0)
                        {
                            //só será executada qualquer atualização no banco após 20 minutos.
                            contexto.Entry(usuarioDB)
                                .Reference(p => p.AccessToken)
                                .Load();

                            usuarioDB.TokenGerado = this.TokenGerado;
                            usuarioDB.TokenValidade = this.TokenValidade;

                            if (usuarioDB.AccessToken == null)
                            {
                                usuarioDB.AccessToken = this.AccessToken;
                            }
                            else
                            {
                                usuarioDB.AccessToken.AcessToken = this.AccessToken.AcessToken;
                            }

                            var pessoaOnBase = contexto.PGBPessoasFisicas
                                        .Where(p => p.CPF.Equals(cpf))
                                        .AsNoTracking()
                                        .FirstOrDefault();

                            if (pessoaOnBase == null)
                            {
                                OnBase.ManutencaoConexao();
                                var wvApp = OnBase.App.WorkView.Applications.Find("PGB - Parametrização Global (Novo)");

                                pessoaOnBase = new PessoaFisica
                                {
                                    Nome = apelido,
                                    CPF = cpf
                                };

                                pessoaOnBase.Criar(wvApp);
                            }

                            pessoaOnBase.Contatos = contexto.PGBContatos.Where(p =>
                                    p.PessoaFisica == pessoaOnBase)
                                    .AsNoTracking()
                                    .Include(p => p.OrigemInformacao)
                                    .AsNoTracking()
                                    .Include(p => p.TipoContato)
                                    .AsNoTracking()
                                    .Include(p => p.PessoaFisica)
                                    .AsNoTracking()
                                    .ToList();

                            var emailAchado = pessoaOnBase.Contatos.Find(p => p.Valor.Contains(emailAtualAC, StringComparison.OrdinalIgnoreCase));

                            if (emailAchado == null ||
                                pessoaOnBase.EMailPrincipalId == null ||
                                !pessoaOnBase.EMailPrincipalId.Equals(emailAchado.Id))
                            {
                                OnBase.ManutencaoConexao();
                                var wvApp = OnBase.App.WorkView.Applications.Find("PGB - Parametrização Global (Novo)");
                                pessoaOnBase.PadraoAC(emailAtualAC, wvApp);
                            }

                            salvar = true;
                        }
                    }

                    if (salvar)
                    {
                        contexto.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Usuário. Executar Atualização de Login. ", e);
            }
        }
    }
}
