using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.ACConfig
{
    internal class PapelConfig : IEntityTypeConfiguration<Models.AcessoCidadao.Papel>
    {
        public void Configure(EntityTypeBuilder<Models.AcessoCidadao.Papel> builder)
        {
            var convGuid = Conversores.ConversorGuid();

            builder
               .ToTable("CC_AC_Papel");

            builder
                .HasKey(a => a.Id);
          
            builder
                .Property(a => a.AcessoCidadaoId)
                .HasColumnType("char(36)")
                .HasConversion(convGuid);

            builder
                .Property(a=> a.Ativo)
                .HasColumnType("bit")
                .HasConversion<bool>();

            builder
                .Property(a => a.Preferencial)
                .HasColumnType("bit")
                .HasConversion<bool>();

            builder
                .HasOne(a => a.Lotacao)
                .WithMany()
                .HasForeignKey("LocalId")
                .IsRequired(false);

            builder
                .HasOne(a => a.Usuario)
                .WithMany(a => a.Papeis)
                .HasForeignKey("UsuarioId")
                .IsRequired(false);
        }
    }
}