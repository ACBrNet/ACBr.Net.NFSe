using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    internal sealed class BeloHorizonteServiceClient : NFSeSOAP11ServiceClient, IABRASFClient
    {
        #region Constructors

        public BeloHorizonteServiceClient(ProviderBeloHorizonte provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        public string CancelarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:CancelarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:CancelarNfseRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/CancelarNfse", message.ToString(), "CancelarNfseResponse");
        }

        public string ConsultarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarLoteRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarLoteRps", message.ToString(), "ConsultarLoteRpsResponse");
        }

        public string ConsultarNFSe(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarNfseRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarNfse", message.ToString(), "ConsultarNfseResponse");
        }

        public string ConsultarNfsePorFaixa(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfsePorFaixaRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarNfsePorFaixaRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarNfsePorFaixa", message.ToString(), "ConsultarNfsePorFaixaResponse");
        }

        public string ConsultarNFSePorRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarNfsePorRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarNfsePorRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarNfsePorRps", message.ToString(), "ConsultarNfsePorRpsResponse");
        }

        public string ConsultarSituacaoLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:ConsultarSituacaoLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:ConsultarSituacaoLoteRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/ConsultarSituacaoLoteRps", message.ToString(), "ConsultarSituacaoLoteRpsResponse");
        }

        public string RecepcionarLoteRps(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:RecepcionarLoteRpsRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:RecepcionarLoteRpsRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/RecepcionarLoteRps", message.ToString(), "RecepcionarLoteRpsResponse");
        }

        public string GerarNfse(string nfseCabecMsg, string nfseDadosMsg)
        {
            var message = new StringBuilder();
            message.Append("<ws:GerarNfseRequest>");
            message.Append("<nfseCabecMsg>");
            message.AppendCData(nfseCabecMsg);
            message.Append("</nfseCabecMsg>");
            message.Append("<nfseDadosMsg>");
            message.AppendCData(nfseDadosMsg);
            message.Append("</nfseDadosMsg>");
            message.Append("</ws:GerarNfseRequest>");

            return Execute("http://ws.bhiss.pbh.gov.br/GerarNfse", message.ToString(), "GerarNfseResponse");
        }

        private string Execute(string action, string message, string responseTag)
        {
            return Execute(action, message, responseTag, "xmlns:ws=\"http://ws.bhiss.pbh.gov.br\"");
        }

        protected override string TratarRetorno(string responseTag, XDocument xmlDocument)
        {
            var element = xmlDocument.ElementAnyNs("Fault");
            if (element != null)
            {
                var exMessage = $"{element.ElementAnyNs("faultcode").GetValue<string>()} - {element.ElementAnyNs("faultstring").GetValue<string>()}";
                throw new ACBrDFeCommunicationException(exMessage);
            }

            return xmlDocument.ElementAnyNs(responseTag).ElementAnyNs("outputXML").Value;
        }

        #endregion Methods
    }
}