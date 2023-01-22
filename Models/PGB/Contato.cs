using CIEO.Servicos;
using Hyland.Unity.Extensions;
using Hyland.Unity.WorkView;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CIEO.Models.PGB
{
    public class Contato
    {
        /// <summary>
        /// Object ID do Objeto Contato
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// De quem é este contato, pode ser PF ou PJ.
        /// </summary>
        public PessoaFisica? PessoaFisica { get; set; }

        //internal PessoaJuridica PessoaJuridica { get; set; }

        /// <summary>
        /// Object Id do Objeto Tipo Contato.
        /// </summary>
        public TipoContato? TipoContato { get; set; }

        /// <summary>
        /// De onde este contato veio.
        /// </summary>
        public OrigemInformacao? OrigemInformacao { get; set; }

        /// <summary>
        /// Se este contato foi "Desabilitado".
        /// </summary>
        public bool? Ativo { get; set; }

        /// <summary>
        /// Valor deste contato (Ex.:fulano@teste.com).
        /// </summary>
        public string? Valor { get; set; }

        /// <summary>
        /// Verifica se o contato existe, se não existe cria.<br/>
        /// Se existe e estiver desatualizado atualiza.
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal void Tratar(Hyland.Unity.WorkView.Application wvApp)
        {
            try
            {
                if (PessoaFisica == null ||
                    PessoaFisica.Id == null ||
                    PessoaFisica.Id <= 0
                    )
                {
                    throw new Exception("Todo contato deve estar vinculado a uma pessoa criada no OnBase.");
                }
                //ainda falta programar.
                //pf e pj = null erro deve deve ser vinculado a uma pessoa.

                if (string.IsNullOrWhiteSpace(this.Valor))
                {
                    throw new Exception("Não é possível gravar um contato em branco.");
                }

                if (
                    TipoContato == null ||
                    (
                     string.IsNullOrWhiteSpace(TipoContato.Valor) &&
                     (TipoContato.Id == null || TipoContato.Id <= 0)
                    )
                   )
                {
                    throw new Exception("É necessário informar o tipo do contato. Através da Id ou do Valor.");
                }
                else if (TipoContato.Id == null || TipoContato.Id <= 0)
                {
                    TipoContato.PegarId(wvApp);
                }

                if (
                    OrigemInformacao == null ||
                    (
                     string.IsNullOrWhiteSpace(OrigemInformacao.Valor) &&
                     (OrigemInformacao.Id == null || OrigemInformacao.Id <= 0)
                    )
                   )
                {
                    throw new Exception("É necessário informar a Origem da Informação do contato. Através da Id ou do Valor.");
                }
                else if (OrigemInformacao.Id == null || OrigemInformacao.Id <= 0)
                {
                    OrigemInformacao.PegarId(wvApp);
                }

                var contatoWVClass = wvApp.Classes.Find("PGBContato");
                var fQ = wvApp.Filters.Find("DB - PGBContato").CreateFilterQuery();
                if (this.PessoaFisica != null)
                {
                    fQ.AddConstraint("PessoaFisica", Operator.Equal, this.PessoaFisica.Id.Value);
                }
                /*
                else (this.PJ != null)
                 {
                } 
                 */

                fQ.AddConstraint("Valor", Operator.Like, this.Valor);

                var contador = fQ.ExecuteCount();
                if (contador == 0)
                {
                    Criar(contatoWVClass);
                }
                else if (contador == 1)
                {
                    Atualizar(fQ.Execute(contador)[0], contatoWVClass);
                }
                else
                {
                    throw new Exception($"Este contato({this.Valor}), desta pessoa já foi registrado mais de uma vez e precisa ser tratado.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Contato. Tratar. ", e);
            }
        }

        /// <summary>
        /// Verifica se os atributos estão diferente e atualiza se for o caso.
        /// </summary>
        /// <param name="objetoEncontrado"></param>
        private void Atualizar(FilterQueryResultItem objetoEncontrado, Hyland.Unity.WorkView.Class contatoWVClass)
        {
            this.Id = objetoEncontrado.ObjectID;
            bool aplicar = false;
            var coluna = objetoEncontrado.GetFilterColumnValue("OrigemInformacao.objectid");
            if (coluna.HasValue)
            {
                if (this.OrigemInformacao.Id.Value != coluna.IntegerValue)
                {
                    aplicar = true;
                }
            }
            else
            {
                aplicar = true;
            }

            coluna = objetoEncontrado.GetFilterColumnValue("TipoContato.objectid");
            if (coluna.HasValue)
            {
                if (this.TipoContato.Id.Value != coluna.IntegerValue)
                {
                    aplicar = true;
                }
            }
            else
            {
                aplicar = true;
            }

            coluna = objetoEncontrado.GetFilterColumnValue("Ativo");
            if (coluna.HasValue)
            {
                if (this.Ativo != coluna.BooleanValue)
                {
                    aplicar = true;
                }
            }
            else
            {
                aplicar = true;
            }

            coluna = objetoEncontrado.GetFilterColumnValue("Valor");
            if (coluna.HasValue)
            {
                if (!this.Valor.Equals(coluna.AlphanumericValue))
                {
                    aplicar = true;
                }
            }
            else
            {
                aplicar = true;
            }

            if (aplicar)
            {
                var objContato = contatoWVClass.GetObjectByID(this.Id.Value);

                var attMod = objContato.CreateAttributeValueModifier();
                attMod.SetAttributeValue("OrigemInformacao", this.OrigemInformacao.Id.Value);
                attMod.SetAttributeValue("TipoContato", this.TipoContato.Id.Value);

                if (this.Ativo == null)
                {
                    attMod.SetAttributeValue("Ativo", true);
                }
                else
                {
                    attMod.SetAttributeValue("Ativo", this.Ativo.Value);
                }

                attMod.SetAttributeValue("Valor", this.Valor);

                attMod.ApplyChanges();
            }
        }

        /// <summary>
        /// Cria o Contato Objeto do WorkView.
        /// </summary>
        private void Criar(Hyland.Unity.WorkView.Class contatoWVClass)
        {
            var objContato = contatoWVClass.CreateObject();
            var attMod = objContato.CreateAttributeValueModifier();
            attMod.SetAttributeValue("OrigemInformacao", this.OrigemInformacao.Id.Value);
            attMod.SetAttributeValue("TipoContato", this.TipoContato.Id.Value);
            attMod.SetAttributeValue("Valor", this.Valor);

            if (this.PessoaFisica != null)
            {
                attMod.SetAttributeValue("PessoaFisica", this.PessoaFisica.Id.Value);
                attMod.SetAttributeValue("TipoPessoa", "PESSOA FÍSICA");
            }

            attMod.ApplyChanges();

            this.Id = objContato.ID;
        }
    }
}
