namespace CIEO.Models.EDocs.Documento
{
    /// <summary>
    /// Tipo de assinatura que um documento vai receber no E-Docs.<br/>
    /// Cata tipo tem seu próprio estilo de processamento.
    /// </summary>
    public enum AssinaturaTipoEnum
    {
        Erro = 0,
        SemAssinatura = 1,
        AutoAssinado = 2,
        ICPBrasil = 3,
        Multiplas = 4
    }
}
