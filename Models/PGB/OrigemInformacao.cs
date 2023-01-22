using CIEO.Servicos;

namespace CIEO.Models.PGB
{
    public class OrigemInformacao
    {
        public long? Id { get; set; }
        public string? Valor { get; set; }
        public bool? Oculto { get; set; }

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
                    throw new Exception("Sem o valor da Origem da Informação não é possível recuperar a Id.");
                }

                var fq = wvApp.Filters.Find("DB - PGBOrigemInformacao").CreateFilterQuery();
                fq.AddConstraint("Valor", Hyland.Unity.WorkView.Operator.Equal, this.Valor);
                var resultado = fq.Execute(1);

                if (resultado != null && resultado.Count > 0)
                {
                    foreach (var item in resultado)
                    {
                        this.Id = item.ObjectID;
                    }
                }
                else
                {
                    throw new Exception("Origem da Informação Informada não foi encontrada.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("OrigemInformacao. PegarId. ", e);
            }
        }
    }
}