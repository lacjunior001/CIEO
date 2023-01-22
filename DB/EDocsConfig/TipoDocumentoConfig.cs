using CIEO.Models.EDocs.Documento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class TipoDocumentoConfig : IEntityTypeConfiguration<TipoDocumento>
    {
        public void Configure(EntityTypeBuilder<TipoDocumento> builder)
        {
            var convByte = Conversores.ConversorByteShort();

            builder
              .ToTable("CC_EDocs_TipoDocumento");

            builder
                .HasKey(a => a.Id);

            builder
               .Property(a => a.Id)
               .HasConversion(convByte);
        }
    }
}