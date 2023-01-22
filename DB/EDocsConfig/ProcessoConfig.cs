using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CIEO.Models.EDocs.Processo;

namespace CIEO.DB.EDocsConfig
{
    internal class ProcessoConfig : IEntityTypeConfiguration<Models.EDocs.Processo.Processo>
    {
        public void Configure(EntityTypeBuilder<Models.EDocs.Processo.Processo> builder)
        {
            var convGuid = Conversores.ConversorGuid();
            var convRestricaoTipo = Conversores.ConversorRestricaoTipo();
            var convAgenteTipo = Conversores.ConversorAgenteTipo();
            var convSituacao = Conversores.ConversorSituacao();
            var convAssinaturaTipo = Conversores.ConversorAssinaturaTipo();
            var convDocumentoTipo = Conversores.ConversorDocumentoTipo();
            var convValorLegal = Conversores.ConversorValorLegal();

            builder
               .ToTable("CC_EDocs.Processo");

            builder
                .HasKey(a => a.OnBaseGUID);

            builder
               .Property(a => a.OnBaseGUID)
               .HasConversion(convGuid);

            builder
               .Property(a => a.EdocsGUID)
               .HasConversion(convGuid);

            builder
               .HasOne(p => p.Classe)
               .WithOne()
               .HasForeignKey<Processo>("ClasseId");

            builder
               .HasOne(p => p.Localizacao)
               .WithOne()
               .HasForeignKey<Processo>("LocalId");

            builder
                .Ignore(a => a.Situacao);

            builder
               .HasMany(p => p.Interessados)
               .WithOne(p => p.Processo)
               .HasForeignKey("OnBaseGUID");

            builder
               .HasMany(p => p.Atos)
               .WithOne(p => p.Processo)
               .HasForeignKey("OnBaseGUID");
        }
    }
}