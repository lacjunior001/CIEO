using CIEO.Models.AcessoCidadao;
using CIEO.Models.PGB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.PGBConfig
{
    internal class UserGroupOBConfig : IEntityTypeConfiguration<UserGroupOB>
    {
        public void Configure(EntityTypeBuilder<UserGroupOB> builder)
        {
            var convLong = Conversores.ConversorBoolToLong();

            builder
              .ToView("CC_OB_UserAccountXUserGroup");

            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.Id)
                .HasColumnName("usergroupnum");

            builder
                .Property(a => a.Nome)
                .HasColumnName("usergroupname");

            builder
                .HasOne(a => a.UsuariosOB)
                .WithMany(a => a.UserGroups)
                .HasForeignKey("usernum");
        }
    }
}