using CIEO.Models.EDocs.Processo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CIEO.DB.EDocsConfig
{
    internal class InteressadoConfig : IEntityTypeConfiguration<Models.EDocs.Processo.Interessado>
    {
        public void Configure(EntityTypeBuilder<Models.EDocs.Processo.Interessado> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convAgente = Conversores.ConversorAgenteTipo();

            builder.ToTable("CC_EDocs.Interessado");

            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.Id)
                .HasConversion(convGuid);

            builder
                .Property(a => a.InteressadoTipo)
                .HasColumnName("AgenteTipoId")
                .HasConversion(convAgente);

            builder
                .HasOne(a => a.Processo)
                .WithMany(a => a.Interessados)
                .HasForeignKey("OnBaseGUID");
        }
    }
}