using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace CIEO.Models.EDocs.DTO
{
    public class Classe
    {
        /// <summary>
        /// Id da Classe no E-Docs.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Código da Classe no E-Docs.
        /// </summary>
        public string? Codigo { get; set; }

        /// <summary>
        /// Nome da Classe no E-Docs.
        /// </summary>
        public string? Nome { get; set; }

        public string? Observacao { get; set; }

        /// <summary>
        /// Caso deixe de vir nas atualizações semanais de informação deverá virar false.
        /// </summary>
        public bool? Ativo { get; set; }
    }
}
