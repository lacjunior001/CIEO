using CIEO.Servicos;

namespace CIEO.Models.EDocs.Processo.DTO
{
    /// <summary>
    /// Objeto usado para autuar processo no E-Docs (Interessado PJ).
    /// </summary>
    public class PessoaJuridicaInteressada
    {
        public string RazaoSocial { get; private set; }
        public string CNPJ { get; private set; }
        public string Email { get; private set; }

        internal PessoaJuridicaInteressada(Interessado? interessado)
        {
            if (interessado == null)
            {
                throw new Exception("Interessado Pessoa Jurídica não foi recebido.");
            }

            if (interessado.InteressadoTipo == null)
            {
                throw new Exception("Não foi possível identificar o tipo do interessado.");
            }

            if (interessado.InteressadoTipo != AgenteTipoEnum.PessoaJuridica)
            {
                throw new Exception("Interessado não é do tipo Pessoa Jurídica.");
            }

            if (string.IsNullOrWhiteSpace(interessado.Nome) || interessado.Nome.Length <= 5)
            {
                throw new Exception("Nome da Pessoa Jurídica não é válido");
            }
            else
            {
                this.RazaoSocial = interessado.Nome.Trim();
            }

            if (Uteis.CNPJValido(interessado.CPFCNPJ))
            {
                this.CNPJ = interessado.CPFCNPJ.Trim();
            }
            else
            {
                throw new Exception("CNJP da Pessoa Jurídica não é válido");
            }

            if (Uteis.EmailValido(interessado.Email))
            {
                this.Email = interessado.Email.Trim();
            }
            else
            {
                throw new Exception("CNJP da Pessoa Jurídica não é válido");
            }
        }
    }
}