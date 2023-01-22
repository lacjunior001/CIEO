using CIEO.Models.AcessoCidadao;
using CIEO.Models.PGB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.PGBConfig
{
    internal class UsuarioOBConfig : IEntityTypeConfiguration<UsuarioOB>
    {
        public void Configure(EntityTypeBuilder<UsuarioOB> builder)
        {
            var convLong = Conversores.ConversorBoolToLong();

            builder
              .ToTable("useraccount");

            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.Id)
                .HasColumnName("usernum");

            builder
                .Property(a => a.UserName)
                .HasColumnName("username");

            builder
                .Property(a => a.Disable)
                .HasColumnName("disablelogin")
                .HasConversion(convLong);

            builder
                .HasOne(a => a.UsuarioAC)
                .WithOne(a => a.UsuarioOB)
                .HasForeignKey<UsuOBXUsuAC>("UsuarioOBID")
                .IsRequired(false);

            builder
                .HasMany(a => a.UserGroups)
                .WithOne(a => a.UsuariosOB)
                .HasForeignKey("usernum");
        }
    }
}