using CIEO.Models.AcessoCidadao;
using CIEO.Servicos;
using System.Runtime.CompilerServices;

namespace CIEO.Models.EDocs.Processo
{
    /// <summary>
    /// Relaciona os interessados do Processo com Pessoa ou Local.
    /// </summary>
    public class Interessado
    {
        /// <summary>
        /// Identifica o relacionamento AgenteInteressado X Processo
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Só existe um interessado se existir um processo.
        /// </summary>
        public Processo? Processo { get; set; }

        /// <summary>
        /// Classifica o interessado no processo.
        /// </summary>
        public AgenteTipoEnum? InteressadoTipo { get; set; }

        /// <summary>
        /// Id do Agente (Em alguma tabela do OnBase)
        /// </summary>
        public string? AgenteId { get; set; }

        /// <summary>
        /// Caso o interessado seja um cidadão, todo cidadão é uma pessoa.
        /// Caso o interessado seja um papel, todo papel pertence a uma pessoa (Servidor).
        /// Caso o interessado seja uma PJ.
        /// Caso o interessado seja uma Pessoa não Identificada.
        /// </summary>
        //public Pessoa Pessoa { get; set; }

        /// <summary>
        /// Caso a pessoa interessada seja um servidor, esta pode ser identificada pela combinação papel + pessoa.
        /// Talvez seja relevante para os casos de PAD.
        /// </summary>
        //public Papel Papel { get; set; }

        /// <summary>
        /// O interessado pode ser um "Local", Governo do Estado, Detran, Etc.
        /// </summary>
        //public Local Local { get; set; }

        /// <summary>
        /// temporario
        /// </summary>
        public string? Nome { get; set; }

        /// <summary>
        /// temporario
        /// </summary>
        public string? CPFCNPJ { get; set; }

        /// <summary>
        /// temporario
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// temporario
        /// </summary>
        public string? Descricao { get; set; }

        internal static List<Interessado>? MontarInteressados(Processo processo, Hyland.Unity.WorkView.Object fornecedor, Local local)
        {
            var list = new List<Interessado>();
            var interessadoFornecedor = new Interessado();
            interessadoFornecedor.Id = Guid.NewGuid();
            interessadoFornecedor.Processo = processo;

            foreach (var item in fornecedor.AttributeValues)
            {
                switch (item.Name)
                {
                    case "TipoPessoa":
                        if (item.HasValue)
                        {
                            if ("PESSOA FÍSICA".Equals(item.AlphanumericValue))
                            {
                                interessadoFornecedor.InteressadoTipo = AgenteTipoEnum.Cidadao;
                            }
                            else
                            {
                                interessadoFornecedor.InteressadoTipo = AgenteTipoEnum.PessoaJuridica;
                            }
                        }
                        break;

                    case "CPFOuCNPJ":
                        if (item.HasValue)
                        {
                            interessadoFornecedor.CPFCNPJ = item.AlphanumericValue;
                        }
                        break;

                    case "NomeOuRazaoSocial":
                        if (item.HasValue)
                        {
                            interessadoFornecedor.Nome = item.AlphanumericValue;
                        }
                        break;

                    case "CPF":
                        if (item.HasValue)
                        {
                            interessadoFornecedor.CPFCNPJ = item.AlphanumericValue;
                        }
                        break;

                    case "CNPJ":
                        if (item.HasValue)
                        {
                            interessadoFornecedor.CPFCNPJ = item.AlphanumericValue;
                        }
                        break;

                    case "EMail":
                        if (item.HasValue)
                        {
                            interessadoFornecedor.Email = item.AlphanumericValue;
                        }
                        break;

                    default:
                        break;
                }
            }


            list.Add(interessadoFornecedor);
            var interessadoCNHSocial = new Interessado();
            interessadoCNHSocial.Id = Guid.NewGuid();
            interessadoCNHSocial.Processo = processo;
            interessadoCNHSocial.InteressadoTipo = AgenteTipoEnum.Grupo;
            interessadoCNHSocial.AgenteId = local.Id.ToString();
            list.Add(interessadoCNHSocial);

            return list;
        }

        /// <summary>
        /// Retorna a GUID do interessado Cidadão.
        /// </summary>
        /// <returns></returns>
        internal Guid RecuperarGUIDCidadao()
        {
            if (this.InteressadoTipo != AgenteTipoEnum.Cidadao)
            {
                throw new Exception("Este Interessado não é um Cidadão.");
            }

            if (string.IsNullOrWhiteSpace(this.AgenteId))
            {
                if (string.IsNullOrWhiteSpace(this.CPFCNPJ))
                {
                    throw new Exception("Este interessado Pessoa Física está sem Id e sem CPF.");
                }
                else
                {
                    var guidRec = Models.AcessoCidadao.Aplicacao.RecuperarIdCidadao(CPFCNPJ);

                    this.AgenteId = guidRec.ToString();

                    return guidRec;
                }
            }
            else
            {
                if (Guid.TryParse(this.AgenteId, out var guid))
                {
                    return guid;
                }
                else
                {
                    var guidRec = Models.AcessoCidadao.Aplicacao.RecuperarIdCidadao(this.AgenteId);
                    this.CPFCNPJ = this.AgenteId;
                    this.AgenteId = guidRec.ToString();
                    return guidRec;
                }
            }
        }
    }
}
