using CIEO.Models.EDocs.Documento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class ValorLegalConfig : IEntityTypeConfiguration<ValorLegal>
    {
        public void Configure(EntityTypeBuilder<ValorLegal> builder)
        {
            var convByteShort = Conversores.ConversorByteShort();

            builder
              .ToTable("CC_EDocs_ValorLegal");

            builder
                .HasKey(a => a.Id);

            builder
               .Property(a => a.Id)
               .HasConversion(convByteShort);
        }
    }
}