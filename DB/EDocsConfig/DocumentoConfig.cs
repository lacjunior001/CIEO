using CIEO.Models.EDocs.Documento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    public class DocumentoConfig : IEntityTypeConfiguration<Models.EDocs.Documento.Documento>
    {
        public void Configure(EntityTypeBuilder<Documento> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convRestricaoTipo = Conversores.ConversorRestricaoTipo();
            var convAgenteTipo = Conversores.ConversorAgenteTipo();
            var convSituacao = Conversores.ConversorSituacao();
            var convAssinaturaTipo = Conversores.ConversorAssinaturaTipo();
            var convDocumentoTipo = Conversores.ConversorDocumentoTipo();
            var convValorLegal = Conversores.ConversorValorLegal();

            builder
               .ToTable("CC_EDocs.Documento");

            builder
                .HasKey(a => a.DocumentHandle);

            builder
               .Property(a => a.EDocsId)
               .HasConversion(convGuid);

            builder
                .HasOne(p => p.Cidadao)
                .WithMany()
                .HasForeignKey("UsuarioId");

            builder
                .HasOne(p => p.AgentePublico)
                .WithMany()
                .HasForeignKey("PapelId");

            builder
               .Property(a => a.AssinaturaTipo)
               .HasColumnName("AssinaturaTipoId")
               .HasConversion(convAssinaturaTipo);

            //builder
            //    .HasMany(p => p.Assinantes)
            //    .WithOne(p => p.DocumentoAssinado)
            //    .HasForeignKey("DocumentHandle");

            builder
               .Property(a => a.DocumentoTipo)
               .HasColumnName("DocumentoTipoId")
               .HasConversion(convDocumentoTipo);

            builder
               .Property(a => a.ValorLegal)
               .HasColumnName("ValorLegalId")
               .HasConversion(convValorLegal);

            builder
                .HasOne(a => a.Classe)
                .WithMany()
                .HasForeignKey("ClasseId");

            builder
               .Property(a => a.RestricaoTipo)
               .HasColumnName("RestricaoTipoId")
               .HasConversion(convRestricaoTipo);

            builder
              .Property(a => a.Situacao)
              .HasColumnName("SituacaoId")
              .HasConversion(convSituacao);
        }
    }
}
