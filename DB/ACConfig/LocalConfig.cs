using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.ACConfig
{
    public class LocalConfig : IEntityTypeConfiguration<Models.AcessoCidadao.Local>
    {
        public void Configure(EntityTypeBuilder<Models.AcessoCidadao.Local> builder)
        {
            var convGuid = Conversores.ConversorGuid();

            builder
               .ToTable("CC_AC_Local");

            builder
                .HasKey(a => a.Id);
                                    
            builder
              .Property(a => a.AcessoCidadaoId)
              .HasColumnType("char(36)")
              .HasConversion(convGuid);

            builder
                .HasOne(a => a.LocalPai)
                .WithMany(a => a.LocaisFilho)
                .HasForeignKey("LocalPaiId")
                .IsRequired(false);
        }
    }
}
