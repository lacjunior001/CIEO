using CIEO.Models.EDocs.Documento;

namespace CIEO.Extension
{
    public static class DocumentoTipoExtension
    {
        /// <summary>
        /// Classificação do documento do E-Docs, Nato-Digital ou Digitalizado.
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static DocumentoTipoEnum DocumentoTipoToEnum(this short valor)
        {
            return valor switch
            {
                1 => DocumentoTipoEnum.NatoDigital,
                2 => DocumentoTipoEnum.Digitalizado,
                _ => DocumentoTipoEnum.Erro,
            };
        }
    }
}
