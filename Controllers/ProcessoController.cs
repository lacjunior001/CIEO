using CIEO.DB;
using CIEO.Models;
using CIEO.Models.AcessoCidadao;
using CIEO.Models.DTOFront;
using CIEO.Models.EDocs.Documento;
using CIEO.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace CIEO.Controllers
{
    public class ProcessoController : Controller
    {
        private readonly ILogger<ProcessoController> _logger;
        private readonly CIEOContext _dbContexto;

        public ProcessoController(ILogger<ProcessoController> logger, CIEOContext contexto)
        {
            _logger = logger;
            _dbContexto = contexto;
        }

        [Authorize]
        public IActionResult ListaProcFila()
        {
            var listaProcessos = new List<Processo>();

            try
            {
                OnBase.ManutencaoConexao();

                var listaEmpenhos = OnBase.App.WorkView.Applications.Find("CNHS - CNH Social").Filters.Find("Empenho");

                var fq = listaEmpenhos.CreateFilterQuery();

                var quantidade = fq.ExecuteCount();

                if (quantidade > 0)
                {
                    var objList = fq.Execute(quantidade);

                    for (int i = 0; i < objList.Count; i++)
                    {
                        var proc = new Processo();

                        proc.ObjectId = objList[i].ObjectID;

                        proc.DataSolicitacao = objList[i].GetFilterColumnValue("DataSolicitacao").HasValue ?
                            objList[i].GetFilterColumnValue("DataSolicitacao").DateValue : null;
                        proc.SaldoEmpenho = objList[i].GetFilterColumnValue("SaldoEmpenho").HasValue ?
                            (double)objList[i].GetFilterColumnValue("SaldoEmpenho").CurrencyValue : null;
                        proc.CodigoNotaEmpenho = objList[i].GetFilterColumnValue("CodigoNotaEmpenho").HasValue ?
                            objList[i].GetFilterColumnValue("CodigoNotaEmpenho").AlphanumericValue : null;

                        listaProcessos.Add(proc);
                    }
                }

                /** Busca pela fila
                var fila = OnBase.App.Workflow.Queues.Find(265);
                if (fila != null)
                {
                    var qO = fila.CreateQueueQueryOptions();
                    var listaDocs = fila.GetDocumentList(qO, 2000, Hyland.Unity.DocumentRetrievalOptions.LoadKeywords);

                    if (listaDocs != null)
                    {
                        foreach (var item in listaDocs)
                        {
                            var processo = new Processo();

                            for (int i = 0; i < item.KeywordRecords.Count; i++)
                            {
                                for (int j = 0; j < item.KeywordRecords[i].Keywords.Count; j++)
                                {
                                    switch (item.KeywordRecords[i].Keywords[j].KeywordType.Name)
                                    {
                                        case "Número Processo DetranNet":
                                            processo.NumeroDetranNet = item.KeywordRecords[i].Keywords[j].AlphaNumericValue;
                                            break;
                                        case "Ano Processo DetranNet":
                                            processo.AnoDetranNet = item.KeywordRecords[i].Keywords[j].AlphaNumericValue;
                                            break;
                                        case "Placa":
                                            processo.Placa = item.KeywordRecords[i].Keywords[j].AlphaNumericValue;
                                            break;
                                        case "Status Processo SS":
                                            processo.Situacao = item.KeywordRecords[i].Keywords[j].AlphaNumericValue;
                                            break;
                                        case "CPF Operador DetranNet":
                                            processo.CPFOperador = item.KeywordRecords[i].Keywords[j].AlphaNumericValue;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            //processo.ServicosSolicitados = item.UnityForm.AllFields.ValueFields.Find("xmlServicosSolicitados").AlphaNumericValue;
                            listaProcessos.Add(processo);
                        }
                    }
                }

                */
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ListaProcessos.");
                throw new Exception("ListaProcessos.", e);
            }

            return View(listaProcessos);
        }

        [Authorize]
        public IActionResult RegistrarEnvio(long id)
        {
            var processoFront = new Models.DTOFront.Processo();
            try
            {
                var userGUID = new Guid(User.FindFirst("guid").Value.ToString().Trim());

                var usuario = _dbContexto.ACUsuario.Where(p => p.Id.Equals(userGUID)).FirstOrDefault();

                if (usuario == null || usuario.TokenValidade.Value.CompareTo(DateTime.Now) <= 0)
                {
                    return NotFound();
                }

                OnBase.ManutencaoConexao();

                var processoOnBase = OnBase.App.WorkView.Applications.Find("CNHS - CNH Social").Classes.Find("Empenho").GetObjectByID(id);
                processoFront.ObjectId = processoOnBase.ID;

                var atributo = processoOnBase.AttributeValues.Find("DataSolicitacao");
                processoFront.DataSolicitacao = atributo.HasValue ?
                    atributo.DateValue : null;
                atributo = processoOnBase.AttributeValues.Find("SaldoEmpenho");
                processoFront.SaldoEmpenho = atributo.HasValue ?
                    (double)atributo.CurrencyValue : null;

                atributo = processoOnBase.AttributeValues.Find("CodigoNotaEmpenho");
                processoFront.CodigoNotaEmpenho = atributo.HasValue ?
                    atributo.AlphanumericValue : null;
                processoFront.Documentos = new List<Models.DTOFront.Documento>();

                var docQuery = OnBase.App.Core.CreateDocumentQuery();
                var dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Termo de Adesão Assinado");
                docQuery.AddDocumentType(dt);
                dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Certidão Empenho");
                docQuery.AddDocumentType(dt);
                dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Solicitação Empenho");
                docQuery.AddDocumentType(dt);
                dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Registro de Pendências");
                docQuery.AddDocumentType(dt);

                docQuery.AddKeyword("ID Registro Workview", id);

                long contador = docQuery.ExecuteCount();

                var listaDocumentos = new List<Models.EDocs.Documento.Documento>();
                if (contador > 0)
                {
                    var docList = docQuery.Execute(contador);
                    var ktEDocsGUID = OnBase.App.Core.KeywordTypes.Find("EDocs Doc: Id");
                    var ktCategoria = OnBase.App.Core.KeywordTypes.Find("Categoria do Anexo CNHS");
                    Hyland.Unity.KeywordRecord kr = null;
                    for (int i = 0; i < docList.Count; i++)
                    {

                        var edocsGUID = docList[i].KeywordRecords.Find(ktEDocsGUID);
                        if (edocsGUID == null)
                        {
                            var docDB = new Models.EDocs.Documento.Documento();

                            kr = docList[i].KeywordRecords.Find(ktCategoria);
                            if (kr == null)
                            {
                                docDB.NomeDocumento = "Sem Categoria";
                            }
                            else
                            {
                                docDB.NomeDocumento = kr.Keywords.Find(ktCategoria).AlphaNumericValue;
                            }

                            docDB.DocumentHandle = docList[i].ID;

                            docDB.CapturadorCPF = usuario.CPF;

                            docDB.CapturadorTipo = Models.EDocs.AgenteTipoEnum.Papel;

                            docDB.Classe = null;

                            docDB.CapturadorCredenciar = true;

                            docDB.Extensao = "pdf";
                           
                            docDB.Situacao = Models.EDocs.SituacaoEnum.AguardandoEnvioEDocs;

                            docDB.RestricaoTipo = Models.EDocs.RestricaoTipoEnum.Organizacional;

                            if ("Solicitação de Empenho".Equals(docDB.NomeDocumento))
                            {
                                docDB.AssinaturaTipo = Models.EDocs.Documento.AssinaturaTipoEnum.AutoAssinado;
                                docDB.DocumentoTipo = Models.EDocs.Documento.DocumentoTipoEnum.NatoDigital;
                                docDB.ValorLegal = ValorLegalEnum.Original;
                            }
                            else if("Termo de Adesão Assinado".Equals(docDB.NomeDocumento))
                            {
                                docDB.AssinaturaTipo = Models.EDocs.Documento.AssinaturaTipoEnum.SemAssinatura;
                                docDB.DocumentoTipo = Models.EDocs.Documento.DocumentoTipoEnum.Digitalizado;
                                docDB.ValorLegal = ValorLegalEnum.CopiaSimples;
                            }
                            else
                            {
                                docDB.AssinaturaTipo = Models.EDocs.Documento.AssinaturaTipoEnum.SemAssinatura;
                                docDB.DocumentoTipo = Models.EDocs.Documento.DocumentoTipoEnum.Digitalizado;
                                docDB.ValorLegal = ValorLegalEnum.CopiaSimples;
                            }

                            listaDocumentos.Add(docDB);
                        }
                    }

                    foreach (var item in listaDocumentos)
                    {
                        _dbContexto.Add(item);
                        _dbContexto.SaveChanges();
                    }
                }

                return View("RegistrarResultado", "Processo registrado para envio.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ocorreu um erro ao registrar a lista de envio.");
                return View("RegistrarResultado", "Ocorreu um erro ao registrar a lista de envio.");
            }
        }

        [Authorize]
        public IActionResult ListaDocAgEnvio()
        {
            var listaDocFront = new List<Models.DTOFront.Documento>();
            try
            {
                foreach (var item in _dbContexto.EDocsDocumento)
                {
                    var docfront = new Models.DTOFront.Documento();
                    docfront.DocumentHandle = item.DocumentHandle;
                    docfront.NomeArquivo = item.NomeDocumento;
                    docfront.Situacao = item.Situacao.ToString();
                    listaDocFront.Add(docfront);
                }

                return View(listaDocFront);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ocorreu um erro ao registrar a lista de envio.");
                return View("RegistrarResultado", "Ocorreu um erro ao registrar a lista de envio.");
            }
        }

        [Authorize]
        public IActionResult EnviarDocEDocs(long id)
        {
            var processoFront = new Models.DTOFront.Processo();
            try
            {
                var userGUID = new Guid(User.FindFirst("guid").Value.ToString().Trim());
                var usuario = _dbContexto.ACUsuario.Where(p => p.Id.Equals(userGUID))
                    .Include(p => p.AccessToken)
                    .Include(p => p.Papeis)
                    .FirstOrDefault();

                if (usuario == null ||
                    usuario.Papeis == null ||
                    usuario.Papeis.Count <= 0 ||
                    usuario.AccessToken == null ||
                    string.IsNullOrEmpty(usuario.AccessToken.AcessToken) ||
                    usuario.TokenValidade.Value.CompareTo(DateTime.Now) <= 0)
                {
                    return NotFound();
                }

                OnBase.ManutencaoConexao();

                var documentoEnvio = _dbContexto.EDocsDocumento.Where(p => p.DocumentHandle.Equals(id)).FirstOrDefault();

                var evento = documentoEnvio.RegistrarEDocs(usuario.AccessToken.AcessToken, usuario.Papeis[0].Id);

                return View("EnviarDocumento", $"Documento Enviado para o E-Docs GUID Evento: {evento}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ocorreu um erro ao registrar a lista de envio.");
                return View("RegistrarResultado", "Ocorreu um erro ao registrar a lista de envio.");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}