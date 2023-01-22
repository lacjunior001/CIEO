using CIEO.Models.AcessoCidadao;
using CIEO.Models.PGB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.ACConfig
{
    internal class UsuOBXUsuACConfig : IEntityTypeConfiguration<UsuOBXUsuAC>
    {
        public void Configure(EntityTypeBuilder<UsuOBXUsuAC> builder)
        {
            builder
                .ToTable("CC_AC_UsuOBXUsuAC");

            builder
                .HasKey(a => a.Id);

            builder
                .HasOne(a => a.UsuarioAC)
                .WithMany(a => a.UsuariosOB)
                .HasForeignKey("UsuarioACID");

            builder
                .HasOne(a => a.UsuarioOB)
                .WithOne(a => a.UsuarioAC)
                .HasForeignKey<UsuOBXUsuAC>("UsuarioOBID");
        }
    }
}