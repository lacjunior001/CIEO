using CIEO.Models.AcessoCidadao;
using CIEO.Models.PGB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.PGBConfig
{
    internal class TipoContatoConfig : IEntityTypeConfiguration<TipoContato>
    {
        public void Configure(EntityTypeBuilder<TipoContato> builder)
        {
            var convGuid = Conversores.ConversorGuid();

            builder
               .ToView("rm_PGBTipoContatoNovo");

            builder
                .HasKey(a => a.Id);
        }
    }
}