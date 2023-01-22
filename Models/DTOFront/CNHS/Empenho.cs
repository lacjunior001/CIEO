namespace CIEO.Models.DTOFront.CNHS
{
    public class Empenho : Processo
    {
        public long? ObjectId { get; set; }
        public int? AnoExercicio { get; set; }
        public DateTime? DataSolicitacao { get; set; }
        public double? SaldoEmpenho { get; set; }
        public double? ValorPagoMedido { get; set; }
        public double? ValorEmpenhado { get; set; }
        public string? CodigoNotaEmpenho { get; set; }


        /// <summary>
        /// Transforma obj do WV em um DTO do front (Processo).
        /// </summary>
        /// <param name="empenhoObj"></param>
        /// <returns></returns>
        internal static Models.DTOFront.CNHS.Empenho Gerar(Hyland.Unity.WorkView.Object empenhoObj)
        {
            var empenhoDTO = new Empenho();

            empenhoDTO.ObjectId = empenhoObj.ID;

            for (int i = 0; i < empenhoObj.AttributeValues.Count; i++)
            {
                switch (empenhoObj.AttributeValues[i].Name)
                {
                    case "OnBaseID":
                        empenhoDTO.OnBaseID =
                            empenhoObj.AttributeValues[i].HasValue ?
                                empenhoObj.AttributeValues[i].AlphanumericValue
                                : null;
                        break;

                    case "AnoExercicio":
                        empenhoDTO.AnoExercicio =
                            empenhoObj.AttributeValues[i].HasValue ?
                                Convert.ToInt32(empenhoObj.AttributeValues[i].AlphanumericValue)
                                : null;
                        break;

                    case "DataSolicitacao":
                        empenhoDTO.DataSolicitacao =
                            empenhoObj.AttributeValues[i].HasValue ?
                                empenhoObj.AttributeValues[i].DateValue
                                : null;
                        break;

                    case "SaldoEmpenho":
                        empenhoDTO.SaldoEmpenho =
                            empenhoObj.AttributeValues[i].HasValue ?
                                (double)empenhoObj.AttributeValues[i].CurrencyValue
                                : null;
                        break;

                    case "ValorPagoMedido":
                        empenhoDTO.ValorPagoMedido =
                            empenhoObj.AttributeValues[i].HasValue ?
                                (double)empenhoObj.AttributeValues[i].CurrencyValue
                                : null;
                        break;

                    case "ValorEmpenhado":
                        empenhoDTO.ValorEmpenhado =
                            empenhoObj.AttributeValues[i].HasValue ?
                                (double)empenhoObj.AttributeValues[i].CurrencyValue
                                : null;
                        break;

                    case "CodigoNotaEmpenho":
                        empenhoDTO.CodigoNotaEmpenho =
                            empenhoObj.AttributeValues[i].HasValue ?
                                empenhoObj.AttributeValues[i].AlphanumericValue
                                : null;
                        break;
                }
            }

            empenhoDTO.Documentos = Models.DTOFront.Documento.EmpenhoCNHS(empenhoObj.ID);

            return empenhoDTO;
        }
    }
}
