using CIEO.Models.AcessoCidadao;
using CIEO.Models.PGB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.PGBConfig
{
    internal class ContatoConfig : IEntityTypeConfiguration<Contato>
    {
        public void Configure(EntityTypeBuilder<Contato> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convBool = Conversores.ConversorBoolToLong();

            builder
               .ToView("rm_PGBContatoNovo");
            
            builder
                .HasKey(a => a.Id);

            builder
                .HasOne(a => a.PessoaFisica)
                .WithMany(a => a.Contatos)
                .HasForeignKey("PessoaFisicaId");

            builder
                .Property(a => a.Ativo)
                .HasConversion(convBool);

            builder
                .HasOne(a => a.TipoContato)
                .WithMany()
                .HasForeignKey("TipoContatoId");

            builder
                .HasOne(a => a.OrigemInformacao)
                .WithMany()
                .HasForeignKey("OrigemInformacaoId");
        }
    }
}