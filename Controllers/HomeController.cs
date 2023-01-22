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

namespace CIEO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CIEOContext _dbContext;

        public HomeController(ILogger<HomeController> logger, CIEOContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var apelido = User.FindFirst("apelido");
                if (apelido != null)
                {
                    ViewBag.Apelido = apelido.Value.ToString();
                }
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> LogIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                var apelido = User.FindFirst("apelido");
                if (apelido != null)
                {
                    ViewBag.Apelido = apelido.Value.ToString();
                }
            }

            return View("Index");
        }

        [Authorize]
        public async Task<IActionResult> RenovarChave()
        {
            try
            {
                ViewBag.Apelido = User.FindFirst("apelido").Value.ToString();

                Claim? claimGUID = User.FindFirst("guid");
                if (claimGUID == null)
                {
                    throw new Exception("Não foi possível identificar o usuário logado.");
                }

                var userGUID = new Guid(User.FindFirst("guid").Value.ToString().Trim());

                var usuario = await _dbContext
                    .ACUsuarios
                    .Where(p => p.AcessoCidadaoId.Equals(userGUID))
                    .Include(p => p.Papeis)
                    .FirstAsync();

                if (usuario == null)
                {
                    throw new Exception("Usuário logado está sem registro no banco.");
                }

                var pessoa = await _dbContext
                    .PGBPessoasFisicas
                    .Where(p => p.Id.Equals(usuario.PFObjectId))
                    .FirstAsync();

                if (pessoa == null)
                {
                    throw new Exception("Usuário logado está sem registro no OnBase.");
                }

                string? nomePapelPreferencial = null;

                if (usuario.Papeis == null || usuario.Papeis.Count == 0)
                {
                    nomePapelPreferencial = "Usuário sem Papel";
                }
                else
                {
                    var papelPreferencial = usuario.Papeis.Find(p => p.Preferencial.Equals(true));

                    if (papelPreferencial == null)
                    {
                        nomePapelPreferencial = usuario.Papeis[0].Nome;
                    }
                    else
                    {
                        nomePapelPreferencial = papelPreferencial.Nome;
                    }
                }

                var renovarToken = new Models.DTOFront.RenovarChave
                {
                    Cpf = pessoa.CPF,
                    Validade = usuario.TokenValidade == null ? DateTime.MinValue : usuario.TokenValidade.Value,
                    PapelPreferencial = nomePapelPreferencial
                };

                return View(renovarToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "CIEO.RenovarChave. ");
                throw new Exception("Renovar Chave. ", e);
            }
        }

        [Authorize]
        public async Task LogOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync("CIEO_Auth");
                await HttpContext.SignOutAsync("ACCIEO");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}