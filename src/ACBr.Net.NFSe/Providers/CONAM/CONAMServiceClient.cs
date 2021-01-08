using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.NFSe.Configuracao;

namespace ACBr.Net.NFSe.Providers
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
            message.Append("<ws_nfe.PROCESSARPS>");
            message.Append("<Sdt_processarpsin xmlns=\"NFe\">");
            message.Append("<Login>");
            message.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
            message.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
            message.Append("</Login>");
            message.Append(dados);
            message.Append("</Sdt_processarpsin>");
            message.Append("</ws_nfe.PROCESSARPS>");

            return Execute(message.ToString(), "PROCESSARPS");
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string ConsultarSituacao(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<ws_nfe.CONSULTAPROTOCOLO>");
            message.Append("<Sdt_consultaprotocoloin xmlns=\"NFe\">");
            message.Append(dados);
            message.Append("<Login>");
            message.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
            message.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
            message.Append("</Login>");
            message.Append("</Sdt_consultaprotocoloin>");
            message.Append("</ws_nfe.CONSULTAPROTOCOLO>");

            return Execute(message.ToString(), "CONSULTAPROTOCOLO");
        }

        public string ConsultarLoteRps(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<ws_nfe.CONSULTANOTASPROTOCOLO>");
            message.Append("<Sdt_consultanotasprotocoloin xmlns=\"NFe\">");
            message.Append(dados);
            message.Append("<Login>");
            message.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
            message.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
            message.Append("</Login>");
            message.Append("</Sdt_consultanotasprotocoloin>");
            message.Append("</ws_nfe.CONSULTANOTASPROTOCOLO>");

            return Execute(message.ToString(), "CONSULTANOTASPROTOCOLO");
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new System.NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabecalho, string dados)
        {
            var xmlEnvio = new StringBuilder();
            xmlEnvio.Append("<SDT_IMPRESSAO_IN xmlns=\"NFe\">");
            xmlEnvio.Append("<Login>");
            xmlEnvio.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
            xmlEnvio.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
            xmlEnvio.Append("</Login>");
            xmlEnvio.Append("<Nota>");
            xmlEnvio.Append(dados);
            xmlEnvio.Append("</Nota>");
            xmlEnvio.Append("</SDT_IMPRESSAO_IN>");

            var message = new StringBuilder();
            message.Append("<ws_nfe.IMPRESSAOLINKNFSE>");
            message.Append("<Xml_entrada>");
            message.AppendCData(xmlEnvio.ToString());
            message.Append("</Xml_entrada>");
            message.Append("</ws_nfe.IMPRESSAOLINKNFSE>");

            return Execute(message.ToString(), "IMPRESSAOLINKNFSE");
        }

        public string ConsultarNFSe(string cabecalho, string dados)
        {
            var xmlEnvio = new StringBuilder();
            xmlEnvio.Append("<SDT_IMPRESSAO_IN xmlns=\"NFe\">");
            xmlEnvio.Append("<Login>");
            xmlEnvio.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
            xmlEnvio.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
            xmlEnvio.Append("</Login>");
            xmlEnvio.Append("<Nota>");
            xmlEnvio.Append(dados);
            xmlEnvio.Append("</Nota>");
            xmlEnvio.Append("</SDT_IMPRESSAO_IN>");

            var message = new StringBuilder();
            message.Append("<ws_nfe.IMPRESSAOLINKNFSE>");
            message.Append("<Xml_entrada>");
            message.AppendCData(xmlEnvio.ToString());
            message.Append("</Xml_entrada>");
            message.Append("</ws_nfe.IMPRESSAOLINKNFSE>");

            return Execute(message.ToString(), "IMPRESSAOLINKNFSE");
        }

        public string CancelarNFSe(string cabecalho, string dados)
        {
            var message = new StringBuilder();
            message.Append("<ws_nfe.CANCELANOTAELETRONICA>");
            message.Append("<Sdt_cancelanfe xmlns=\"NFe\">");
            message.Append("<Login>");
            message.Append($"<CodigoUsuario>{Provider.Configuracoes.WebServices.Usuario}</CodigoUsuario>");
            message.Append($"<CodigoContribuinte>{Provider.Configuracoes.WebServices.Senha}</CodigoContribuinte>");
            message.Append("</Login>");
            message.Append("<Nota>");
            message.Append(dados);
            message.Append("</Nota>");
            message.Append("</Sdt_cancelanfe>");
            message.Append("</ws_nfe.CANCELANOTAELETRONICA>");

            return Execute(message.ToString(), "CANCELANOTAELETRONICA");
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
            return Execute("", message, "", responseTag);
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            return xmlDocument.ElementAnyNs(responseTag[0]).ElementAnyNs("return").Value;
        }

        protected override bool ValidarCertificadoServidor()
        {
            return false;
        }

        #endregion Methods
    }
}
