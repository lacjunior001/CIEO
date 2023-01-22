namespace CIEO.Models.EDocs.Processo.DTO
{
    public class DespacharEnvio : AtoBaseEnvio
    {
        /// <summary>
        /// Id do local para onde o Processo deve ser enviado.
        /// </summary>
        public Guid IdDestino { get; set; }

        /// <summary>
        /// Mensagem de despacho.
        /// </summary>
        public string Mensagem { get; set; }
    }
}
