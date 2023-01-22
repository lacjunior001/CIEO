using CIEO.Models;
using CIEO.Models.EDocs.Documento;

namespace CIEO.Extension
{
    public static class ValorLegalExtensions
    {
        public static ValorLegalEnum ValorLegalToEnum(this short valor)
        {
            return valor switch
            {
                1 => ValorLegalEnum.Original,
                2 => ValorLegalEnum.CopiaAutenticadaCartorio,
                3 => ValorLegalEnum.CopiaAutenticadaAdministrativamente,
                4 => ValorLegalEnum.CopiaSimples,
                _ => ValorLegalEnum.Erro,
            };
        }
    }
}
