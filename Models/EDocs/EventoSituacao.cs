namespace CIEO.Models.EDocs
{
    /// <summary>
    /// Situações dos eventos de documentos e atos no E-Docs.<br/>
    /// 0 - 29 = E-Docs.<br/>
    /// 30 + = CIEO/OnBase.
    /// </summary>
    public enum EventoSituacaoEnum : short
    {
        Erro = 0,
        Criado = 1,
        Enfileirado = 2,
        Processando = 3,
        Executado = 4,
        Concluido = 5,
        Cancelado = 9,
        AguardandoCriacao = 30,
        ErroConsulta = 31
    }
}
