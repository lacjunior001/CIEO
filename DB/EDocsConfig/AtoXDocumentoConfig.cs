using CIEO.Models.EDocs.Processo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class AtoXDocumentoConfig : IEntityTypeConfiguration<Models.EDocs.Processo.AtoXDocumento>
    {
        public void Configure(EntityTypeBuilder<AtoXDocumento> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convShort = Conversores.ConversorByteShort();

            builder
                .ToTable("CC_EDocs.AtoXDocumento");

            builder
               .HasKey(a => a.Id);

            builder
                .Property(a => a.Id)
                .HasConversion(convGuid);

            builder
                .Property(a => a.Posicao)
                .HasColumnName("Ordem")
                .HasConversion(convShort);

            builder
                .HasOne<Autuacao>(a => (Autuacao)a.Ato)
                .WithMany(a => a.Documentos)
                .HasForeignKey("AtoId");

            builder
                .HasOne(a => a.Documento)
                .WithMany()
                .HasForeignKey("DocumentHandle");
        }
    }
}