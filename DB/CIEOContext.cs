using Microsoft.EntityFrameworkCore;
namespace CIEO.DB
{
    public class CIEOContext : DbContext
    {
        public DbSet<Models.PGB.TipoContato> PGBTiposContato { get; set; }
        public DbSet<Models.PGB.OrigemInformacao> PGBOrigensInformacoes { get; set; }
        public DbSet<Models.PGB.Contato> PGBContatos { get; set; }
        public DbSet<Models.PGB.PessoaFisica> PGBPessoasFisicas { get; set; }
        public DbSet<Models.PGB.UsuarioOB> PGBUsuariosOB { get; set; }

        public DbSet<Models.AcessoCidadao.Local> ACLocais { get; set; }
        public DbSet<Models.AcessoCidadao.Papel> ACPapeis { get; set; }
        public DbSet<Models.AcessoCidadao.Usuario> ACUsuarios { get; set; }
        public DbSet<Models.AcessoCidadao.UsuOBXUsuAC> ACUsuOBxUsuAC { get; set; }
        public DbSet<Models.AcessoCidadao.Token> ACTokens { get; set; }

        public DbSet<Models.EDocs.FundamentoLegal> EDocsFundamentos { get; set; }
        public DbSet<Models.EDocs.TipoRestricaoAcesso> EDocsTipoRestricao { get; set; }
        public DbSet<Models.EDocs.Classe> EDocsClasses { get; set; }
        public DbSet<Models.EDocs.Evento> EDocsEventos { get; set; }
        public DbSet<Models.EDocs.Documento.Documento> EDocsDocumentos { get; set; }
        public DbSet<Models.EDocs.Documento.Assinante> EDocsAssinantes { get; set; }
        public DbSet<Models.EDocs.Documento.DocConfigPad> EDocsDocConfigPad { get; set; }
        public DbSet<Models.EDocs.Documento.TipoAssinatura> EDocsDocTipAss { get; set; }
        public DbSet<Models.EDocs.Documento.TipoDocumento> EDocsDocTipDoc { get; set; }
        public DbSet<Models.EDocs.Documento.ValorLegal> EDocsDocValLegal { get; set; }

        public DbSet<Models.EDocs.Processo.Processo> EDocsProcessos { get; set; }
        public DbSet<Models.EDocs.Processo.Interessado> EDocsInteressados { get; set; }
        public DbSet<Models.EDocs.Processo.Autuacao> EDocsAutuacoes { get; set; }
        public DbSet<Models.EDocs.Processo.AtoXDocumento> EDocsAtosXDocumentos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);

            optionsBuilder.EnableSensitiveDataLogging();

            switch (Program.AmbienteDestino)
            {
                case Models.Ambiente.Producao:
                    optionsBuilder.UseSqlServer("");
                    break;

                case Models.Ambiente.Homologacao:
                    optionsBuilder.UseSqlServer("");
                    break;

                case Models.Ambiente.Desenvolvimento:
                    optionsBuilder.UseSqlServer("");
                    break;

                default:
                    throw new Exception("Servidor destino inesperado.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("hsi");

            modelBuilder.ApplyConfiguration(new PGBConfig.ContatoConfig());
            modelBuilder.ApplyConfiguration(new PGBConfig.OrigemInformacaoConfig());
            modelBuilder.ApplyConfiguration(new PGBConfig.PessoaFisicaConfig());
            modelBuilder.ApplyConfiguration(new PGBConfig.TipoContatoConfig());
            modelBuilder.ApplyConfiguration(new PGBConfig.UsuarioOBConfig());
            modelBuilder.ApplyConfiguration(new PGBConfig.UserGroupOBConfig());

            modelBuilder.ApplyConfiguration(new ACConfig.LocalConfig());
            modelBuilder.ApplyConfiguration(new ACConfig.PapelConfig());
            modelBuilder.ApplyConfiguration(new ACConfig.UsuarioConfig());
            modelBuilder.ApplyConfiguration(new ACConfig.TokenConfig());
            modelBuilder.ApplyConfiguration(new ACConfig.UsuOBXUsuACConfig());

            modelBuilder.ApplyConfiguration(new EDocsConfig.FundamentoLegalConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.TipoRestricaoConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.ClasseConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.DocumentoConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.DocConfigPadConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.ValorLegalConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.TipoAssinaturaConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.TipoDocumentoConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.ProcessoConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.InteressadoConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.AtoConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.AutuacoesConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.AtoXDocumentoConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.AssinanteConfig());
            modelBuilder.ApplyConfiguration(new EDocsConfig.EventoConfig());
        }
    }
}
