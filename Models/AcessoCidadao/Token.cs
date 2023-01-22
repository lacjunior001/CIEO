namespace CIEO.Models.AcessoCidadao
{
    public class Token
    {
        public long? Id { get; set; }
        public Usuario? Usuario { get; set; }
        public string? AcessToken { get; set; }

        public Token()
        {
        }

        public Token(string accessToken)
        {
            this.AcessToken = accessToken;
        }
    }
}
