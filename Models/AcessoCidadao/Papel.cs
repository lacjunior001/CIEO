using CIEO.Models.AcessoCidadao.DTO;
using Hyland.Public;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace CIEO.Models.AcessoCidadao
{
    public class Papel
    {
        public long? Id { get; set; }
        public Guid? AcessoCidadaoId { get; set; }
        public Usuario? Usuario { get; set; }
        public Local? Lotacao { get; set; }
        public bool? Ativo { get; set; }
        public bool? Preferencial { get; set; }
        public string? Nome { get; set; }
        public string? Tipo { get; set; }

        public Papel()
        {
        }

        /// <summary>
        /// Instancia um papel a partir do DTO Recebido do Acesso Cidadão.
        /// </summary>
        /// <param name="papelDto"></param>
        internal Papel(Models.AcessoCidadao.DTO.Papel papelDto)
        {
            this.AcessoCidadaoId = papelDto.Guid;
            this.Lotacao = new Local(papelDto.LotacaoGuid.Value);
            this.Ativo = true;
            this.Preferencial = papelDto.Prioritario;
            this.Nome = papelDto.Nome;
            this.Tipo = papelDto.Tipo;
        }
    }
}
