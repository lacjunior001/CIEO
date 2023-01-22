using CIEO.Models.AcessoCidadao;
using CIEO.Models.PGB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.PGBConfig
{
    internal class PessoaFisicaConfig : IEntityTypeConfiguration<PessoaFisica>
    {
        public void Configure(EntityTypeBuilder<PessoaFisica> builder)
        {
            builder
               .ToView("rm_PGBPessoaFisicaNovo");

            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.EMailPrincipalId)
                .HasColumnName("ContatoId");

            //builder
            //    .HasOne(a => a.EMailPrincipal)
            //    .WithOne(a => a.PessoaFisica)
            //    .HasForeignKey<PessoaFisica>("ContatoId");

            builder
                .HasMany(a => a.Contatos)
                .WithOne(a => a.PessoaFisica)
                .HasForeignKey("PessoaFisicaId");
        }
    }
}