using CIEO.Models.AcessoCidadao;
using CIEO.Models.DTOFront;
using Newtonsoft.Json;
using RestSharp;

namespace CIEO.Models.EDocs.Processo
{
    /// <summary>
    /// Classe base dos Atos de um processo.
    /// </summary>
    public abstract class Ato
    {
        /// <summary>
        /// Id gerada pelo CIEO.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Processo ao qual este ato está vinculado.
        /// </summary>
        public Processo? Processo { get; set; }

        /// <summary>
        /// Data e hora que o ato foi praticado no E-Docs.
        /// </summary>
        public DateTime? DataHora { get; set; }

        /// <summary>
        /// GUID do Ato no E-Docs.
        /// </summary>
        public Guid? EDocsGUID { get; set; }

        /// <summary>
        /// Papel que praticou o ato.
        /// </summary>
        public Papel? PapelPraticante { get; set; }

        /// <summary>
        /// Situação da comunicação da integração.<br/>
        /// Visa definir se o ato já foi praticado ou não ou se ocorreu algum erro.
        /// </summary>
        public SituacaoEnum? Situacao { get; set; }

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
                var cliente = new RestClient($"{Program.UrlBaseEdocs}processos/{this.Processo.EdocsGUID}/atos/{this.EDocsGUID}");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Authorization", $"Bearer {token}");
                var resposta = cliente.Execute(request);

                if (resposta.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var atoResposta = JsonConvert.DeserializeObject<Models.EDocs.Processo.DTO.AtoResposta>(resposta.Content);

                    if (this.DataHora.Equals(atoResposta.DataHora))
                    {
                        return false;
                    }
                    else
                    {
                        this.DataHora = atoResposta.DataHora;
                        return true;
                    }
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
                throw new Exception("Documento.BaixarDados", e);
            }
        }
    }
}
