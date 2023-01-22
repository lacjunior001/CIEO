namespace CIEO.Models.EDocs.DTO
{
    /// <summary>
    /// Dados para classificação da informação.
    /// </summary>
    public class ClassificacaoInformacao
    {
        /// <summary>
        /// Prazo em anos para a classificação da informação.
        /// </summary>
        public long PrazoAnos { get; set; }

        /// <summary>
        /// Prazo em meses para a classificação da informação.
        /// </summary>
        public long PrazoMeses { get; set; }

        /// <summary>
        /// Prazo em dias para a classificação da informação.
        /// </summary>
        public long PrazoDias { get; set; }

        /// <summary>
        /// Justificativa para a classificação da informação.
        /// </summary>
        public string Justificativa { get; set; }

        /// <summary>
        /// Id do papel que será solicitado aprovação para a classificação da informação.
        /// </summary>
        public string IdPapelAprovador { get; set; }
    }
}