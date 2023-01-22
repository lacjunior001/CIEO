using CIEO.Models;

namespace CIEO.Extension
{
    public static class AmbienteExtensions
    {
        private static readonly Dictionary<string, Ambiente> mapa = new Dictionary<string, Ambiente>
        {
            { "Desenvolvedor", Ambiente.Desenvolvedor },
            { "Desenvolvimento", Ambiente.Desenvolvimento },
            { "Homologação", Ambiente.Homologacao },
            { "Producao", Ambiente.Producao },
            { "Produção", Ambiente.Producao },
            { "Externo", Ambiente.Externo },
            { "Homo", Ambiente.Homologacao },
            { "Homolog", Ambiente.Homologacao },
            { "Prod", Ambiente.Producao },
            { "Ext", Ambiente.Externo }
        };

        public static string AmbienteToString(this Ambiente valor)
        {
            return mapa.First(c => c.Value == valor).Key;
        }

        public static Ambiente AmbienteToEnum(this string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                throw new Exception("ConvertAmbienteString. Valor nulo.");
            }

            texto = texto.ToLower();

            if ("dev".Equals(texto))
            {
                throw new Exception("ConvertAmbienteString.\"DEV\" pode ser \"Desenvolvimento\" ou \"Desenvolvedor\".Selecione outro valor.");
            }

            texto = char.ToUpper(texto[0]) + texto.Substring(1);

            return mapa.First(c => c.Key == texto).Value;
        }
    }
}
