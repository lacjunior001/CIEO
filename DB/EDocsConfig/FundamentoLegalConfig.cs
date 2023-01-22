using CIEO.Models.EDocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CIEO.DB.EDocsConfig
{
    internal class FundamentoLegalConfig : IEntityTypeConfiguration<Models.EDocs.FundamentoLegal>
    {
        public void Configure(EntityTypeBuilder<Models.EDocs.FundamentoLegal> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convTipoRestricao = Conversores.ConversorRestricaoTipo();
            var convTipoAgente = Conversores.ConversorAgenteTipo();

            builder
               .ToTable("CC_EDOCS.FundamentoLegal");

            builder
                .HasKey(a => a.Id);

            builder
              .Property(a => a.Id)
              .HasColumnType("char")
              .HasConversion(convGuid);

            builder
                .Property(a => a.NivelAcesso)
                .HasColumnName("TipoRestricaoId")
                .HasConversion(convTipoRestricao);

            builder
                .Property(a => a.Agente)
                .HasColumnName("TipoAgenteId")
                .HasConversion(convTipoAgente);

            builder
                .HasOne(a => a.Patriarca)
                .WithMany()
                .HasForeignKey("LocalId")
                .IsRequired(false);
        }
    }
}