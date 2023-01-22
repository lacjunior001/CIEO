using CIEO.Models;
using CIEO.Models.EDocs;

namespace CIEO.Extension
{
    public static class TipoRestricaoExtensions
    {
        public static RestricaoTipoEnum RestricaoTipoToEnum(this string texto)
        {
            texto = texto.ToLower();
            texto = char.ToUpper(texto[0]) + texto.Substring(1);

            return texto switch
            {
                "Público" => RestricaoTipoEnum.Publico,
                "Publico" => RestricaoTipoEnum.Publico,
                "Organizacional" => RestricaoTipoEnum.Organizacional,
                "Sigiloso" => RestricaoTipoEnum.Sigiloso,
                "Classificado" => RestricaoTipoEnum.Classificado,
                _ => RestricaoTipoEnum.Erro,
            };
        }

        public static RestricaoTipoEnum RestricaoTipoToEnum(this short valor)
        {
            return valor switch
            {
                1 => RestricaoTipoEnum.Publico,
                2 => RestricaoTipoEnum.Organizacional,
                3 => RestricaoTipoEnum.Sigiloso,
                4 => RestricaoTipoEnum.Classificado,
                _ => RestricaoTipoEnum.Erro,
            };
        }
    }
}