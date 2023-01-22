namespace CIEO.Models.EDocs
{
    /// <summary>
    /// Tipos possíveis de restrição de acesso dos documentos e atos no E-Docs.
    /// </summary>
    public enum RestricaoTipoEnum : short
    {
        Erro = 0,
        Publico = 1,
        Organizacional = 2,
        Sigiloso = 3,
        Classificado = 4
    }
}
