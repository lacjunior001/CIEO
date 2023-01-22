using CIEO.Servicos;

namespace CIEO.Models.DTOFront
{
    public class Documento
    {
        /// <summary>
        /// Id do Documento no OnBase.
        /// </summary>
        public long? DocumentHandle { get; set; }

        /// <summary>
        /// Ordem para exibição do documento.
        /// </summary>
        public short OrdemExibicao { get; set; }

        /// <summary>
        /// Categoria do Anexo no OnBase.
        /// Nome do Documento.
        /// </summary>
        public string? CategoriaAenxo { get; set; }

        /// <summary>
        /// Código de registo do documento no E-Docs.
        /// </summary>
        public string? RegistroEDocs { get; set; }

        /// <summary>
        /// Recupera uma lista de documentos de um empenho do CNHS.
        /// </summary>
        /// <param name="objID"></param>
        /// <returns></returns>
        public static List<Models.DTOFront.Documento> EmpenhoCNHS(long objID)
        {
            var lista = new List<Models.DTOFront.Documento>(10);

            var docQuery = OnBase.App.Core.CreateDocumentQuery();
            var dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Termo de Adesão Assinado");
            docQuery.AddDocumentType(dt);
            dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Certidão Empenho");
            docQuery.AddDocumentType(dt);
            dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Solicitação Empenho");
            docQuery.AddDocumentType(dt);
            dt = OnBase.App.Core.DocumentTypes.Find("CNHS - Anexo Registro de Pendências");
            docQuery.AddDocumentType(dt);
            docQuery.AddKeyword("ID Registro Workview", objID);
            long contador = docQuery.ExecuteCount();
            if (contador > 0)
            {
                var docList = docQuery.Execute(contador);

                foreach (var documentoAnexo in docList)
                {
                    var documentoDTO = new Models.DTOFront.Documento();

                    documentoDTO.DocumentHandle = documentoAnexo.ID;

                    for (int i = 0; i < documentoAnexo.KeywordRecords.Count; i++)
                    {
                        for (int j = 0; j < documentoAnexo.KeywordRecords[i].Keywords.Count; j++)
                        {
                            switch (documentoAnexo.KeywordRecords[i].Keywords[j].KeywordType.Name)
                            {
                                case "EDocs Doc: Registro":
                                    documentoDTO.RegistroEDocs = documentoAnexo.KeywordRecords[i].Keywords[j].AlphaNumericValue;
                                    break;
                                case "Categoria do Anexo CNHS":
                                    documentoDTO.CategoriaAenxo = documentoAnexo.KeywordRecords[i].Keywords[j].AlphaNumericValue;
                                    break;
                            }

                            if (documentoAnexo.DocumentType.Name.Equals("CNHS - Anexo Termo de Adesão Assinado"))
                            {
                                documentoDTO.CategoriaAenxo = "TERMO DE ADESÃO";
                            }

                            switch (documentoDTO.CategoriaAenxo)
                            {
                                case "SOLICITAÇÃO DE EMPENHO":
                                    documentoDTO.OrdemExibicao = 1;
                                    break;
                                case "TERMO DE ADESÃO":
                                    documentoDTO.OrdemExibicao = 2;
                                    break;
                                case "CERTIDÃO NEGATIVA FEDERAL":
                                    documentoDTO.OrdemExibicao = 3;
                                    break;
                                case "CERTIDÃO NEGATIVA ESTADUAL":
                                    documentoDTO.OrdemExibicao = 4;
                                    break;
                                case "CERTIDÃO NEGATIVA MUNICIPAL":
                                    documentoDTO.OrdemExibicao = 5;
                                    break;
                                case "CERTIDÃO NEGATIVA FGTS":
                                    documentoDTO.OrdemExibicao = 6;
                                    break;
                                case "TRABALHISTA":
                                    documentoDTO.OrdemExibicao = 7;
                                    break;
                                case "PENDÊNCIA":
                                    documentoDTO.OrdemExibicao = 8;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    lista.Add(documentoDTO);
                }
            }

            lista.Sort((x, y) => x.OrdemExibicao.CompareTo(y.OrdemExibicao));

            return lista;
        }

    }
}
