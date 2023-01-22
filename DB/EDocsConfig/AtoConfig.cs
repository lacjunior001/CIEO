using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class AtoConfig : IEntityTypeConfiguration<Models.EDocs.Processo.Ato>
    {
        public void Configure(EntityTypeBuilder<Models.EDocs.Processo.Ato> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convSituacao = Conversores.ConversorSituacao();

            builder.ToTable("CC_EDocs.Ato");

            builder
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Models.EDocs.Processo.Autuacao>("Autuacao");

            /*builder
                .Property<string>("Discriminator")
                .HasColumnName("Discriminator");*/

            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.Id)
                .HasConversion(convGuid);

            builder
                .HasOne(a => a.Processo)
                .WithMany(a => a.Atos)
                .HasForeignKey("OnBaseGUID");

            builder
                .Property(a => a.EDocsGUID)
                .HasConversion(convGuid);

            builder
                .HasOne(a => a.PapelPraticante)
                .WithMany()
                .HasForeignKey("PapelId");
            
            builder
                .Property(a => a.Situacao)
                .HasColumnName("SituacaoId")
                .HasConversion(convSituacao);
        }
    }
}