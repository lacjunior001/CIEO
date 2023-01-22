using CIEO.Models.AcessoCidadao;
using CIEO.Models.PGB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.PGBConfig
{
    internal class OrigemInformacaoConfig : IEntityTypeConfiguration<OrigemInformacao>
    {
        public void Configure(EntityTypeBuilder<OrigemInformacao> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convBool = Conversores.ConversorBoolToLong();

            builder
               .ToView("rm_PGBOrigemInformacaoNovo");

            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.Oculto)
                .HasConversion(convBool);
        }
    }
}