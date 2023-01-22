namespace CIEO.Models.EDocs
{
    public enum SituacaoEnum : short
    {
        /// <summary>
        /// Algum erro ocorreu durante o processamento (Não Catalogado).
        /// </summary>
        Erro = 0,

        /// <summary>
        /// Indica que está esperando ser enviado ao E-Doc.
        /// </summary>
        AguardandoEnvioEDocs = 1,

        /// <summary>
        /// Indica que o ato ainda não foi enviado pois está aguardando o envio dos documentos ao E-Docs.
        /// </summary>
        AguardandoEnvioDosDocumentos = 2,

        /// <summary>
        /// Indica que ocorreu algum erro ao enviar um documento ou ato para o E-Docs.
        /// </summary>
        ErroAoEnviarParaEDocs = 3,

        /// <summary>
        /// Indica que está esperando resposta de um evento criado.
        /// </summary>
        AguardandoRespostaEDocs = 4,

        /// <summary>
        /// Indica que ocorreu algum erro ao tentar consultar o Evento do Ato ou Documento.
        /// </summary>
        ErroConsultarEvento = 5,

        /// <summary>
        /// Indica que o Documento ou Ato foi enviado com sucesso para o E-Docs.
        /// </summary>
        EnviadoParaEDocs = 6,

        /// <summary>
        /// Indica que o ato ou documento está esperando o processo de download para o OnBase.
        /// </summary>
        AguardandoBaixarDoEDocs = 7,

        /// <summary>
        /// Indica que ocorreu um erro ao tentar baixar dados do E-Docs.
        /// </summary>
        ErroAoBaixarDadosDoEDocs = 8,

        /// <summary>
        /// Indica que os dados foram baixados do E-Docs com sucesso.
        /// </summary>
        DadosBaixadosDoEDocs = 9
    }
}
