namespace CIEO.Models.EDocs.Processo.DTO
{
    /// <summary>
    /// Objeto que será serializado para fazer o pedido de Autuação.
    /// </summary>
    public class AutuarEnvio
    {
        /// <summary>
        /// ID da Classe do novo processo que será autuado.
        /// </summary>
        public Guid IdClasse { get; private set; }

        /// <summary>
        /// Id do papel do servidor responsável pela autuação - o papel deve ter essa permissão de autuar na Organização.
        /// </summary>
        public Guid IdPapelResponsavel { get; private set; }

        /// <summary>
        /// Id do local de autuação - o papel deve ter essa permissão de autuação nesse local.
        /// </summary>
        public Guid IdLocal { get; private set; }

        /// <summary>
        /// Resumo ou título do processo
        /// </summary>
        public string Resumo { get; private set; }

        /// <summary>
        /// Lista de ids dos agentes interessados (Papel/Cidadão/Grupo/Organização/etc.).
        /// </summary>
        public List<Guid> IdsAgentesInteressados { get; private set; }

        /// <summary>
        /// Lista das Pessoas Jurídicos privadas interessados no processo autuado
        /// </summary>
        public List<PessoaJuridicaInteressada> PessoasJuridicasInteressadas { get; private set; }

        /// <summary>
        /// Lista dos Documentos juntados na Autuação.
        /// </summary>
        public List<Guid> IdsDocumentosEntranhados { get; private set; }

        /// <summary>
        /// Constrói a autuação com os parâmetros necessário de envio ao E-Docs.
        /// </summary>
        /// <param name="autuacao"></param>
        public AutuarEnvio(Autuacao? autuacao)
        {
            if (autuacao == null)
            {
                throw new ArgumentNullException("Autuacao", "Nenhuma Autuação do banco recebida.");
            }

            if (autuacao.Processo == null)
            {
                throw new ArgumentNullException("Processo", "Nenhuma Processo recebido.");
            }

            if (autuacao.EDocsGUID != null || autuacao.Processo.EdocsGUID != null || !string.IsNullOrWhiteSpace(autuacao.Processo.Protocolo))
            {
                throw new Exception("Este processo já foi autuado no E-Docs.");
            }

            if (autuacao.Processo.Classe == null)
            {
                throw new ArgumentNullException("IdClasse", "Nenhuma Classe Recebida.");
            }

            if (autuacao.Processo.Classe.Id.HasValue)
            {
                this.IdClasse = autuacao.Processo.Classe.AcessoCidadaoId.Value;
            }
            else
            {
                throw new ArgumentNullException("IdClasse", "Id da Classe está nula.");
            }

            if (autuacao.PapelPraticante == null)
            {
                throw new ArgumentNullException("IdPapelResponsavel", "Nenhum Papel recebido.");
            }

            if (autuacao.PapelPraticante.AcessoCidadaoId.HasValue)
            {
                this.IdPapelResponsavel = autuacao.PapelPraticante.AcessoCidadaoId.Value;
            }
            else
            {
                throw new ArgumentNullException("IdPapelResponsavel", "Id da Classe Processual está nula.");
            }

            if (autuacao.LocalAutuacao == null)
            {
                throw new ArgumentNullException("IdLocal", "Nenhum local de Autuação recebido.");
            }

            if (autuacao.LocalAutuacao.AcessoCidadaoId.HasValue)
            {
                this.IdLocal = autuacao.LocalAutuacao.AcessoCidadaoId.Value;
            }
            else
            {
                throw new ArgumentNullException("IdLocal", "Id do local de autuação está nula.");
            }

            if (string.IsNullOrWhiteSpace(autuacao.Processo.Resumo))
            {
                throw new ArgumentNullException("Resumo", "Nenhum texto de resumo do processo foi informado.");
            }
            else
            {
                this.Resumo = autuacao.Processo.Resumo.Trim();
            }

            if (autuacao.Processo.Interessados == null || autuacao.Processo.Interessados.Count == 0)
            {
                throw new ArgumentNullException("IdsAgentesInteressados", "Nenhum interessado foi informado.");
            }
            else
            {
                this.IdsAgentesInteressados = new List<Guid>(5);
                this.PessoasJuridicasInteressadas = new List<PessoaJuridicaInteressada>(5);

                foreach (var interessado in autuacao.Processo.Interessados)
                {
                    switch (interessado.InteressadoTipo)
                    {
                        case AgenteTipoEnum.Cidadao:
                            var guidCidadao = interessado.RecuperarGUIDCidadao();
                            this.IdsAgentesInteressados.Add(guidCidadao);
                            break;

                        case AgenteTipoEnum.PessoaJuridica:
                            var pjIneressada = new PessoaJuridicaInteressada(interessado);
                            this.PessoasJuridicasInteressadas.Add(pjIneressada);
                            break;

                        case AgenteTipoEnum.Papel:
                            if (string.IsNullOrWhiteSpace(interessado.AgenteId))
                            {
                                throw new Exception("A Id do Papel precisa ser indicada no campo AgenteId.");
                            }
                            else
                            {
                                if (Guid.TryParse(interessado.AgenteId, out var guidPapel))
                                {
                                    this.IdsAgentesInteressados.Add(guidPapel);
                                }
                                else
                                {
                                    throw new Exception($"Id informada para o papel({interessado.AgenteId}) não é válida.");
                                }
                            }
                            break;
                        case AgenteTipoEnum.Grupo:
                            if (string.IsNullOrWhiteSpace(interessado.AgenteId))
                            {
                                throw new Exception("A Id do Grupo precisa ser indicada no campo AgenteId.");
                            }
                            else
                            {
                                if (Guid.TryParse(interessado.AgenteId, out var guidPapel))
                                {
                                    this.IdsAgentesInteressados.Add(guidPapel);
                                }
                                else
                                {
                                    throw new Exception($"Id informada para o Grupo({interessado.AgenteId}) não é válida.");
                                }
                            }
                            break;
                        case AgenteTipoEnum.Unidade:
                            if (string.IsNullOrWhiteSpace(interessado.AgenteId))
                            {
                                throw new Exception("A Id da Unidade precisa ser indicada no campo AgenteId.");
                            }
                            else
                            {
                                if (Guid.TryParse(interessado.AgenteId, out var guidPapel))
                                {
                                    this.IdsAgentesInteressados.Add(guidPapel);
                                }
                                else
                                {
                                    throw new Exception($"Id informada para o Grupo({interessado.AgenteId}) não é válida.");
                                }
                            }
                            break;

                        case AgenteTipoEnum.Organizacao:
                            if (string.IsNullOrWhiteSpace(interessado.AgenteId))
                            {
                                throw new Exception("A Id da Organização precisa ser indicada no campo AgenteId.");
                            }
                            else
                            {
                                if (Guid.TryParse(interessado.AgenteId, out var guidPapel))
                                {
                                    this.IdsAgentesInteressados.Add(guidPapel);
                                }
                                else
                                {
                                    throw new Exception($"Id informada para o Organização({interessado.AgenteId}) não é válida.");
                                }
                            }
                            break;

                        case AgenteTipoEnum.Sistema:
                            if (string.IsNullOrWhiteSpace(interessado.AgenteId))
                            {
                                throw new Exception("A Id do Sistema precisa ser indicada no campo AgenteId.");
                            }
                            else
                            {
                                if (Guid.TryParse(interessado.AgenteId, out var guidPapel))
                                {
                                    this.IdsAgentesInteressados.Add(guidPapel);
                                }
                                else
                                {
                                    throw new Exception($"Id informada para o Sistema({interessado.AgenteId}) não é válida.");
                                }
                            }
                            break;

                        case AgenteTipoEnum.NaoIdentificado:
                            throw new Exception("Por regra de negócios da implantação da integração agentes não identificados não são permitidos.");

                        default:
                            throw new Exception("Tipo de interessado não identificado.");
                    }
                }
            }

            if (autuacao.Documentos != null && autuacao.Documentos.Count > 0)
            {
                this.IdsDocumentosEntranhados = new List<Guid>(autuacao.Documentos.Count);
                foreach (var documento in autuacao.Documentos)
                {
                    //if (documento.Documento.Situacao == SituacaoEnum.EnviadoParaEDocs ||
                    //    documento.Documento.Situacao == SituacaoEnum.DadosBaixadosDoEDocs)
                    //{
                        IdsDocumentosEntranhados.Add(documento.Documento.EDocsId.Value);
                    //}
                    //else
                    //{
                    //    throw new Exception("Aguardando Sincronização de Documentos.");
                    //}
                }
            }
        }
    }
}