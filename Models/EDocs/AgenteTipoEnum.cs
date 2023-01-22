namespace CIEO.Models.EDocs
{
    /// <summary>
    /// Parâmetro do E-Docs, normalmente identifica uma pessoa.<br/>
    /// 0 - 99 = E-Docs <br/>
    /// 100 + = CIEO/OnBase
    /// </summary>
    public enum AgenteTipoEnum : short
    {
        Erro = 0,
        Cidadao = 1,
        Papel = 2,
        Grupo = 3,
        Unidade = 4,
        Organizacao = 5,
        Sistema = 6,
        NaoIdentificado = 99,
        PessoaJuridica = 100
    }
}
