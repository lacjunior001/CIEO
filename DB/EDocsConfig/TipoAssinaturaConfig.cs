using CIEO.Models.EDocs.Documento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class TipoAssinaturaConfig : IEntityTypeConfiguration<TipoAssinatura>
    {
        public void Configure(EntityTypeBuilder<TipoAssinatura> builder)
        {
            var convByte = Conversores.ConversorByteShort();

            builder
              .ToTable("CC_EDocs_TipoAssinatura");

            builder
                .HasKey(a => a.Id);

            builder
               .Property(a => a.Id)
               .HasConversion(convByte);
        }
    }
}