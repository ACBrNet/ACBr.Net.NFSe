using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.NFSe.Configuracao;

namespace ACBr.Net.NFSe.Providers.CONAM
{
    internal sealed class CONAMServiceClient : NFSeSOAP12ServiceClient, IServiceClient
    {
        #region Constructors

        public CONAMServiceClient(ProviderCONAM provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:RecepcionarLoteRpsV3>");
            message.Append("<arg0>");
            message.AppendEnvio(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendEnvio(dados);
            message.Append("</arg1>");
            message.Append("</gin:RecepcionarLoteRpsV3>");

            return Execute(message.ToString(), "RecepcionarLoteRpsV3Response");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string ConsultarSituacao(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:ConsultarSituacaoLoteRpsV3>");
            message.Append("<arg0>");
            message.AppendEnvio(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendEnvio(dados);
            message.Append("</arg1>");
            message.Append("</gin:ConsultarSituacaoLoteRpsV3>");

            return Execute(message.ToString(), "ConsultarSituacaoLoteRpsV3Response");
        }

        public string ConsultarLoteRps(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:ConsultarLoteRpsV3>");
            message.Append("<arg0>");
            message.AppendEnvio(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendEnvio(dados);
            message.Append("</arg1>");
            message.Append("</gin:ConsultarLoteRpsV3>");

            return Execute(message.ToString(), "ConsultarLoteRpsV3Response");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:ConsultarNfsePorRpsV3>");
            message.Append("<arg0>");
            message.AppendEnvio(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendEnvio(dados);
            message.Append("</arg1>");
            message.Append("</gin:ConsultarNfsePorRpsV3>");

            return Execute(message.ToString(), "ConsultarNfsePorRpsV3Response");
        }

        public string ConsultarNFSe(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<gin:ConsultarNfseV3>");
            message.Append("<arg0>");
            message.AppendEnvio(cabecalho);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendEnvio(dados);
            message.Append("</arg1>");
            message.Append("</gin:ConsultarNfseV3Response>");

            return Execute(message.ToString(), "RecepcionarLoteRpsV3Response");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var message = new StringBuilder();
            message.Append("<gin:CancelarNfseV3>");
            message.Append("<arg0>");
            message.AppendEnvio(cabec);
            message.Append("</arg0>");
            message.Append("<arg1>");
            message.AppendEnvio(msg);
            message.Append("</arg1>");
            message.Append("</gin:CancelarNfseV3>");

            return Execute(message.ToString(), "CancelarNfseV3Response");
        }

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        private string Execute(string message, string responseTag)
        {
            var ns = Provider.Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao ?
                            "xmlns:gin=\"http://homologacao.ginfes.com.br\"" : "xmlns:gin=\"http://producao.ginfes.com.br\"";

            return Execute("", message, "", responseTag, ns);
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("return").Value;
        }

        #endregion Methods
    }
}
