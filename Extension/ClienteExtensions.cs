using CIEO.Models;

namespace CIEO.Extension
{
    public static class ClienteExtensions
    {
        private static readonly Dictionary<string, Cliente> mapa = new Dictionary<string, Cliente>
        {
            { "DETRAN", Cliente.Detran },
            { "SESA", Cliente.Sesa },
            { "SEJUS", Cliente.Sejus },
            { "SEG", Cliente.Seg },
        };

        public static string ClienteToString(this Cliente valor)
        {
            return mapa.First(c => c.Value == valor).Key;
        }

        public static Cliente ClienteToEnum(this string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                throw new Exception("ConvertAmbienteString. Valor nulo.");
            }

            texto = texto.ToUpper();

            return mapa.First(c => c.Key == texto).Value;
        }
    }
}
