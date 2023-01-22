using CIEO.DB;
using CIEO.Models;
using Newtonsoft.Json;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace CIEO.Servicos
{
    public static class Uteis
    {
        /// <summary>
        /// Verifica se um CPF é válido.
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns>True = Válido</returns>
        internal static bool CPFValido(string cpf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                {
                    return false;
                }

                if (cpf.Length != 11)
                {
                    return false;
                }

                if (cpf.Equals("11111111111") ||
                    cpf.Equals("22222222222") ||
                    cpf.Equals("33333333333") ||
                    cpf.Equals("44444444444") ||
                    cpf.Equals("55555555555") ||
                    cpf.Equals("66666666666") ||
                    cpf.Equals("77777777777") ||
                    cpf.Equals("88888888888") ||
                    cpf.Equals("99999999999") ||
                    cpf.Equals("00000000000"))
                {
                    return false;
                }

                string tempCpf;
                string digito;
                int soma;
                int resto;

                tempCpf = cpf.Substring(0, 9);
                soma = 0;

                for (int i = 10; i > 1; i--)
                {
                    soma += int.Parse(tempCpf[10 - i].ToString()) * i;
                }

                resto = soma % 11;

                if (resto < 2)
                {
                    resto = 0;
                }
                else
                {
                    resto = 11 - resto;
                }

                digito = resto.ToString();
                tempCpf = tempCpf + digito;
                soma = 0;

                for (int i = 11; i > 1; i--)
                {
                    soma += int.Parse(tempCpf[11 - i].ToString()) * i;
                }

                resto = soma % 11;

                if (resto < 2)
                {
                    resto = 0;
                }
                else
                {
                    resto = 11 - resto;
                }

                digito = digito + resto.ToString();

                return cpf.EndsWith(digito);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se um CNPJ é válido.
        /// </summary>
        /// <param name="cPFCNPJ"></param>
        /// <returns>true = válido</returns>
        internal static bool CNPJValido(string? cnpj)
        {
            try
            {
                int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                int soma;
                int resto;
                string digito;
                string tempCnpj;

                cnpj = cnpj.Trim();
                cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

                if (cnpj.Length != 14)
                    return false;

                tempCnpj = cnpj.Substring(0, 12);

                soma = 0;
                for (int i = 0; i < 12; i++)
                    soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

                resto = (soma % 11);
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = resto.ToString();

                tempCnpj = tempCnpj + digito;
                soma = 0;
                for (int i = 0; i < 13; i++)
                    soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

                resto = (soma % 11);
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = digito + resto.ToString();

                return cnpj.EndsWith(digito);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se um E-Mail é válido
        /// </summary>
        /// <param name="email"></param>
        /// <returns>true = válido</returns>
        internal static bool EmailValido(string? email)
        {
            try
            {
                var trimmedEmail = email.Trim();

                if (trimmedEmail.EndsWith("."))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    return false;
                }

                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converte stream em Byte[]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        internal static string MontarMsgErro(string msg, Exception e)
        {
            if (e == null)
            {
                return msg;
            }
            else
            {
                msg += e.Message;
                return MontarMsgErro(msg, e.InnerException);
            }
        }
    }
}

