using CIEO.Models.EDocs;
using CIEO.Models.EDocs.Documento;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CIEO.DB
{
    public static class Conversores
    {
        internal static ValueConverter<Guid?, string?> ConversorGuid()
        {
            return new ValueConverter<Guid?, string?>(
              v => v == null ? null : v.ToString(),
              v => string.IsNullOrWhiteSpace(v) ? null : new Guid(v));
        }

        internal static ValueConverter<short?, byte?> ConversorByteShort()
        {
            return new ValueConverter<short?, byte?>(
              v => v == null ? null : Convert.ToByte(v),
              v => v == null ? null : Convert.ToInt16(v));
        }

        internal static ValueConverter<bool?, long?> ConversorBoolToLong()
        {
            return new ValueConverter<bool?, long?>(
              v => v == null ? null : BoolToLong(v.Value),
              v => v == null ? null : LongToBool(v.Value));
        }

        internal static ValueConverter<long?, bool?> ConversorLongToBool()
        {
            return new ValueConverter<long?, bool?>(
              v => v == null ? null : LongToBool(v.Value),
              v => v == null ? null : BoolToLong(v.Value));
        }

        private static bool LongToBool(long val)
        {
            if (val == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static long BoolToLong(bool val)
        {
            if (val)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        internal static ValueConverter<RestricaoTipoEnum?, byte?> ConversorRestricaoTipo()
        {
            return new ValueConverter<RestricaoTipoEnum?, byte?>(
              v => v == null ? null : (byte)v,
              v => v == null ? RestricaoTipoEnum.Erro : (RestricaoTipoEnum)v);
        }

        internal static ValueConverter<AgenteTipoEnum?, byte?> ConversorAgenteTipo()
        {
            return new ValueConverter<AgenteTipoEnum?, byte?>(
              v => v == null ? null : (byte)v,
              v => v == null ? AgenteTipoEnum.Erro : (AgenteTipoEnum)v);
        }

        internal static ValueConverter<SituacaoEnum?, byte?> ConversorSituacao()
        {
            return new ValueConverter<SituacaoEnum?, byte?>(
              v => v == null ? null : (byte)v,
              v => v == null ? SituacaoEnum.Erro : (SituacaoEnum)v);
        }

        internal static ValueConverter<AssinaturaTipoEnum?, byte?> ConversorAssinaturaTipo()
        {
            return new ValueConverter<AssinaturaTipoEnum?, byte?>(
              v => v == null ? null : (byte)v,
              v => v == null ? AssinaturaTipoEnum.Erro : (AssinaturaTipoEnum)v);
        }

        internal static ValueConverter<DocumentoTipoEnum?, byte?> ConversorDocumentoTipo()
        {
            return new ValueConverter<DocumentoTipoEnum?, byte?>(
              v => v == null ? null : (byte)v,
              v => v == null ? DocumentoTipoEnum.Erro : (DocumentoTipoEnum)v);
        }

        internal static ValueConverter<ValorLegalEnum?, byte?> ConversorValorLegal()
        {
            return new ValueConverter<ValorLegalEnum?, byte?>(
              v => v == null ? null : (byte)v,
              v => v == null ? ValorLegalEnum.Erro : (ValorLegalEnum)v);
        }

        internal static ValueConverter<EventoSituacaoEnum?, byte?> ConversorSituacaoEvento()
        {
            return new ValueConverter<EventoSituacaoEnum?, byte?>(
             v => v == null ? null : (byte)v,
             v => v == null ? EventoSituacaoEnum.Erro : (EventoSituacaoEnum)v);
        }
    }
}
