using CIEO.Models.EDocs.Processo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class AutuacoesConfig : IEntityTypeConfiguration<Models.EDocs.Processo.Autuacao>
    {
        public void Configure(EntityTypeBuilder<Autuacao> builder)
        {
            var convGUID = Conversores.ConversorGuid();

            builder
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Autuacao>("Autuacao");

            builder
                .HasOne(a => a.LocalAutuacao)
                .WithMany()
                .HasForeignKey("LocalId");

            builder
                .HasMany(a => a.Documentos)
                .WithOne(a => (Autuacao)a.Ato)
                .HasForeignKey("AtoId");
            
            //builder
            //    .UsePropertyAccessMode(PropertyAccessMode.);

            /*
            var convGuid = Conversores.ConversorGuid();
            var convSituacao = Conversores.ConversorSituacao();

            builder.ToTable("CC_EDocs.Ato");

            builder
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Autuacao>("Autuacao");

            builder
               .HasKey(a => a.Id);

            builder
                .Property(a => a.Id)
                .HasConversion(convGuid);

            builder
                .HasOne(a => a.Processo)
                .WithMany()
                .HasForeignKey("OnBaseGUID");

            builder
                .Property(a => a.EDocsGUID)
                .HasConversion(convGuid);

            builder
                .HasOne(a => a.PapelPraticante)
                .WithMany()
                .HasForeignKey("PapelId");

            builder
                .HasOne(a => a.LocaPraticado)
                .WithMany()
                .HasForeignKey("LocalId");

            builder
                .Property(a => a.Situacao)
                .HasColumnName("SituacaoId")
                .HasConversion(convSituacao);
            */
            //builder.Ignore(a => a.Documentos);
        }
    }
}