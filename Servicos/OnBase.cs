using Hyland.Unity;

namespace CIEO.Servicos
{
    /// <summary>
    /// Classe que Gerencia a Conexão com OnBase.
    /// </summary>
    internal static class OnBase
    {
        /// <summary>
        /// App do OnBase.
        /// </summary>
        internal static Hyland.Unity.Application? App { get; set; }

        private static DateTime _ultimaUtilizacao;

        /// <summary>
        /// Abre Conexão se estiver fechada.
        /// </summary>
        internal static void ManutencaoConexao()
        {
            bool abrir = false;
            _ultimaUtilizacao = DateTime.Now;

            try
            {
                if (App == null)
                {
                    abrir = true;
                }
                else if (!App.IsConnected)
                {
                    //não pode ficar encima por causa do dispose.
                    App.Dispose();
                    abrir = true;
                }
                else if (!App.Ping())
                {
                    //Em caso de algum problema este ping pode demorar ou retornar Erro
                    App.Dispose();
                    abrir = true;
                }
            }
            catch (Exception)
            {
                abrir = true;
            }

            if (abrir)
            {
                Conectar();
            }
        }

        /// <summary>
        /// Finaliza Conexão com OnBase.
        /// </summary>
        internal static void Finalizar()
        {
            //conexão só poderá ser finalizada se estiver 20 minutos parada.
            var validade = _ultimaUtilizacao.AddMinutes(20);
            if (validade.CompareTo(DateTime.Now) <= 0)
            {
                if (App != null)
                {
                    if (App.IsConnected)
                    {
                        App.Disconnect();
                    }

                    App.Dispose();
                }
            }
        }

        /// <summary>
        /// Abre conexão com OnBase. Se não estiver aberta.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void Conectar()
        {
            if (App == null || !App.IsConnected || !App.Ping())
            {
                var authProps = Application
                    .CreateOnBaseAuthenticationProperties(
                    //"http://10.243.129.85/AppServer64/service.asmx",
                    //"http://10.243.131.72/AppServer/service.asmx",
                    "http://10.243.129.36/AppServer/service.asmx",
                    "luiz.junior",
                    "m4r1n0m0",
                    "OnBase"
                    );

                authProps.LicenseType = LicenseType.QueryMetering;

                try
                {
                    App = Application.Connect(authProps);
                }
                catch (Exception e)
                {
                    throw new Exception("OnBase. Conectar. ", e);
                }
            }
        }
    }
}