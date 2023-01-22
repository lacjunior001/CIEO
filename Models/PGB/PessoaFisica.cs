using CIEO.Models.AcessoCidadao;
using CIEO.Models.EDocs.Processo;
using CIEO.Servicos;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace CIEO.Models.PGB
{
    public class PessoaFisica
    {
        /// <summary>
        /// Object ID de Pessoa.
        /// Evite utilizar como indexador dê preferencia ao CPF.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Nome.
        /// </summary>
        public string? Nome { get; set; }

        /// <summary>
        /// CPF da pessoa. Sem qualquer carácter especial.
        /// </summary>
        public string? CPF { get; set; }

        /// <summary>
        /// E-Mail que deve ter como fonte o Acesso Cidadão.
        /// </summary>
        public long? EMailPrincipalId { get; set; }

        /// <summary>
        /// Lista de Contatos de uma Pessoa.
        /// </summary>
        public List<Contato>? Contatos { get; set; }

        /// <summary>
        /// Baixa o E-Mail cadastrado pela pessoa no Acesso Cidadão.
        /// </summary>
        /// <param name="token">Cidadão ou Sistema</param>
        /// <param name="cidadaoId"></param>
        /// <returns></returns>
        internal static string? BaixarEmail(string token, Guid cidadaoId)
        {
            try
            {
                var cliente = new RestClient($"https://sistemas.es.gov.br/prodest/acessocidadao.webapi/api/cidadao/{cidadaoId}/email");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer  {token}");
                var response = cliente.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                    var emailAtt = jsonObj.GetValue("email");
                    if (emailAtt == null)
                    {
                        return null;
                    }
                    else
                    {
                        return emailAtt.ToString();
                    }
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
                throw new Exception($"PG. PessoaFisica. BaixarEmail. {cidadaoId}. ", e);
            }
        }

        /// <summary>
        /// Atualiza as informações desta pessoa no OnBase
        /// </summary>
        /// <param name="wvApp"></param>
        internal void Atualizar(Hyland.Unity.WorkView.Application wvApp)
        {
            var objPessoa = wvApp.Classes.Find("PGBPessoaFisica").GetObjectByID(this.Id.Value);

            var attMod = objPessoa.CreateAttributeValueModifier();
            bool aplicar = false;

            if (!string.IsNullOrWhiteSpace(this.Nome))
            {
                attMod.SetAttributeValue("Nome", this.Nome);
                aplicar = true;
            }

            if (this.EMailPrincipalId != null && this.EMailPrincipalId > 0)
            {
                attMod.SetAttributeValue("EmailPrincipal", this.EMailPrincipalId.Value);
                aplicar = true;
            }

            if (aplicar)
            {
                attMod.ApplyChanges();
            }

            this.Id = objPessoa.ID;
            if (this.Contatos != null && this.Contatos.Count > 0)
            {
                foreach (var contato in this.Contatos)
                {
                    contato.PessoaFisica = this;
                    contato.Tratar(wvApp);
                }
            }
        }

        /// <summary>
        /// Cria uma nova PGBPessoaFisica no OnBase com todos contatos cadastrados na classe instanciada.
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal void Criar(Hyland.Unity.WorkView.Application wvApp)
        {
            try
            {
                var objPessoa = wvApp.Classes.Find("PGBPessoaFisica").CreateObject();
                var attMod = objPessoa.CreateAttributeValueModifier();
                attMod.SetAttributeValue("Nome", this.Nome);
                attMod.SetAttributeValue("CPF", this.CPF);
                attMod.ApplyChanges();
                this.Id = objPessoa.ID;
                if (this.Contatos != null && this.Contatos.Count > 0)
                {
                    foreach (var contato in this.Contatos)
                    {
                        contato.PessoaFisica = this;
                        contato.Tratar(wvApp);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Pessoa Física. Criar.", e);
            }
        }

        /// <summary>
        /// Padroniza esta instancia para que o E-Mail principal seja o do AC.<br/>
        /// Esta instancia deve refletir o que existe no OnBase no momento da execução.<br/>
        /// A instancia de pessoa deve estar carregada no contato.
        /// </summary>
        /// <param name="emailAC">E-Mail da Pessoa registrado no Acesso Cidadão.</param>
        /// <param name="wvApp">app do wv que onde o "usuário" tem permissão de edição.</param>
        internal void PadraoAC(string emailAC, Hyland.Unity.WorkView.Application wvApp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailAC))
                {
                    throw new Exception("E-Mail recebido está nulo.");
                }

                if (this.Contatos == null)
                {
                    this.Contatos = new List<Contato>(1);
                }

                if (this.Contatos.Count == 0 ||
                    !this.Contatos.Exists(p =>
                            p.Valor.Contains(emailAC, StringComparison.OrdinalIgnoreCase)))
                {
                    //Caso o E-Mail não exista na lista de contatos da pessoa.
                    var novoContato = new Contato()
                    {
                        PessoaFisica = this,
                        TipoContato = new TipoContato { Valor = "E-Mail" },
                        Ativo = true,
                        OrigemInformacao = new OrigemInformacao { Valor = "Acesso Cidadão" },
                        Valor = emailAC
                    };
                    novoContato.Tratar(wvApp);
                    this.EMailPrincipalId = novoContato.Id;
                    this.Atualizar(wvApp);
                }
                else
                {
                    //Caso o E-Mail exista na lista de contatos da pessoa.
                    var contatoEncontrado = this.Contatos.Find(p =>
                            p.Valor.Contains(emailAC, StringComparison.OrdinalIgnoreCase));

                    contatoEncontrado.PessoaFisica = this;
                    bool atualizarContatoEncontrado = false;

                    if (!"E-Mail".Equals(contatoEncontrado.TipoContato.Valor))
                    {
                        contatoEncontrado.TipoContato = new TipoContato { Valor = "E-Mail" };
                        atualizarContatoEncontrado = true;
                    }

                    if (!"Acesso Cidadão".Equals(contatoEncontrado.OrigemInformacao.Valor))
                    {
                        contatoEncontrado.OrigemInformacao = new OrigemInformacao { Valor = "Acesso Cidadão" };
                        atualizarContatoEncontrado = true;
                    }

                    if (contatoEncontrado.Ativo == null || contatoEncontrado.Ativo == false)
                    {
                        contatoEncontrado.Ativo = true;
                        atualizarContatoEncontrado = true;
                    }

                    if (!emailAC.Equals(contatoEncontrado.Valor))
                    {
                        contatoEncontrado.Valor = emailAC;
                        atualizarContatoEncontrado = true;
                    }

                    if (contatoEncontrado.Id == null || contatoEncontrado.Id <= 0)
                    {
                        atualizarContatoEncontrado = true;
                    }

                    if (atualizarContatoEncontrado)
                    {
                        contatoEncontrado.Tratar(wvApp);
                    }

                    //esta remoção é para acertar a lista de contatos da pessoa caso necessário.
                    this.Contatos.Remove(contatoEncontrado);

                    var listaContatosACAntigos = this.Contatos.FindAll(p =>
                                    p.TipoContato.Valor.Contains("E-Mail", StringComparison.OrdinalIgnoreCase) &&
                                    p.OrigemInformacao.Valor.Contains("Acesso Cidadão", StringComparison.OrdinalIgnoreCase)
                                    );

                    if (listaContatosACAntigos != null && listaContatosACAntigos.Count > 0)
                    {
                        foreach (var contatoAntigo in listaContatosACAntigos)
                        {
                            contatoAntigo.PessoaFisica = this;
                            contatoAntigo.OrigemInformacao = new OrigemInformacao { Valor = "AC (Substituído)" };
                            contatoAntigo.Tratar(wvApp);
                        }
                    }

                    this.Contatos.Add(contatoEncontrado);

                    if (this.EMailPrincipalId == null || this.EMailPrincipalId != contatoEncontrado.Id)
                    {
                        this.EMailPrincipalId = contatoEncontrado.Id;
                        this.Atualizar(wvApp);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Pessoa Física. Padrão Acesso Cidadão.{this.CPF}. ", e);
            }
        }

        //public Endereco? Domicilio { get; set; }
        //public List<Endereco>? Enderecos { get; set; }
    }
}
