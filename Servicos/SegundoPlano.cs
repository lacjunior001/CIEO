

using CIEO.DB;
using CIEO.Models.AcessoCidadao;
using CIEO.Models.EDocs;
using CIEO.Models.EDocs.Processo;
using Microsoft.EntityFrameworkCore;

namespace CIEO.Servicos
{
    /// <summary>
    /// Classe dedicada ao disparo de serviços em segundo plano de limpeza de informações vencidas do cache.
    /// </summary>
    public sealed class SegundoPlano : IHostedService
    {
        private readonly Cache _cache;
        private readonly ILogger<SegundoPlano> _logger;
        private readonly IConfiguration _configuration;

        private Timer? AtualizacaoSemanal;
        private bool ExecutandoAtualizacaoSemanal;

        private Timer? VerificarEventos;
        private bool ExecutandoVerificarEventos;

        private Timer? EnviarDadosEDocs;
        private bool ExecutandoEnviarDadosEDocs;

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="cache"></param>
        public SegundoPlano(Cache cache, ILogger<SegundoPlano> log, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
            _logger = log;

            ExecutandoAtualizacaoSemanal = false;
            ExecutandoEnviarDadosEDocs = false;
            ExecutandoVerificarEventos = false;
        }

        /// <summary>
        /// Inicia a execução do serviço.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //AtualizacaoSemanal = new Timer(ExecutarAtualizacaoSemanal, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            //EnviarDadosEDocs = new Timer(ExecutarEnviarDadosEDocs, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            //VerificarEventos = new Timer(ExecutarVerificacaoEvento, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{DateTime.Now} - SegundoPlano.StopAsync foi chamado.");
            return Task.CompletedTask;
        }

        private void ExecutarAtualizacaoSemanal(object? objeto)
        {
            try
            {
                if (!ExecutandoAtualizacaoSemanal)
                {
                    ExecutandoAtualizacaoSemanal = true;

                    var locais = Models.AcessoCidadao.Local.Baixar(Models.AcessoCidadao.Aplicacao.AcTokenSistema);

                    using (var contexto = new CIEOContext())
                    {
                        for (int i = 0; i < locais.Count; i++)
                        {
                            if (locais[i].Nome != null && locais[i].Nome.Length > 104)
                            {
                                string achou = "nomegrande";
                            }

                            if (locais[i].Sigla != null && locais[i].Sigla.Length > 20)
                            {
                                string achou = "sigla";
                            }

                            if (locais[i].Tipo != null && locais[i].Tipo.Length > 25)
                            {
                                string achou = "sigla";
                            }

                            var localDB = contexto.ACLocais.Where(p => p.AcessoCidadaoId.Equals(locais[i].AcessoCidadaoId)).FirstOrDefault();
                            if (localDB == null)
                            {
                                Models.AcessoCidadao.Local localPaiDB = null;
                                if (locais[i].LocalPai != null)
                                {
                                    localPaiDB = contexto.ACLocais.Where(p => p.AcessoCidadaoId.Equals(locais[i].LocalPai.AcessoCidadaoId)).FirstOrDefault();
                                    if (localPaiDB == null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        locais[i].LocalPai = localPaiDB;
                                        contexto.ACLocais.Add(locais[i]);
                                        contexto.SaveChanges();
                                    }
                                }
                                else
                                {
                                    contexto.ACLocais.Add(locais[i]);
                                    contexto.SaveChanges();
                                }
                            }
                        }

                        //Falta Lógica para desativar um local quando não vier da Prodest.
                        //Falta lógica para atualizar as informações de um local que existe no banco quando for o caso.

                        var detranLocal = contexto
                                            .ACLocais
                                            .Where(p => p.Sigla.Contains("DETRAN"))
                                            .Include(P => P.LocalPai)
                                            .FirstOrDefault();

                        /**
                        var fundamentos = Models.EDocs.FundamentoLegal.Baixar(Models.AcessoCidadao.Aplicacao.AcTokenSistema, detranLocal.LocalPai.AcessoCidadaoId.Value);

                        for (int i = 0; i < fundamentos.Count; i++)
                        {
                            if (fundamentos[i].Nome != null && fundamentos[i].Nome.Length > 200)
                            {
                                string parar = "nome grande";
                            }


                            var fundamentoDB = contexto.EDocsFundamentos.Where(p => p.Id.Equals(fundamentos[i].Id)).FirstOrDefault();
                            if (fundamentoDB == null)
                            {
                                if (fundamentos[i].Patriarca != null)
                                {
                                    fundamentos[i].Patriarca = contexto.ACLocais.Where(p => p.Id.Equals(fundamentos[i].Patriarca.Id)).FirstOrDefault();
                                }

                                contexto.EDocsFundamentos.Add(fundamentos[i]);
                                contexto.SaveChanges();
                            }
                        }*/

                        var classes = Models.EDocs.Classe.Baixar(Models.AcessoCidadao.Aplicacao.AcTokenSistema, detranLocal.LocalPai.AcessoCidadaoId.Value);

                        for (int i = 0; i < classes.Count; i++)
                        {
                            var classesDB = contexto.EDocsClasses.Where(p => p.Id.Equals(classes[i].Id)).FirstOrDefault();
                            if (classesDB == null)
                            {
                                classes[i].Ativo = true;
                                contexto.EDocsClasses.Add(classes[i]);
                                contexto.SaveChanges();
                            }
                        }
                    }
                }

                ExecutandoAtualizacaoSemanal = false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - SegundoPlano.ExecutarAtualizacaoSemanal Ocorreu Erro {e.Message}.");
                _logger.LogError(e, "SegundoPlano.ExecutarLimpeza.");
                ExecutandoAtualizacaoSemanal = false;
            }
        }

        private void ExecutarVerificacaoEvento(object? objeto)
        {

            try
            {
                if (ExecutandoVerificarEventos)
                {
                    //se estiver executando faz nada;
                }
                else
                {
                    ExecutandoVerificarEventos = true;

                    using (var contexto = new CIEOContext())
                    {
                        var eventos = contexto.EDocsEventos
                            .Where(p =>
                                    p.Situacao.Equals(Models.EDocs.EventoSituacaoEnum.Criado) ||
                                    p.Situacao.Equals(Models.EDocs.EventoSituacaoEnum.Enfileirado) ||
                                    p.Situacao.Equals(Models.EDocs.EventoSituacaoEnum.Executado) ||
                                    p.Situacao.Equals(Models.EDocs.EventoSituacaoEnum.Processando)
                                    )
                            .Include(p => p.DocumentoRelacionado)
                            .Include(p => p.AtoRelacionado)
                            .ThenInclude(p => p.Processo)
                            .ToList();

                        var token = Models.AcessoCidadao.Aplicacao.AcTokenSistema;
                        bool salvar = false;
                        foreach (var evento in eventos)
                        {
                            try
                            {
                                salvar = evento.Consultar(token);
                            }
                            catch (Exception e)
                            {
                                string msg = e.ToString();
                            }
                        }

                        if (salvar)
                        {
                            contexto.SaveChanges();
                        }
                    }

                    ExecutandoVerificarEventos = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} - SegundoPlano.ExecutarAtualizacaoSemanal Ocorreu Erro {e.Message}.");
                _logger.LogError(e, "SegundoPlano.ExecutarLimpeza.");
                ExecutandoVerificarEventos = false;
            }
        }

        private void ExecutarEnviarDadosEDocs(object? objeto)
        {
            try
            {
                if (ExecutandoEnviarDadosEDocs)
                {
                }
                else
                {
                    ExecutandoEnviarDadosEDocs = true;

                    using (var contexto = new CIEOContext())
                    {
                        var listaEventos = contexto.EDocsEventos.Where(p => p.Situacao.Equals(EventoSituacaoEnum.AguardandoCriacao))
                            .Include(a => a.AtoRelacionado)
                            .Include(a => a.DocumentoRelacionado)
                            .ToList();

                        bool salvar = false;

                        foreach (var evento in listaEventos)
                        {
                            if (evento.AtoRelacionado == null)
                            {
                                //Primeiro tenta enviar Documentos que houve uma tentativa ainda.
                                if (evento.DocumentoRelacionado.Situacao == Models.EDocs.SituacaoEnum.AguardandoEnvioEDocs)
                                {
                                    contexto.Entry(evento.DocumentoRelacionado)
                                        .Reference(b => b.AgentePublico)
                                        .Load();

                                    Usuario usuario = null;

                                    if (evento.DocumentoRelacionado.AgentePublico != null)
                                    {
                                        usuario = contexto.ACUsuarios
                                            .Where(p => p.Id.Equals(evento.DocumentoRelacionado.AgentePublico.Usuario))
                                            .Include(p => p.AccessToken)
                                            .FirstOrDefault();
                                    }

                                    contexto.Entry(evento.DocumentoRelacionado)
                                        .Reference(b => b.Cidadao)
                                        .Load();

                                    if (evento.DocumentoRelacionado.Cidadao != null)
                                    {
                                        usuario = contexto.ACUsuarios
                                            .Where(p => p.Id.Equals(evento.DocumentoRelacionado.Cidadao))
                                            .Include(p => p.AccessToken)
                                            .FirstOrDefault();
                                    }

                                    contexto.Entry(evento.DocumentoRelacionado)
                                        .Reference(b => b.Classe)
                                        .Load();
                                    /*
                                    contexto.Entry(evento.DocumentoRelacionado)
                                        .Collection(b => b.Assinantes)
                                        .Load();

                                    foreach (var assinante in evento.DocumentoRelacionado.Assinantes)
                                    {
                                        contexto.Entry(assinante)
                                            .Reference(b => b.Cidadao)
                                            .Load();

                                        contexto.Entry(assinante)
                                            .Reference(b => b.AgentePublico)
                                            .Load();
                                    }*/

                                    try
                                    {
                                        evento.IdEDocs = evento.DocumentoRelacionado.RegistrarEDocs(usuario.AccessToken.AcessToken);
                                        evento.Situacao = EventoSituacaoEnum.Criado;
                                        salvar = true;
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                    /**
                                    if (guidEventoRecebida == null)
                                    {
                                        if ("Erro Envio".Equals(msgErro))
                                        {
                                            evento.DocumentoEnviado.Situacao = Models.EDocs.SituacaoEnum.ErroEnvioEdocs;
                                            salvar = true;
                                        }
                                        else
                                        {
                                            evento.DocumentoEnviado.Situacao = Models.EDocs.SituacaoEnum.ErroGuidVazia;
                                            salvar = true;
                                        }
                                    }
                                    else
                                    {
                                        guidEventoRecebida = guidEventoRecebida.Replace("\"", "");
                                        evento.IdEDocs = new Guid(guidEventoRecebida);
                                        evento.Situacao = Models.EDocs.EventoSituacaoEnum.Criado;
                                        evento.DocumentoEnviado.Situacao = Models.EDocs.SituacaoEnum.AguardandoRespostaEDocs;
                                        salvar = true;
                                    }*/
                                }
                            }
                            else
                            {
                                /**
                                if (evento.Ato.Situacao == Models.EDocs.SituacaoEnum.AguardandoEnvioEDocs ||
                                    evento.Ato.Situacao == Models.EDocs.SituacaoEnum.ErroEnvioEdocs)
                                {

                                    var docsRelacionados = contexto.EDocsAtosXDocumentos
                                              .Where(p => p.Ato.Equals(evento.Ato))
                                              .Include(p => p.Documento)
                                              .ToList();

                                    var listaGUID = new List<Guid>(20);

                                    if (docsRelacionados != null && docsRelacionados.Count > 0)
                                    {
                                        var naoenv = docsRelacionados.Find(p => !p.Documento.Situacao.Equals(Models.EDocs.SituacaoEnum.EnviadoParaEDocs));
                                        if (naoenv != null)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            docsRelacionados.Sort((x, y) => x.Ordem.Value.CompareTo(y.Ordem.Value));

                                            foreach (var item in docsRelacionados)
                                            {
                                                listaGUID.Add(item.Documento.EDocsId.Value);
                                            }
                                        }
                                    }

                                    var usuario = contexto.ACUsuarios
                                        .Where(p => p.Papeis.Contains(evento.Ato.PapelPraticante))
                                        .Include(p => p.AccessToken)
                                        .Include(p => p.Papeis)
                                        .FirstOrDefault();

                                    string guidenvio = null;
                                    string msgErro = null;
                                    try
                                    {
                                        switch (evento.Ato)
                                        {
                                            case Models.EDocs.Processo.Autuacao:

                                                var autuacao = contexto.EDocsAutuacoes.Where(p => p.Equals(evento.Ato))
                                                    .Include(a => a.LocaPraticado)
                                                    .Include(a => a.PapelPraticante)
                                                    .Include(a => a.Processo)
                                                    .ThenInclude(p => p.Interessados)
                                                    .Include(a => a.Processo)
                                                    .ThenInclude(p => p.Localizacao)
                                                    .Include(a => a.Processo)
                                                    .ThenInclude(p => p.Classe)
                                                    .FirstOrDefault();

                                                guidenvio = autuacao.EnviarParaEdocs(usuario.AccessToken.AcessToken, listaGUID);

                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        msgErro = "Erro Envio";
                                        guidenvio = null;
                                        _logger.LogError(e, "SegundoPlano.EnviarAto.");
                                    }

                                    if (guidenvio == null)
                                    {
                                        if ("Erro Envio".Equals(msgErro))
                                        {
                                            evento.Ato.Situacao = Models.EDocs.SituacaoEnum.ErroEnvioEdocs;
                                            salvar = true;
                                        }
                                        else
                                        {
                                            evento.Ato.Situacao = Models.EDocs.SituacaoEnum.ErroGuidVazia;
                                            salvar = true;
                                        }
                                    }
                                    else
                                    {
                                        guidenvio = guidenvio.Replace("\"", "");
                                        evento.IdEDocs = new Guid(guidenvio);
                                        evento.Situacao = Models.EDocs.EventoSituacaoEnum.Criado;
                                        evento.Ato.Situacao = Models.EDocs.SituacaoEnum.AguardandoRespostaEDocs;
                                        salvar = true;
                                    }
                                }
                                
                                */
                            }
                        }

                        if (salvar)
                        {
                            contexto.SaveChanges();
                        }
                    }

                    ExecutandoEnviarDadosEDocs = false;
                }
            }
            catch (Exception e)
            {
                ExecutandoEnviarDadosEDocs = false;
                Console.WriteLine($"{DateTime.Now} - SegundoPlano.ExecutarEnviarDadosEDocs Ocorreu Erro {e.Message}.");
                _logger.LogError(e, "SegundoPlano.ExecutarEnviarDadosEDocs.");
            }
        }
    }
}
