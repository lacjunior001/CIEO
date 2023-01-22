using CIEO.Models.EDocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class TipoRestricaoConfig : IEntityTypeConfiguration<TipoRestricaoAcesso>
    {
        public void Configure(EntityTypeBuilder<TipoRestricaoAcesso> builder)
        {
            var convByteShort = Conversores.ConversorByteShort();

            builder
              .ToTable("CC_EDocs_TipoRestricao");

            builder
                .HasKey(a => a.Id);

            builder
               .Property(a => a.Id)
               .HasConversion(convByteShort);
        }
    }
}