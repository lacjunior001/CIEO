/*
using OBGerConn.Models;
using System.Drawing;

namespace OBGerConn.Extension
{
    public static class TipoContatoExtension
    {
        public static long? TipoContatoToLong(this TipoContato? valor)
        {
            if (valor == null || !(valor.Value == TipoContato.Telefone || valor.Value == TipoContato.EMail))
            {
                return null;
            }
            else
            {
                return (long)valor.Value;
            }
        }

        public static decimal? TipoContatoToDecimal(this TipoContato? valor)
        {
            if (valor == null || !(valor.Value == TipoContato.Telefone || valor.Value == TipoContato.EMail))
            {
                return null;
            }
            else
            {
                return Convert.ToDecimal((long)valor.Value);
            }
        }

        public static TipoContato? TipoContatoToEnum(this long? valor)
        {
            if (valor == null || !(valor.Value == (long)TipoContato.Telefone || valor.Value == (long)TipoContato.EMail))
            {
                return null;
            }
            else
            {
                return (TipoContato)valor.Value;
            }
        }

        public static TipoContato? TipoContatoToEnum(this decimal? valor)
        {
            if (valor == null)
            {
                return null;
            }
            else
            {
                return TipoContatoToEnum(Convert.ToInt64(valor.Value));
            }
        }
    }
}
*/