using System.Drawing;

namespace CIEO.Models.PGB
{
    public class Endereco
    {
        internal long Id { get; private set; }
        internal string? Logradouro { get; private set; }
        internal PessoaFisica PessoaFisica { get; private set; }
        //internal PessoaJuridica PessoaJuridica { get; private set; }

        public Endereco(long id, string? logradouro, PessoaFisica pessoaFisica/*, PessoaJuridica pessoaJuridica*/)
        {
            Id = id;
            Logradouro = string.IsNullOrWhiteSpace(logradouro) ? null : logradouro.Trim();
            PessoaFisica = pessoaFisica;
            //PessoaJuridica = pessoaJuridica;
        }
    }
}
