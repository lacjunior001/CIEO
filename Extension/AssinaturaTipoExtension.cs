using CIEO.Models.EDocs.Documento;

namespace CIEO.Extension
{
    public static class AssinaturaTipoExtension
    {
        public static AssinaturaTipoEnum AssinaturaTipoToEnum(this short valor)
        {
            return valor switch
            {
                1 => AssinaturaTipoEnum.SemAssinatura,
                2 => AssinaturaTipoEnum.AutoAssinado,
                3 => AssinaturaTipoEnum.ICPBrasil,
                4 => AssinaturaTipoEnum.Multiplas,
                _ => AssinaturaTipoEnum.Erro,
            };
        }
    }
}
