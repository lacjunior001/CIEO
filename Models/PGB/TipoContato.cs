using CIEO.Servicos;
using Hyland.Unity.WorkView;

namespace CIEO.Models.PGB
{
    /// <summary>
    /// Tipo do Contato
    /// </summary>
    public class TipoContato
    {
        public long? Id { get; set; }
        public string? Valor { get; set; }

        /// <summary>
        /// Pega a Id no OnBase do tipo contato instanciado.<br/>
        /// Utiliza FilterQuery/OnBase Cache.
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal void PegarId(Hyland.Unity.WorkView.Application wvApp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.Valor))
                {
                    throw new Exception("Sem o valor do Tipo de Contato não é possível recuperar a Id.");
                }

                var filtro = wvApp.Filters.Find("DB - PGBTipoContato");

                var fq = wvApp.Filters.Find("DB - PGBTipoContato").CreateFilterQuery();
                fq.AddConstraint("Valor", Hyland.Unity.WorkView.Operator.Equal, this.Valor.Trim());
                var resultado = fq.Execute(1);
                if (resultado != null && resultado.Count > 0)
                {
                    foreach (var item in resultado)
                    {
                        this.Id = item.ObjectID;
                    }

                    OnBase.App.Cache.AddItem($"TIPOCONTATO.{this.Valor.ToUpper().Trim()}", this.Id);
                }
                else
                {
                    throw new Exception("Tipo de Contato que foi informado não encontrado.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("TipoContato. PegarId. ", e);
            }
        }
    }
}
