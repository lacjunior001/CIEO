using CIEO.Models.EDocs.Documento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class DocConfigPadConfig : IEntityTypeConfiguration<DocConfigPad>
    {
        public void Configure(EntityTypeBuilder<DocConfigPad> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convRestricaoTipo = Conversores.ConversorRestricaoTipo();
            var convAgenteTipo = Conversores.ConversorAgenteTipo();
            var convSituacao = Conversores.ConversorSituacao();
            var convAssinaturaTipo = Conversores.ConversorAssinaturaTipo();
            var convDocumentoTipo = Conversores.ConversorDocumentoTipo();
            var convValorLegal = Conversores.ConversorValorLegal();

            builder
               .ToTable("CC_EDocs_DocConfigPad");

            builder
                .HasKey(a => a.Id);

            builder
              .Property(a => a.TipoDocumental)
              .HasColumnName("TipoDocumental");

            builder
              .Property(a => a.CategoriaAnexo)
              .HasColumnName("CategoriaAnexo");

            builder
              .Property(a => a.Nome)
              .HasColumnName("Nome");

            builder
              .Property(a => a.Extensao)
              .HasColumnName("Extensao");

            builder
              .Property(a => a.Credenciar)
              .HasColumnName("Credenciar");

            builder
               .Property(a => a.AssinaturaTipo)
               .HasColumnName("TipoAssinaturaId")
               .HasConversion(convAssinaturaTipo);

            builder
                .HasMany(p => p.Assinantes)
                .WithOne(p => p.DocAssiConfig)
                .HasForeignKey("DocConfigPadId");

            builder
               .Property(a => a.DocumentoTipo)
               .HasColumnName("TipoDocumentoId")
               .HasConversion(convDocumentoTipo);

            builder
              .Property(a => a.RestricaoTipo)
              .HasColumnName("TipoRestricaoId")
              .HasConversion(convRestricaoTipo);

            builder
               .Property(a => a.ValorLegal)
               .HasColumnName("ValorLegalId")
               .HasConversion(convValorLegal);

            builder
                .HasOne(a => a.Classe)
                .WithMany()
                .HasForeignKey("ClasseId");

            builder
              .Property(a => a.ValorLegal)
              .HasColumnName("ValorLegalId")
              .HasConversion(convValorLegal);

            builder
              .Property(a => a.Ativo)
              .HasColumnName("Ativo");
        }
    }
}