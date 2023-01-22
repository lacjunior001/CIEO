using CIEO.Models;
using CIEO.Servicos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CIEO.DB;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CIEO.Models.DTOFront.Manager;
using CIEO.Models.PGB;
using CIEO.Models.AcessoCidadao;
using Hyland.Unity.WorkView;
using CIEO.Models.DTOFront;
using CIEO.Models.AcessoCidadao.DTO;
using CIEO.Models.EDocs;
using System.Linq;
using CIEO.Models.EDocs.Documento;
using CIEO.Extension;
using CIEO.Models.EDocs.DTO;

namespace CIEO.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;
        private readonly CIEOContext _dbContext;

        public ManagerController(ILogger<ManagerController> logger, CIEOContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                Guid? guid = ValidarGUID(User.FindFirst("guid"));
                if (guid == null)
                {
                    return RedirectToAction("ErroResultado", "Cookie Inválido.");
                }

                if (!IsManager(guid.Value))
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }

                if (User.Identity.IsAuthenticated)
                {
                    var apelido = User.FindFirst("apelido");
                    if (apelido != null)
                    {
                        ViewBag.Apelido = apelido.Value.ToString();
                    }

                    return View();
                }
                else
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Manager.Index.");
                var msg = Uteis.MontarMsgErro("Manager.Index.", e);
                return RedirectToAction("ErroResultado", msg);
            }
        }

        public async Task<IActionResult> ListaContRelacionadas()
        {
            var listaUsuOB = new List<UsuOBXUsuAC>();

            try
            {
                Guid? guid = ValidarGUID(User.FindFirst("guid"));
                if (guid == null)
                {
                    return RedirectToAction("ErroResultado", "Cookie Inválido.");
                }

                if (!IsManager(guid.Value))
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }

                if (User.Identity.IsAuthenticated)
                {
                    listaUsuOB = _dbContext
                        .ACUsuOBxUsuAC.Where(p =>
                            p.UsuarioAC.AcessoCidadaoId.Value.Equals(guid))
                        .Include(p => p.UsuarioAC)
                        .Include(p => p.UsuarioOB)
                        .ToList();

                    return View(listaUsuOB);
                }
                else
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Manager.ListaContRelacionadas.");
                var msg = Uteis.MontarMsgErro("Manager.ListaContRelacionadas.", e);
                return RedirectToAction("ErroResultado", msg);
            }
        }

        public async Task<IActionResult> ListaDocumentosConfigurados()
        {
            var docsConfig = new List<Models.DTOFront.Manager.DocumentoConfigLista>();

            try
            {
                Guid? guid = ValidarGUID(User.FindFirst("guid"));
                if (guid == null)
                {
                    return RedirectToAction("ErroResultado", "Cookie Inválido.");
                }

                if (!IsManager(guid.Value))
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }

                if (User.Identity.IsAuthenticated)
                {
                    var docsConfigDB = _dbContext.EDocsDocConfigPad.ToList();

                    foreach (var docConfigDB in docsConfigDB)
                    {
                        var docConfig = new DocumentoConfigLista
                        {
                            Id = docConfigDB.Id.Value,
                            TipoDocumental = docConfigDB.TipoDocumental,
                            CategoriaDoAnexo = docConfigDB.CategoriaAnexo,
                            Ativo = docConfigDB.Ativo
                        };

                        docsConfig.Add(docConfig);
                    }

                    return View(docsConfig);
                }
                else
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Manager.ListaDocumentosConfigurados.");
                var msg = Uteis.MontarMsgErro("Manager.ListaDocumentosConfigurados.", e);
                return RedirectToAction("ErroResultado", msg);
            }
        }

        public async Task<IActionResult> RelacionarContasForm()
        {
            try
            {
                Guid? guid = ValidarGUID(User.FindFirst("guid"));
                if (guid == null)
                {
                    return RedirectToAction("ErroResultado", "Cookie Inválido.");
                }

                if (!IsManager(guid.Value))
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }

                if (User.Identity.IsAuthenticated)
                {
                    return View("RelacionarContas");
                }
                else
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Manager.RelacionarContasForm.");
                var msg = Uteis.MontarMsgErro("Manager.RelacionarContasForm.", e);
                return RedirectToAction("ErroResultado", msg);
            }
        }

        public async Task<IActionResult> DocConfigPadForm()
        {
            try
            {
                Guid? guid = ValidarGUID(User.FindFirst("guid"));
                if (guid == null)
                {
                    return RedirectToAction("ErroResultado", "Cookie Inválido.");
                }

                if (!IsManager(guid.Value))
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }

                if (User.Identity.IsAuthenticated)
                {
                    var configDoc = new Models.DTOFront.Manager.DocConficPad();

                    configDoc.TiposDocumentais = new List<string>(8000);
                    OnBase.ManutencaoConexao();
                    foreach (var item in OnBase.App.Core.DocumentTypes)
                    {
                        configDoc.TiposDocumentais.Add(item.Name);
                    }

                    var selectItemMain = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = "Escolha uma opção",
                        Value = "0",
                        Disabled = false,
                        Selected = true
                    };

                    configDoc.AssinaturaTipo = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> { selectItemMain };

                    var tiposAssinatura = _dbContext.EDocsDocTipAss.ToList();
                    for (int i = 1; i < tiposAssinatura.Count; i++)
                    {
                        var selectItem = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Text = tiposAssinatura[i].Nome,
                            Value = tiposAssinatura[i].Id.ToString(),
                            Disabled = false,
                            Selected = false
                        };
                        configDoc.AssinaturaTipo.Add(selectItem);
                    }

                    configDoc.DocumentoTipo = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> { selectItemMain };
                    var tiposDocumento = _dbContext.EDocsDocTipDoc.ToList();
                    for (int i = 1; i < tiposDocumento.Count; i++)
                    {
                        var selectItem = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Text = tiposDocumento[i].Nome,
                            Value = tiposDocumento[i].Id.ToString(),
                            Disabled = false,
                            Selected = false
                        };
                        configDoc.DocumentoTipo.Add(selectItem);
                    }

                    configDoc.RestricaoTipo = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> { selectItemMain };
                    var tiposRestricao = _dbContext.EDocsTipoRestricao.ToList();
                    for (int i = 1; i < tiposRestricao.Count; i++)
                    {
                        var selectItem = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Text = tiposRestricao[i].Nome,
                            Value = tiposRestricao[i].Id.ToString(),
                            Disabled = false,
                            Selected = false
                        };

                        configDoc.RestricaoTipo.Add(selectItem);
                    }

                    configDoc.ValorLegal = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> { selectItemMain };
                    var valoreLegais = _dbContext.EDocsDocValLegal.ToList();
                    for (int i = 1; i < tiposRestricao.Count; i++)
                    {
                        var selectItem = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                        {
                            Text = valoreLegais[i].Nome,
                            Value = valoreLegais[i].Id.ToString(),
                            Disabled = false,
                            Selected = false
                        };

                        configDoc.ValorLegal.Add(selectItem);
                    }

                    configDoc.Classes = _dbContext.EDocsClasses.ToList();

                    return View("DocConfigPad", configDoc);
                }
                else
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Manager.DocConfigPadForm.");
                var msg = Uteis.MontarMsgErro("Manager.DocConfigPadForm.", e);
                return RedirectToAction("ErroResultado", msg);
            }
        }

        public async Task<IActionResult> CadastrarAssinanteForm(long idDocConfigPad)
        {
            try
            {
                Guid? guid = ValidarGUID(User.FindFirst("guid"));
                if (guid == null)
                {
                    return RedirectToAction("ErroResultado", "Cookie Inválido.");
                }

                if (!IsManager(guid.Value))
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }

                if (User.Identity.IsAuthenticated)
                {
                    if (idDocConfigPad < 1)
                    {
                        return RedirectToAction("ErroResultado", $"Id ({idDocConfigPad}) do documento é Inválida");
                    }

                    var docConfigPad = _dbContext.EDocsDocConfigPad.Where(p => p.Id.Equals(idDocConfigPad))
                        .FirstOrDefault();

                    if (docConfigPad == null)
                    {
                        return RedirectToAction("ErroResultado", $"Id ({idDocConfigPad}) do documento é Inválida");
                    }

                    var assinante = new Models.DTOFront.Manager.Assinante();
                    assinante.DocConfigPadId = idDocConfigPad;

                    return View("Assinante", assinante);
                }
                else
                {
                    return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Manager.DocConfigPadForm.");
                var msg = Uteis.MontarMsgErro("Manager.DocConfigPadForm.", e);
                return RedirectToAction("ErroResultado", msg);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RelacionarContas(RelacionarContas relacionarContas)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid? guid = ValidarGUID(User.FindFirst("guid"));
                    if (guid == null)
                    {
                        return RedirectToAction("ErroResultado", "Cookie Inválido.");
                    }

                    if (!IsManager(guid.Value))
                    {
                        return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                    }

                    if (User.Identity.IsAuthenticated)
                    {
                        if (!Uteis.CPFValido(relacionarContas.CpfAcessoCidadao))
                        {
                            throw new Exception("CPF informado não é válido.");
                        }

                        var pessoaFisica = _dbContext.PGBPessoasFisicas
                                .Where(p => p.CPF.Trim().Equals(relacionarContas.CpfAcessoCidadao))
                                .FirstOrDefault();

                        Usuario? usuarioAC = null;

                        if (pessoaFisica == null)
                        {
                            throw new Exception("É necessário que esta pessoa faça login no acesso cidadão antes de vincular contas. (PF Não Cadastrada)");
                        }
                        else
                        {
                            usuarioAC = _dbContext.ACUsuarios
                                .Where(p => p.PFObjectId.Equals(pessoaFisica.Id))
                                .FirstOrDefault();

                            if (usuarioAC == null)
                            {
                                throw new Exception("É necessário que esta pessoa faça login no acesso cidadão antes de vincular contas. (UsuarioAC Não Cadastrado)");
                            }
                        }

                        var usuarioOB = _dbContext.PGBUsuariosOB
                                .Where(p => p.UserName.Trim().ToUpper().Equals(relacionarContas.UserNameOnBase.Trim().ToUpper()))
                                .FirstOrDefault();

                        if (usuarioOB == null)
                        {
                            throw new Exception("Usuário não cadastrado no OnBase.");
                        }

                        var relacionamento = _dbContext.ACUsuOBxUsuAC
                                .Where(p => p.UsuarioAC.Equals(usuarioAC) &&
                                            p.UsuarioOB.Equals(usuarioOB))
                                .Include(p => p.UsuarioAC)
                                .Include(p => p.UsuarioOB)
                                .FirstOrDefault();

                        if (relacionamento == null)
                        {
                            relacionamento = new UsuOBXUsuAC
                            {
                                UsuarioAC = usuarioAC,
                                UsuarioOB = usuarioOB
                            };

                            _dbContext.ACUsuOBxUsuAC.Add(relacionamento);
                            _dbContext.SaveChanges();

                            return View("RelaCadastrado", relacionarContas);
                        }
                        else
                        {
                            throw new Exception("Relacionamento já existe.");
                        }
                    }
                    else
                    {
                        return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Manager.RelacionarContas.");
                    var msg = Uteis.MontarMsgErro("Manager.RelacionarContas.", e);
                    return RedirectToAction("ErroResultado", msg);
                }
            }
            else
            {
                return View(relacionarContas);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurarDocumento(CIEO.Models.DTOFront.Manager.DocConficPad docConfigPadForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid? guid = ValidarGUID(User.FindFirst("guid"));
                    if (guid == null)
                    {
                        return RedirectToAction("ErroResultado", "Cookie Inválido.");
                    }

                    if (!IsManager(guid.Value))
                    {
                        return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                    }

                    if (User.Identity.IsAuthenticated)
                    {
                        OnBase.ManutencaoConexao();
                        var tipoDocumental = OnBase.App.Core.DocumentTypes.Find(docConfigPadForm.TipoDocumental);
                        if (tipoDocumental == null)
                        {
                            throw new Exception("Tipo Documental não encontrado.");
                        }

                        var docConfigPadDB = _dbContext.EDocsDocConfigPad
                            .Where(p =>
                                p.TipoDocumental.Trim().ToUpper().Equals(tipoDocumental.Name) &&
                                p.CategoriaAnexo.Contains(docConfigPadForm.CategoriaAnexo) &&
                                p.Ativo == true
                            ).FirstOrDefault();

                        if (docConfigPadDB == null)
                        {
                            docConfigPadDB = new Models.EDocs.Documento.DocConfigPad();
                        }
                        else
                        {
                            throw new Exception("Este documento já foi configurado.");
                        }

                        if (!string.IsNullOrWhiteSpace(docConfigPadForm.CategoriaAnexo))
                        {
                            docConfigPadDB.CategoriaAnexo = docConfigPadForm.Nome;
                        }

                        if (string.IsNullOrWhiteSpace(docConfigPadForm.Nome) || docConfigPadForm.Nome.Length < 6)
                        {
                            throw new Exception("O documento precisa de um nome para ser enviado ao E-Docs.");
                        }
                        else
                        {
                            docConfigPadDB.Nome = docConfigPadForm.Nome;
                        }

                        if (string.IsNullOrWhiteSpace(docConfigPadForm.Extensao))
                        {
                            throw new Exception("Por favor informe uma Extensão válida para os documentos.");
                        }

                        if (docConfigPadForm.Credenciar == null)
                        {
                            docConfigPadDB.Credenciar = false;
                        }

                        docConfigPadDB.AssinaturaTipo = docConfigPadForm.AssinaturaTipoId.AssinaturaTipoToEnum();
                        docConfigPadDB.DocumentoTipo = docConfigPadForm.DocumentoTipoId.DocumentoTipoToEnum();
                        docConfigPadDB.RestricaoTipo = docConfigPadForm.RestricaoTipoId.RestricaoTipoToEnum();
                        docConfigPadDB.ValorLegal = docConfigPadForm.ValorLegalId.ValorLegalToEnum();

                        if (docConfigPadDB.AssinaturaTipo == AssinaturaTipoEnum.Erro)
                        {
                            throw new Exception("Por favor indique um tipo de Assinatura Válida.");
                        }
                        if (docConfigPadDB.DocumentoTipo == DocumentoTipoEnum.Erro)
                        {
                            throw new Exception("Por favor indique um tipo de Documento Válido.");
                        }
                        if (docConfigPadDB.RestricaoTipo == RestricaoTipoEnum.Erro)
                        {
                            throw new Exception("Por favor indique um tipo de Restrição de Acesso Válida.");
                        }
                        if (docConfigPadDB.ValorLegal == ValorLegalEnum.Erro)
                        {
                            throw new Exception("Por favor indique um valor Legal Válido.");
                        }
                        if (docConfigPadDB.DocumentoTipo == DocumentoTipoEnum.Digitalizado &&
                                 docConfigPadDB.AssinaturaTipo != AssinaturaTipoEnum.SemAssinatura)
                        {
                            throw new Exception("Documentos com assinatura são considerados Natos-Digitais.");
                        }
                        if (
                            (docConfigPadDB.AssinaturaTipo == AssinaturaTipoEnum.AutoAssinado ||
                              docConfigPadDB.AssinaturaTipo == AssinaturaTipoEnum.Multiplas ||
                              docConfigPadDB.AssinaturaTipo == AssinaturaTipoEnum.ICPBrasil) &&
                              docConfigPadDB.ValorLegal != ValorLegalEnum.Original
                            )
                        {
                            throw new Exception("Todo documento assinado, é considerado Original.");
                        }
                        if (
                           !(docConfigPadDB.RestricaoTipo == RestricaoTipoEnum.Publico ||
                            docConfigPadDB.RestricaoTipo == RestricaoTipoEnum.Organizacional)
                            )
                        {
                            throw new Exception("O tipo de restrição escolhida ainda não foi implementada.");
                        }

                        if (docConfigPadForm.ClasseId != null && docConfigPadForm.ClasseId > 0)
                        {
                            var classe = _dbContext.EDocsClasses.Where(p =>
                                    p.Id == docConfigPadForm.ClasseId
                                ).FirstOrDefault();

                            if (classe == null)
                            {
                                throw new Exception("A classe informada não foi localizada.");
                            }
                            else
                            {
                                docConfigPadDB.Classe = classe;
                            }
                        }

                        _dbContext.EDocsDocConfigPad.Add(docConfigPadDB);
                        _dbContext.SaveChanges();

                        if (docConfigPadDB.AssinaturaTipo == AssinaturaTipoEnum.Multiplas)
                        {
                            return RedirectToAction("CadastrarAssinanteForm", docConfigPadDB.Id);
                        }
                        else
                        {
                            return View("DocConfigPadCadastrado");
                        }
                    }
                    else
                    {
                        return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Manager.ConfigurarDocumento.");
                    var msg = Uteis.MontarMsgErro("Manager.ConfigurarDocumento.", e);
                    return RedirectToAction("ErroResultado", msg);
                }
            }
            else
            {
                return View("DocConfigPad");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarAssinante(CIEO.Models.DTOFront.Manager.Assinante assinanteForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid? guid = ValidarGUID(User.FindFirst("guid"));
                    if (guid == null)
                    {
                        return RedirectToAction("ErroResultado", "Cookie inválido.");
                    }

                    if (!IsManager(guid.Value))
                    {
                        return RedirectToAction("ErroResultado", $"Usuário não autorizado.{guid}");
                    }

                    if (User.Identity.IsAuthenticated)
                    {
                        var docConfigPadDB = _dbContext.EDocsDocConfigPad
                            .Where(p =>
                                p.Id == assinanteForm.Id
                            ).FirstOrDefault();

                        if (docConfigPadDB == null)
                        {
                            throw new Exception("Documento Inválido.");
                        }

                        PessoaFisica? pessoaFisica = null;
                        if (assinanteForm.GUIDAssinante == null)
                        {
                            if (Uteis.CPFValido(assinanteForm.CPFAssinante))
                            {
                                throw new Exception("CPF Inválido.");
                            }
                            else
                            {
                                pessoaFisica = _dbContext.PGBPessoasFisicas
                                    .Where(p => p.CPF.Trim().Equals(assinanteForm.CPFAssinante))
                                    .FirstOrDefault();

                                if (pessoaFisica == null)
                                {
                                    pessoaFisica = new PessoaFisica();
                                    pessoaFisica.CPF = assinanteForm.CPFAssinante;
                                    OnBase.ManutencaoConexao();
                                    var wvApp = OnBase.App.WorkView.Applications.Find("PGB - Parametrização Global (Novo)");
                                    pessoaFisica.Criar(wvApp);

                                    assinanteForm.GUIDAssinante = Usuario.BaixarSub(Aplicacao.AcTokenSistema, pessoaFisica.CPF);
                                }
                            }
                        }

                        var usuario = _dbContext.ACUsuarios.Where(p =>
                            p.AcessoCidadaoId == assinanteForm.GUIDAssinante
                            )
                            .Include(p => p.Papeis)
                            .FirstOrDefault();

                        if (usuario == null)
                        {
                            usuario = new Usuario();
                            usuario.AcessoCidadaoId = assinanteForm.GUIDAssinante;
                            _dbContext.ACUsuarios.Add(usuario);
                            _dbContext.SaveChanges();
                        }
                        else if (usuario.PFObjectId != pessoaFisica.Id)
                        {
                            usuario.PFObjectId = pessoaFisica.Id.Value;
                            _dbContext.SaveChanges();
                        }

                        if (usuario.Papeis == null || usuario.Papeis.Count == 0)
                        {
                            var papeisDTO = Models.AcessoCidadao.DTO
                                .Papel.Baixar(Aplicacao.AcTokenSistema, assinanteForm.GUIDAssinante.Value);

                            if (papeisDTO != null && papeisDTO.Count > 0)
                            {
                                foreach (var papel in papeisDTO)
                                {
                                    usuario.Papeis.Add(
                                        new Models.AcessoCidadao.Papel(papel));
                                }
                                _dbContext.SaveChanges();
                            }

                            assinanteForm.Papeis = usuario.Papeis;
                        }

                        _dbContext.EDocsDocConfigPad.Add(docConfigPadDB);

                    }
                    else
                    {
                        return RedirectToAction("ErroResultado", $"Usuário não Autorizado.{guid}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Manager.ConfigurarDocumento.");
                    var msg = Uteis.MontarMsgErro("Manager.ConfigurarDocumento.", e);
                    return RedirectToAction("ErroResultado", msg);
                }
            }
            else
            {
                return View("DocConfigPad");
            }
        }

        public async Task<IActionResult> ErroResultado(string msg)
        {
            ViewBag.ErroMessage = msg;

            return View();
        }

        /// <summary>
        /// Verifica se a guid do cookie é válida.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private Guid? ValidarGUID(Claim guid)
        {
            if (guid == null)
            {
                return null;
            }
            else
            {
                if (Guid.TryParse(guid.Value, out var valorGUID))
                {
                    return valorGUID;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Verifica se o usuário logado é Manager
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool IsManager(Guid guid)
        {
            try
            {
                var usuario = _dbContext.ACUsuOBxUsuAC
                        .Where(p =>
                            p.UsuarioAC.AcessoCidadaoId.Equals(guid) &&
                            p.UsuarioOB.Disable == false &&
                            !p.UsuarioOB.UserName.Contains("deact")
                            )
                        .Include(p => p.UsuarioOB)
                        .ThenInclude(p => p.UserGroups)
                        .FirstOrDefault();

                if (usuario == null ||
                    usuario.UsuarioOB == null ||
                    usuario.UsuarioOB.UserGroups == null ||
                    usuario.UsuarioOB.UserGroups.Count == 0 ||
                    !usuario.UsuarioOB.UserGroups.Exists(p => p.Nome.Trim().Equals("MANAGER")))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("IsManager.", e);
            }
        }
    }
}