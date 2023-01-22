using CIEO.Models.EDocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class ClasseConfig : IEntityTypeConfiguration<Models.EDocs.Classe>
    {
        public void Configure(EntityTypeBuilder<Classe> builder)
        {
            var convGuid = Conversores.ConversorGuid();

            builder
               .ToTable("CC_EDocs_Classe");

            builder
                .HasKey(a => a.Id);

            builder
              .Property(a => a.AcessoCidadaoId)
              .HasColumnType("char")
              .HasConversion(convGuid);
        }
    }
}