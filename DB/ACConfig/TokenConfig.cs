using CIEO.Models.AcessoCidadao;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.ACConfig
{
    internal class TokenConfig : IEntityTypeConfiguration<Models.AcessoCidadao.Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            var convGuid = Conversores.ConversorGuid();

            builder
               .ToTable("CC_AC_UsuarioToken");

            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.AcessToken)
                .HasColumnName("Token")
                .HasColumnType("char");

            builder
                .HasOne(a => a.Usuario)
                .WithOne(a => a.AccessToken)
                .HasForeignKey<Token>("UsuarioId");
        }
    }
}