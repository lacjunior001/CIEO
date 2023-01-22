using CIEO.Models.EDocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class EventoConfig : IEntityTypeConfiguration<Models.EDocs.Evento>
    {
        public void Configure(EntityTypeBuilder<Evento> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convSituacaoEvento = Conversores.ConversorSituacaoEvento();
            
            builder
               .ToTable("CC_EDocs.Evento");

            builder
                .HasKey(a => a.Id);

            builder
               .Property(a => a.Id)
               .HasConversion(convGuid);

            builder
               .Property(a => a.IdEDocs)
               .HasConversion(convGuid);

            //builder
            //   .Property<long?>("DocumentHandle");

            //builder
            //   .Property<Guid?>("AtoId")
            //   .HasConversion(convGuid);

            builder
                .HasOne(a => a.DocumentoRelacionado)
                .WithOne()
                .HasForeignKey<Evento>("DocumentHandle")
                .IsRequired(false);

            builder
                .HasOne(a => a.AtoRelacionado)
                .WithOne()
                .HasForeignKey<Evento>("AtoId")
                .IsRequired(false);

            builder
                .Property(a => a.Situacao)
                .HasColumnName("SituacaoEvento")
                .HasConversion(convSituacaoEvento);
        }
    }
}