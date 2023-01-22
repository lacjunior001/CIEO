using CIEO.Models.AcessoCidadao;
using CIEO.Models.AcessoCidadao.DTO;
using CIEO.Servicos;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http.Headers;

namespace CIEO.Models.EDocs.Documento
{
    /// <summary>
    /// Encapsula todas informações pertinentes para enviar ou receber um documento do E-Docs.
    /// </summary>
    public class Documento
    {
        /// <summary>
        /// Id do Documento no OnBase/CIEO.
        /// </summary>
        public long? DocumentHandle { get; set; }

        /// <summary>
        /// Id do Documento no E-Docs (Só existe após o envio).
        /// </summary>
        public Guid? EDocsId { get; set; }

        /// <summary>
        /// Registro do Documento no E-Docs (Só existe após o envio).
        /// </summary>
        public string? Registro { get; set; }

        /// <summary>
        /// Nome que o documento deverá receber no E-Docs<br/>
        /// Via de Regra é nossa categoria do Anexo.
        /// </summary>
        public string? NomeDocumento { get; set; }

        /// <summary>
        /// Extensão do documento.<br/>
        /// Extensões que não seja PDF ainda devem ser testadas.
        /// </summary>
        public string? Extensao { get; set; }

        /// <summary>
        /// Quando o Capturador for Agente Publico (Servidor).
        /// </summary>
        public Models.AcessoCidadao.Papel? AgentePublico { get; set; }

        /// <summary>
        /// Quando o Capturador for Cidadão
        /// </summary>
        public Models.AcessoCidadao.Usuario? Cidadao { get; set; }

        /// <summary>
        /// Permite que o documento fique na pasta do capturador após a captura.
        /// </summary>
        public bool? CapturadorCredenciar { get; set; }

        /// <summary>
        /// Assinatura que o Documento irá receber.
        /// </summary>
        public AssinaturaTipoEnum? AssinaturaTipo { get; set; }

        /// <summary>
        /// Tipo do documento.
        /// </summary>
        public DocumentoTipoEnum? DocumentoTipo { get; set; }

        /// <summary>
        /// Valor do documento. Cópia/Original/Etc.
        /// </summary>
        public ValorLegalEnum? ValorLegal { get; set; }

        /// <summary>
        /// Classe do documento.
        /// </summary>
        public Classe? Classe { get; set; }

        /// <summary>
        /// Qual a restrição de acesso para o documento.
        /// </summary>
        public RestricaoTipoEnum? RestricaoTipo { get; set; }

        /// <summary>
        /// Situação da sincronização com o e-docs.
        /// </summary>
        public SituacaoEnum? Situacao { get; set; }

        /// <summary>
        /// Faz o DE->Para dos documentos anexados a um empenho para os dados necessários no E-Docs para prototocolo.
        /// </summary>
        /// <param name="documentos">Lista de Documentos do OnBase</param>
        /// <param name="usuarioCPF">CPF do usuário que está enviado o Documento.</param>
        /// <returns>Lista de Documentos para serem gravados no banco.</returns>
        /// <exception cref="Exception"></exception>
        internal static List<Models.EDocs.Documento.Documento> EmpenhoParaEnvio(Hyland.Unity.DocumentList documentos, string usuarioCPF, Classe classe)
        {
            var lista = new List<Documento>(20);
            //DocumentLock? dockLock = null;
            var ktEDocsGUID = OnBase.App.Core.KeywordTypes.Find("EDocs Doc: Id");
            try
            {
                var ktCategoria = OnBase.App.Core.KeywordTypes.Find("Categoria do Anexo CNHS");
                Hyland.Unity.KeywordRecord kr = null;
                for (int i = 0; i < documentos.Count; i++)
                {

                    //var docLock = documentos[i].LockDocument();
                    //if (docLock.Status.Equals(DocumentLockStatus.AlreadyLocked))
                    //{
                    //throw new Exception($"Está sendo visualizado por {docLock.UserHoldingLock.Name}, desde {docLock.LockTime}.");
                    //}

                    var docDB = new Models.EDocs.Documento.Documento();
                    kr = documentos[i].KeywordRecords.Find(ktCategoria);
                    if (kr == null)
                    {
                        if (documentos[i].DocumentType.Name.Equals("CNHS - Anexo Termo de Adesão Assinado"))
                        {
                            docDB.NomeDocumento = "Termo de Adesão Assinado";
                        }
                        else
                        {
                            docDB.NomeDocumento = "Documento não categorizado";
                        }
                    }
                    else
                    {
                        docDB.NomeDocumento = kr.Keywords.Find(ktCategoria).AlphaNumericValue;
                    }

                    docDB.DocumentHandle = documentos[i].ID;
                    //docDB.CapturadorCPF = usuarioCPF;
                    //docDB.CapturadorTipo = Models.EDocs.AgenteTipoEnum.Papel;
                    docDB.Classe = null;
                    docDB.CapturadorCredenciar = true;
                    docDB.Extensao = "pdf";
                    docDB.RestricaoTipo = Models.EDocs.RestricaoTipoEnum.Organizacional;
                    docDB.Situacao = Models.EDocs.SituacaoEnum.AguardandoEnvioEDocs;

                    if ("SOLICITAÇÃO DE EMPENHO".Equals(docDB.NomeDocumento))
                    {
                        /*
                        docDB.AssinaturaTipo = Models.EDocs.Documento.AssinaturaTipoEnum.AutoAssinado;
                        docDB.DocumentoTipo = Models.EDocs.Documento.DocumentoTipoEnum.NatoDigital;
                        docDB.ValorLegal = ValorLegalEnum.Original;
                        docDB.Classe = classe;*/

                        docDB.AssinaturaTipo = Models.EDocs.Documento.AssinaturaTipoEnum.SemAssinatura;
                        docDB.DocumentoTipo = Models.EDocs.Documento.DocumentoTipoEnum.Digitalizado;
                        docDB.ValorLegal = ValorLegalEnum.CopiaSimples;
                    }
                    else
                    {
                        docDB.AssinaturaTipo = Models.EDocs.Documento.AssinaturaTipoEnum.SemAssinatura;
                        docDB.DocumentoTipo = Models.EDocs.Documento.DocumentoTipoEnum.Digitalizado;
                        docDB.ValorLegal = ValorLegalEnum.CopiaSimples;
                    }

                    kr = documentos[i].KeywordRecords.Find(ktEDocsGUID);
                    if (kr == null)
                    {
                        //var kwMod = documentos[i].CreateKeywordModifier();
                        //kwMod.AddKeyword(ktEDocsGUID.ID, "Aguardando");
                        //kwMod.ApplyChanges();
                    }

                    lista.Add(docDB);
                    //docLock.Release();
                }

                return lista;
            }
            catch (Exception e)
            {
                //if (dockLock != null && dockLock.Status == DocumentLockStatus.LockObtained)
                //{
                //    dockLock.Release();
                //}

                throw new Exception("Documento.EmpenhoParaEnvio.", e);
            }
        }

        /// <summary>
        /// Faz o envio de um documento para o E-Docs. 1º/2º/3º Etapa.
        /// </summary>
        /// <param name="token">Cidadão e Servidor</param>
        /// <param name="idPapel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="Exception"></exception>
        internal Guid RegistrarEDocs(string token)
        {
            Guid? evento = null;
            try
            {
                if (this.DocumentoTipo == null)
                {
                    throw new Exception("O tipo de documento não foi registrado.");
                }

                var documentoArquivo = EnviarDocumento(token);

                if (documentoArquivo == null)
                {
                    throw new Exception("A Reserva de espaço no E-Docs não retornou os dados necessários.");
                }
                else
                {
                    RestClient cliente = null;
                    var request = new RestRequest();
                    request.Method = Method.Post;
                    request.AddHeader("Authorization", $"Bearer {token}");
                    request.AddHeader("Content-Type", "application/json-patch+json");

                    if (this.AgentePublico == null && this.Cidadao == null)
                    {
                        throw new Exception("Nenhum capturador foi registrado.");
                    }
                    else if (this.AgentePublico != null && this.Cidadao != null)
                    {
                        throw new Exception("Um capturador não pode ser Agente Publico e Cidadão ao mesmo tempo.");
                    }
                    else if (this.AgentePublico == null)
                    {
                        //Quando Nulo significa que o capturador é um Cidadão.
                        throw new NotImplementedException("Capturador Cidadão Ainda Não Implementado.");
                    }
                    else
                    {
                        if (DocumentoTipo == DocumentoTipoEnum.NatoDigital)
                        {
                            if (AssinaturaTipo == null || AssinaturaTipo == AssinaturaTipoEnum.Erro)
                            {
                                throw new Exception("Documentos Natos-Digitais devem ter indicação do tipo de Assinatura.");
                            }

                            if (AssinaturaTipo == AssinaturaTipoEnum.SemAssinatura)
                            {
                                var docRegistrar = new DTO.ServidorSemAssinatura(this, documentoArquivo);
                                cliente = new RestClient($"{Program.UrlBaseEdocs}documentos/capturar/nato-digital/copia/servidor");
                                request.AddJsonBody(docRegistrar);
                            }
                            else if (AssinaturaTipo == AssinaturaTipoEnum.Multiplas)
                            {
                                //var docRegistrar = new DTO.ServidorMultiplasAssinaturas(this, documentoArquivo);
                                //cliente = new RestClient($"{Program.UrlBaseEdocs}documentos/capturar/digitalizado/servidor");
                                //request.AddJsonBody(docRegistrar);
                                throw new Exception("acertar isso fdp");
                            }
                            else if (AssinaturaTipo == AssinaturaTipoEnum.AutoAssinado)
                            {
                                var docRegistrar = new DTO.ServidorAutoAssinado(this, documentoArquivo);
                                cliente = new RestClient($"{Program.UrlBaseEdocs}documentos/capturar/nato-digital/auto-assinado/servidor");
                                request.AddJsonBody(docRegistrar);
                            }
                            else if (AssinaturaTipo == AssinaturaTipoEnum.ICPBrasil)
                            {
                                var docRegistrar = new DTO.ServidorICPBrasil(this, documentoArquivo);
                                cliente = new RestClient($"{Program.UrlBaseEdocs}documentos/capturar/nato-digital/icp-brasil/servidor");
                                request.AddJsonBody(docRegistrar);
                            }
                        }
                        else if (DocumentoTipo == DocumentoTipoEnum.Digitalizado)
                        {
                            if (AssinaturaTipo != null &&
                                (AssinaturaTipo == AssinaturaTipoEnum.Multiplas ||
                                AssinaturaTipo == AssinaturaTipoEnum.ICPBrasil ||
                                AssinaturaTipo == AssinaturaTipoEnum.AutoAssinado)
                                )
                            {
                                throw new Exception("Documentos digitalizados não podem ser assinados.");
                            }

                            var docRegistrar = new DTO.ServidorDigitalizado(this, documentoArquivo);
                            cliente = new RestClient($"{Program.UrlBaseEdocs}documentos/capturar/digitalizado/servidor");
                            request.AddJsonBody(docRegistrar);
                        }
                        else
                        {
                            throw new Exception("Tipo de documento não reconhecido.");
                        }
                    }

                    var resposta = cliente.Execute(request);

                    if (resposta.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string respostaString = System.Text.Json.JsonSerializer.Deserialize<string>(resposta.Content);

                        if (string.IsNullOrWhiteSpace(respostaString))
                        {
                            throw new Exception($"Nenhum valor retornado. Código:<<{resposta.StatusCode}>>.Conteúdo:<<{resposta.Content}>>.");
                        }
                        else
                        {
                            if (Guid.TryParse(respostaString, out var guidEveto))
                            {
                                return guidEveto;
                            }
                            else
                            {
                                throw new Exception($"Não foi possível ler a GUID de Evento recebida. Código:<<{resposta.StatusCode}>>.Conteúdo:<<{resposta.Content}>>");
                            }
                        }
                    }
                    else
                    {
                        string teste = "teste";

                        if (resposta.Content.Contains("O documento não pode conter assinaturas digitais."))
                        {
                            this.DocumentoTipo = DocumentoTipoEnum.NatoDigital;
                            this.AssinaturaTipo = AssinaturaTipoEnum.ICPBrasil;
                        }

                        throw new Exception($"Código:<<{resposta.StatusCode}>>.Conteúdo:<<{resposta.Content}>>.");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("E-Docs. Documento. Registrar EDocs. ", e);
            }
        }

        /// <summary>
        /// Executa a primeira e segunda etapa do envio de um documento para o E-Docs.<br/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private DTO.DocumentoArquivo EnviarDocumento(string token)
        {
            DTO.DocumentoArquivo reserva = null;
            try
            {
                var byteArray = PegarPDFOnBase(DocumentHandle.Value);

                reserva = ReservaEspacoUpload(byteArray.LongLength, token);

                if (reserva == null)
                {
                    throw new Exception("Não foi possível reservar espaço para upload no E-Docs (Documento Arquivo Nulo).");
                }
                else
                {
                    if (!UploadArquivo(ref byteArray, reserva))
                    {
                        throw new Exception("Não foi possível fazer upload do documento.");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Enviar Documento. ", e);
            }
            return reserva;
        }

        /// <summary>
        /// Recupera um PDF no OnBase como byte[].
        /// </summary>
        /// <param name="docHandle">Código identificador do documento no OnBase</param>
        /// <returns>Byte[] do PDF</returns>
        internal static byte[] PegarPDFOnBase(long? docHandle)
        {
            try
            {
                if (docHandle == null || docHandle == 0)
                {
                    throw new Exception("A Document Handle informada não é válida.");
                }

                var documento = OnBase.App.Core.GetDocumentByID(docHandle.Value);

                if (documento == null)
                {
                    throw new Exception("Nenhum documento foi encontrado com esta Document Handle.");
                }

                byte[] byteArray = null;

                using (var ms = new MemoryStream())
                {
                    var pageData = OnBase.App.Core.Retrieval.PDF.GetDocument(documento.DefaultRenditionOfLatestRevision);

                    if (pageData == null)
                    {
                        throw new Exception("O documento indicado não é um PDF.");
                    }

                    pageData.Stream.CopyTo(ms);
                    byteArray = ms.ToArray();
                    pageData.Dispose();
                }

                return byteArray;
            }
            catch (Exception e)
            {
                throw new Exception("Pegar PDF do OnBase. ", e);
            }
        }

        /// <summary>
        /// Reserva no E-Docs o espaço para enviar o documento.(1º Etapa)
        /// </summary>
        /// <param name="tamanho">Tamanho do arquivo</param>
        /// <returns>True = Sucesso / False = Algum Erro</returns>
        private DTO.DocumentoArquivo ReservaEspacoUpload(long tamanho, string token)
        {
            DTO.DocumentoArquivo reserva = null;
            try
            {
                var cliente = new RestClient($"{Program.UrlBaseEdocs}documentos/upload-arquivo/gerar-url/{tamanho}");
                var requisicao = new RestRequest();
                requisicao.Method = Method.Get;
                requisicao.AddHeader("Authorization", $"Bearer {token}");
                var response = cliente.Execute(requisicao);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    reserva = JsonConvert.DeserializeObject<DTO.DocumentoArquivo>(response.Content);
                }
                else
                {
                    if (response.Content.Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>. Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>. Conteúdo:<<{response.Content}>>");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Reserva de Espaço para Upload. ", e);
            }
            return reserva;
        }

        /// <summary>
        /// Envia o Arquivo para o E-Docs.(2º Etapa)
        /// </summary>
        /// <param name="dadosUpload">Dados Usados para Executar o Envio</param>
        /// <returns>True = Sucesso / False = Algum Erro</returns>
        private bool UploadArquivo(ref byte[] arquivo, DTO.DocumentoArquivo documentoArquivo)
        {
            bool enviado = false;
            try
            {
                if (string.IsNullOrWhiteSpace(this.NomeDocumento) || this.NomeDocumento.Length < 4)
                {
                    throw new Exception("Nenhum nome foi especificado para o Documento.");
                }

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                MemoryStream memoryStream = new MemoryStream(arquivo);
                StreamContent streamContent = new StreamContent(memoryStream);
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file", //nome do parâmetro do arquivo, não alterar
                    FileName = NomeDocumento //na prática esse nome aqui não será utilizado???
                };
                var content = new MultipartFormDataContent { streamContent };
                foreach (var item in documentoArquivo.Body)
                {
                    content.Add(new StringContent(item.Value), item.Key);
                }

                var taskResponse = client.PostAsync(documentoArquivo.Url, content);

                taskResponse.Wait();

                var response = taskResponse.Result;

                if (response.IsSuccessStatusCode)
                {
                    enviado = true;
                }
                else
                {
                    string conteudo = response.Content.ToString();

                    if (string.IsNullOrWhiteSpace(conteudo))
                    {
                        conteudo = "A resposta não veio com conteúdo.";
                    }

                    if (conteudo.Contains("<HTML>"))
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<Retornou um Html>>");
                    }
                    else
                    {
                        throw new Exception($"Resposta de Erro do E-Docs. Código:<<{response.StatusCode}>>.Conteúdo:<<{conteudo}>>");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Upload do Arquivo. ", e);
            }
            return enviado;
        }

        /// <summary>
        /// Baixa os dados de um documento enviado ao E-Docs.
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="Exception"></exception>
        internal void Baixar(string token)
        {
            try
            {
                var cliente = new RestClient($"{Program.UrlBaseEdocs}documentos/{this.EDocsId}");
                var request = new RestRequest();
                request.Method = Method.Get;
                request.AddHeader("Authorization", $"Bearer {token}");
                var resposta = cliente.Execute(request);
                if (resposta.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var respostaDocumento = JsonConvert.DeserializeObject<DTO.RespostaDocumento>(resposta.Content);
                    this.Registro = respostaDocumento.Registro;
                }
                else
                {
                    throw new Exception($"Código:<<{resposta.StatusCode}>>.Conteúdo:<<{resposta.Content}>>");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Documento.BaixarDados", e);
            }
        }
    }
}
