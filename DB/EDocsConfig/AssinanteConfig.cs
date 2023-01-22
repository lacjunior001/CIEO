using CIEO.Models.EDocs.Documento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class AssinanteConfig : IEntityTypeConfiguration<Models.EDocs.Documento.Assinante>
    {
        public void Configure(EntityTypeBuilder<Assinante> builder)
        {
            var convGuid = Conversores.ConversorGuid();

            builder
               .ToTable("CC_EDocs_DocAssinante");

            builder
                .HasKey(a => a.Id);

            builder
                .HasOne(p => p.DocAssiConfig)
                .WithMany(p => p.Assinantes)
                .HasForeignKey("DocConfigPadId");

            builder
               .HasOne(p => p.UsuarioIndicado)
               .WithMany()
               .HasForeignKey("UsuarioId");

            builder
               .HasOne(p => p.PapelIndicado)
               .WithMany()
               .HasForeignKey("PapelId");

            builder
               .Property(p => p.Ativo)
               .HasColumnName("Ativo");
        }
    }
}