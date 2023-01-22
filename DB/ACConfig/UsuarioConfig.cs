using CIEO.Models.AcessoCidadao;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.ACConfig
{
    internal class UsuarioConfig : IEntityTypeConfiguration<Models.AcessoCidadao.Usuario>
    {
        public void Configure(EntityTypeBuilder<Models.AcessoCidadao.Usuario> builder)
        {
            var convGuid = Conversores.ConversorGuid();

            builder
               .ToTable("CC_AC_Usuario");

            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.AcessoCidadaoId)
                .HasColumnType("char(36)")
                .HasConversion(convGuid);

            builder
                .HasOne(a => a.AccessToken)
                .WithOne()
                .HasForeignKey<Token>("UsuarioId");

            builder
                .HasMany(a => a.Papeis)
                .WithOne(a => a.Usuario)
                .HasForeignKey("UsuarioId");

            builder
               .HasMany(a => a.UsuariosOB)
               .WithOne(a => a.UsuarioAC)
               .HasForeignKey("UsuarioACID");
        }
    }
}