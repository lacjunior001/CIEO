using Newtonsoft.Json;

namespace CIEO.Models.EDocs.DTO
{
    /// <summary>
    /// Dados para restrição de acesso de documento.
    /// </summary>
    public class RestricaoAcesso
    {
        /// <summary>
        /// Transparência Ativa - se positivo será público, se negativo sem fundamentos legais e classificação da informação será organizacional.
        /// </summary>
        public bool TransparenciaAtiva { get; set; }

        /// <summary>
        /// Lista de ids dos fundamentos legais aplicáveis.
        /// </summary>
        public List<Guid> IdsFundamentosLegais { get; set; }

        /// <summary>
        /// Dados da Classificação da Informação.
        /// </summary>
        public ClassificacaoInformacao ClassificacaoInformacao { get; set; }

        public RestricaoAcesso(RestricaoTipoEnum restricaoTipoEnum, List<Guid>? idsFundamentos, PrazosSigilo? prazos)
        {
            switch (restricaoTipoEnum)
            {
                case RestricaoTipoEnum.Publico:
                    TransparenciaAtiva = true;
                    IdsFundamentosLegais = null;
                    ClassificacaoInformacao = null;
                    break;
                case RestricaoTipoEnum.Organizacional:
                    TransparenciaAtiva = false;
                    IdsFundamentosLegais = null;
                    ClassificacaoInformacao = null;
                    break;
                case RestricaoTipoEnum.Sigiloso:
                    TransparenciaAtiva = false;
                    IdsFundamentosLegais = idsFundamentos;
                    ClassificacaoInformacao = null;
                    break;
                default:
                    throw new Exception("Tipo de Restrição não suportado");
            }
        }
    }
}