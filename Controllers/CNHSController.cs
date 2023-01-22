using CIEO.DB;
using CIEO.Extension;
using CIEO.Models.AcessoCidadao;
using CIEO.Models.DTOFront.CNHS;
using CIEO.Models.EDocs;
using CIEO.Models.EDocs.Processo;
using CIEO.Servicos;
using Hyland.Unity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CIEO.Controllers
{
    [Authorize]
    public class CNHSController : Controller
    {
        private readonly ILogger<CNHSController> _logger;
        private readonly CIEOContext _dbContexto;
        private readonly IConfiguration _configuration;

        public CNHSController(ILogger<CNHSController> logger, CIEOContext contexto, IConfiguration configuration)
        {
            _logger = logger;
            _dbContexto = contexto;
            _configuration = configuration;
        }

        public IActionResult ListaEmpenhoEsperando()
        {
            ViewBag.ServidorDestino = _configuration["ServidorDestino"].AmbienteToEnum();

            var listaEmpenho = new List<Empenho>(200);
            try
            {
                OnBase.ManutencaoConexao();

                //Fila E-Docs
                var fila = OnBase.App.Workflow.Queues.Find(483);

                //Fila "Atualizar dados Empenho"
                //var fila = OnBase.App.Workflow.Queues.Find(398);

                if (fila != null)
                {
                    var qO = fila.CreateQueueQueryOptions();
                    var listaObjProc = fila.GetObjectList(qO, 10, Hyland.Unity.WorkViewObjectRetrievalOptions.None);
                    if (listaObjProc != null)
                    {
                        foreach (var item in listaObjProc)
                        {
                            if (!item.AttributeValues.Find("ProtocoloEdocs").HasValue)
                            {
                                var empenho = Models.DTOFront.CNHS.Empenho.Gerar(item);
                                listaEmpenho.Add(empenho);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CNHS.ListaEmpenhos.");
                throw new Exception("CNHS.ListaEmpenhos.", e);
            }

            /** Testes Ficticios
            listaEmpenho.Add(
                new Empenho
                {
                    AnoExercicio = 2003,
                    CodigoNotaEmpenho = "CodNotEmpe1",
                    DataSolicitacao = DateTime.Now,
                    OnBaseID = "ddddd",
                    ObjectId = 00000,
                    SaldoEmpenho = 100.50,
                    Documentos = new List<Models.DTOFront.Documento>
                    {
                        new Models.DTOFront.Documento
                        {
                           CategoriaAenxo = "Categoria Um",
                           DocumentHandle = 1111,
                           RegistroEDocs = "teste Um teste"
                        },
                        new Models.DTOFront.Documento
                        {
                            CategoriaAenxo = "Categoria Dois",
                            DocumentHandle = 22222,
                            RegistroEDocs = "teste Dois teste"
                        }
                    }
                }
                );

            listaEmpenho.Add(
                new Empenho
                {
                    AnoExercicio = 2003,
                    CodigoNotaEmpenho = "CodNotEmpe2",
                    DataSolicitacao = DateTime.Now,
                    OnBaseID = "ddddd",
                    ObjectId = 00000,
                    SaldoEmpenho = 100.50,
                    Documentos = new List<Models.DTOFront.Documento>
                    {
                        new Models.DTOFront.Documento
                        {
                           CategoriaAenxo = "Categoria quatro",
                           DocumentHandle = 1111,
                           RegistroEDocs = "teste quatro teste"
                        },
                        new Models.DTOFront.Documento
                        {
                            CategoriaAenxo = "Categoria Quatro",
                            DocumentHandle = 22222,
                            RegistroEDocs = "teste Quatro teste"
                        }
                    }
                }
                );*/

            return View(listaEmpenho);
        }

        public IActionResult Autuar(long id)
        {
            try
            {
                var userGUID = new Guid(User.FindFirst("guid").Value.ToString().Trim());
                var usuario = _dbContexto.ACUsuarios.Where(p => p.Id.Equals(userGUID))
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

                Papel papelProtocolo = null;

                if (usuario.Papeis.Count > 0)
                {
                    papelProtocolo = usuario.Papeis.Find(p => p.Nome.Trim().ToUpper().Contains("COORDENADOR") || p.Nome.Trim().ToUpper().Contains("ONBASE"));
                    if (papelProtocolo == null)
                    {
                        return NotFound();
                    }
                }

                //var classeDoc = _dbContexto.EDocsClasses.Where(p => p.Nome.Trim().ToUpper().Contains("Nota de Empenho")).FirstOrDefault();
                OnBase.ManutencaoConexao();

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

                List<Models.EDocs.Documento.Documento> listaDocRegistrados = null;
                
                long contador = docQuery.ExecuteCount();
                /**
                if (contador > 0)
                {
                    var doclistOnBase = docQuery.Execute(contador);

                    var documentosDb = Models.EDocs.Documento.Documento.EmpenhoParaEnvio(doclistOnBase, usuario.PFObjectId, null);

                    for (int i = 0; i < documentosDb.Count; i++)
                    {
                        var documentoAchado = _dbContexto.EDocsDocumentos.Where(p => p.DocumentHandle.Equals(documentosDb[i].DocumentHandle)).FirstOrDefault();
                        if (documentoAchado == null)
                        {
                            _dbContexto.EDocsDocumentos.Add(documentosDb[i]);

                            var eventoDoc = new Evento
                            {
                                DocumentoRelacionado = documentosDb[i],
                                Id = Guid.NewGuid(),
                                IdEDocs = null,
                                Situacao = EventoSituacaoEnum.AguardandoCriacao
                            };

                            _dbContexto.EDocsEventos.Add(eventoDoc);

                            _dbContexto.SaveChanges();
                        }
                        else
                        {
                            var documentoRemover = documentosDb.Find(p => p.DocumentHandle.Equals(documentoAchado.DocumentHandle));
                            documentosDb.Remove(documentoRemover);
                            documentosDb.Add(documentoAchado);
                        }
                    }

                    listaDocRegistrados = documentosDb;
                }
                */
                var empenhoObjOnBase = OnBase.App.WorkView.Applications.Find("CNHS - CNH Social").Classes.Find("Empenho").GetObjectByID(id);
                var fornecedorObjOnBase = empenhoObjOnBase.GetRelatedObject("Fornecedor");

                var local = _dbContexto.ACLocais.Where(p => p.Sigla.Trim().ToUpper().Contains("CNH SOCIAL")).FirstOrDefault();

                var processoBanco = new Models.EDocs.Processo.Processo(empenhoObjOnBase, null, local, papelProtocolo);

                foreach (var item in listaDocRegistrados)
                {
                    var atoXDoc = new AtoXDocumento();
                    atoXDoc.Id = Guid.NewGuid();
                    atoXDoc.Ato = processoBanco.Atos[0];
                    atoXDoc.Documento = item;

                    switch (item.NomeDocumento.Trim().ToUpper())
                    {
                        case "SOLICITAÇÃO DE EMPENHO":
                            atoXDoc.Posicao = 1;
                            break;
                        case "TERMO DE ADESÃO ASSINADO":
                            atoXDoc.Posicao = 2;
                            break;
                        case "CERTIDÃO NEGATIVA FEDERAL":
                            atoXDoc.Posicao = 3;
                            break;
                        case "CERTIDÃO NEGATIVA ESTADUAL":
                            atoXDoc.Posicao = 4;
                            break;
                        case "CERTIDÃO NEGATIVA MUNICIPAL":
                            atoXDoc.Posicao = 5;
                            break;
                        case "CERTIDÃO NEGATIVA FGTS":
                            atoXDoc.Posicao = 6;
                            break;
                        case "TRABALHISTA":
                            atoXDoc.Posicao = 7;
                            break;
                        case "PENDÊNCIA":
                            atoXDoc.Posicao = 8;
                            break;
                        default:
                            atoXDoc.Posicao = 9;
                            break;
                    }
                }

                var eventoAto = new Models.EDocs.Evento();
                eventoAto.Id = Guid.NewGuid();
                eventoAto.AtoRelacionado = processoBanco.Atos[0];
                eventoAto.Situacao = EventoSituacaoEnum.AguardandoCriacao;

                _dbContexto.EDocsProcessos.Add(processoBanco);

                _dbContexto.EDocsEventos.Add(eventoAto);

                _dbContexto.SaveChanges();

                //Criar tabela DocumentoXAto
                //Mapear documento X Ato 
                //Testar Gravação completa (usar ID de outro objeto para pegar os documentos)

                //Testar 

                var filaEDocs = OnBase.App.Workflow.Queues.Find(483);
                var filaArquivoEmpenho = OnBase.App.Workflow.Queues.Find(402);

                filaEDocs.TransitionObject(filaArquivoEmpenho, empenhoObjOnBase);

                var processoFront = new Models.DTOFront.CNHS.Empenho();
                return View("ProcessoRegistrado", processoFront);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CNHS.VisualisarDocumento.");
                throw new Exception("CNHS.VisualisarDocumento.", e);
            }
        }


        public IActionResult VisualisarDocumento(long id)
        {
            try
            {
                OnBase.ManutencaoConexao();

                byte[] documento = Models.EDocs.Documento.Documento.PegarPDFOnBase(id);

                return File(documento, "application/pdf");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CNHS.VisualisarDocumento.");
                throw new Exception("CNHS.VisualisarDocumento.", e);
            }
        }

        public IActionResult ResetarTeste()
        {
            try
            {
                OnBase.ManutencaoConexao();

                var ktEDocsRegistro = OnBase.App.Core.KeywordTypes.Find("EDocs Doc: Registro");
                var ktEDocsGUID = OnBase.App.Core.KeywordTypes.Find("EDocs Doc: Id");
                KeywordRecord kr = null;

                //Fila E-Docs
                var fila = OnBase.App.Workflow.Queues.Find(483);

                //Fila "Atualizar dados Empenho"
                //var fila = OnBase.App.Workflow.Queues.Find(398);

                if (fila != null)
                {
                    var qO = fila.CreateQueueQueryOptions();
                    var listaObjProc = fila.GetObjectList(qO, 2000, Hyland.Unity.WorkViewObjectRetrievalOptions.None);
                    if (listaObjProc != null)
                    {
                        foreach (var item in listaObjProc)
                        {
                            if (item.AttributeValues.Find("ProtocoloEdocs").HasValue)
                            {
                                var attMody = item.CreateAttributeValueModifier();
                                attMody.ClearAttributeValue("ProtocoloEdocs");
                                attMody.ApplyChanges();
                            }

                            var docQuery = OnBase.App.Core.CreateDocumentQuery();
                            var dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Termo de Adesão Assinado");
                            docQuery.AddDocumentType(dt);
                            dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Certidão Empenho");
                            docQuery.AddDocumentType(dt);
                            dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Solicitação Empenho");
                            docQuery.AddDocumentType(dt);
                            dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Registro de Pendências");
                            docQuery.AddDocumentType(dt);
                            docQuery.AddKeyword("ID Registro Workview", item.ID);
                            long contador = docQuery.ExecuteCount();
                            if (contador > 0)
                            {
                                var docList = docQuery.Execute(contador);
                                foreach (var documentoAnexo in docList)
                                {
                                    kr = documentoAnexo.KeywordRecords.Find(ktEDocsGUID);
                                    if (kr != null)
                                    {
                                        var docLock = documentoAnexo.LockDocument();
                                        var kwMod = documentoAnexo.CreateKeywordModifier();
                                        kwMod.RemoveKeyword(kr.Keywords.Find(ktEDocsGUID));
                                        kwMod.ApplyChanges();
                                        docLock.Release();
                                    }

                                    kr = documentoAnexo.KeywordRecords.Find(ktEDocsRegistro);
                                    if (kr != null)
                                    {
                                        var docLock = documentoAnexo.LockDocument();
                                        var kwMod = documentoAnexo.CreateKeywordModifier();
                                        kwMod.RemoveKeyword(kr.Keywords.Find(ktEDocsRegistro));
                                        kwMod.ApplyChanges();
                                        docLock.Release();
                                    }
                                }
                            }
                        }
                    }
                }

                _dbContexto.EDocsDocumentos.RemoveRange(_dbContexto.EDocsDocumentos);
                _dbContexto.EDocsEventos.RemoveRange(_dbContexto.EDocsEventos);
                _dbContexto.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CNHS.ResetarTeste.");
                throw new Exception("CNHS.ResetarTeste.", e);
            }

            return RedirectToAction("ListaEmpenhoEsperando");
        }

        /** Modelo enviar documento E-Docs
        [Authorize]
        public IActionResult EnviarDocEDocs(long id)
        {
            //var processoFront = new Models.DTOFront.Processo();
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
                
                return View("EnviarDocumento", $"Documento Enviado para o E-Docs GUID Evento:");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Ocorreu um erro ao registrar a lista de envio.");
                return View("RegistrarResultado", "Ocorreu um erro ao registrar a lista de envio.");
            }
        }
        */
    }
}