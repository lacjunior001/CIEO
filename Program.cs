using CIEO.DB;
using CIEO.Extension;
using CIEO.Models;
using CIEO.Models.AcessoCidadao;
using CIEO.Models.DTOFront.Manager;
using CIEO.Servicos;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CIEO
{
    public class Program
    {
        internal static string? UrlBaseEdocs { get; private set; }
        internal static Ambiente AmbienteDestino { get; private set; }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder
                .Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            UrlBaseEdocs = builder.Configuration["UrlBaseEDocs"].ToString();

            AmbienteDestino = builder.Configuration["ServidorDestino"].AmbienteToEnum();

            builder
                .Logging.ClearProviders();

            builder
                .Host.UseNLog();

            builder
                .Services.AddDbContext<DB.CIEOContext>();

            builder
                .Services.AddSingleton<Servicos.Cache>();

            builder
                .Services.AddHostedService<SegundoPlano>();

            builder
                .Services
                    .AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = "";
                    config.DefaultChallengeScheme = "";
                })
                    .AddCookie("", config =>
                {
                    config.Cookie.Name = "";
                    config.SlidingExpiration = false;
                    config.ExpireTimeSpan = new TimeSpan(0, 50, 0);
                    config.Cookie.SameSite = SameSiteMode.Lax;
                    config.Cookie.HttpOnly = true;
                    config.ForwardChallenge = "";
                    config.Cookie.IsEssential = true;
                })
                    .AddOpenIdConnect("", config =>
                {
                    config.Authority = "";
                    config.RequireHttpsMetadata = true;
                    config.ClientId = builder.Configuration["AcLogin:ClientId"].ToString();
                    config.ClientSecret = builder.Configuration["AcLogin:ClientSecret"].ToString();
                    config.ResponseType = "code id_token";
                    config.SaveTokens = true;
                    config.GetClaimsFromUserInfoEndpoint = true;
                    config.SignInScheme = "";
                    config.Scope.Clear();
                    var scopes = builder.Configuration.GetSection("AcLogin:Scopes").Get<string[]>();

                    Models.AcessoCidadao.Usuario usuario = null;
                    config.Events = new OpenIdConnectEvents
                    {
                        OnTokenResponseReceived = async evento =>
                        {
                            await Task.Yield();

                            usuario = new Usuario();
                            var jwt = new JwtSecurityTokenHandler();
                            var valores = jwt.ReadJwtToken(evento.TokenEndpointResponse.AccessToken);
                            usuario.TokenValidade = valores.ValidTo.ToLocalTime();
                            usuario.TokenGerado = valores.ValidFrom.ToLocalTime();
                            usuario.AccessToken = new Token();
                            usuario.AccessToken.AcessToken = evento.TokenEndpointResponse.AccessToken;
                        },
                        OnUserInformationReceived = async evento =>
                        {
                            await Task.Yield();

                            usuario.AcessoCidadaoId = new Guid(evento.User.RootElement.GetProperty("subNovo").ToString());
                            string apelido = evento.User.RootElement.GetProperty("apelido").ToString();
                            string cpf = evento.User.RootElement.GetProperty("cpf").ToString();
                            string emailAtual = evento.User.RootElement.GetProperty("email").ToString();

                            if (usuario != null)
                            {
                                usuario.ExecutarAtualizacaoLogin(emailAtual, cpf, apelido);
                            }

                            var claims = new List<Claim>
                            {
                                new Claim("apelido", apelido),
                                new Claim("guid", usuario.AcessoCidadaoId.ToString()),
                            };

                            evento.Principal.AddIdentity(new ClaimsIdentity(claims));
                        }
                    };
                });

            builder
                .Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder
                .Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.RequireHeaderSymmetry = false;
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            builder
                .Services.AddControllersWithViews();

            var app = builder.Build();

            app.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.Always
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}