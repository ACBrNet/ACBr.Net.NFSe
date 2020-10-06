using ACBr.Net.Core.Extensions;
using System;
using System.Text;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Providers.Sigiss
{
    internal sealed class SigissServiceClient : NFSeSOAP11ServiceClient, IServiceClient
    {
        #region Constructors

        public SigissServiceClient(ProviderSigiss provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {

        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            var request = new StringBuilder();
            request.Append("<urn:GerarNota soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            request.Append("<DescricaoRps xsi:type=\"urn:tcDescricaoRps\">");
            request.AppendEnvio(msg);
            request.Append("</DescricaoRps>");
            request.Append("</urn:GerarNota>");

            return Execute("urn:sigiss_ws#GerarNota", request.ToString(), "ns1:GerarNotaResponse");
        }

        public string ConsultarSituacao(string cabec, string msg)
        {
            var request = new StringBuilder();
            request.Append("<urn:ConsultarNotaValida soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            request.Append("<DadosConsultaNota xsi:type=\"urn:tcDadosConsultaNota\">");
            request.AppendEnvio(msg);
            request.Append("</DadosConsultaNota>");
            request.Append("</urn:ConsultarNotaValida>");

            return Execute("urn:sigiss_ws#ConsultarNotaValida", request.ToString(), "ns1:ConsultarNotaValidaResponse");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            var request = new StringBuilder();
            request.Append("<urn:CancelarNota soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            request.Append("<DadosCancelaNota xsi:type=\"urn:tcDadosCancelaNota\">");
            request.AppendEnvio(msg);
            request.Append("</DadosCancelaNota>");
            request.Append("</urn:CancelarNota>");

            return Execute("urn:sigiss_ws#CancelarNota", request.ToString(), "ns1:CancelarNotaResponse");
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            return xmlDocument.ElementAnyNs(responseTag[0])/*.ElementAnyNs("return")*/.ToString();
        }

        #region Não Utilizados

        public string CancelarNFSeLote(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarLoteRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarNFSeRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string ConsultarSequencialRps(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string EnviarSincrono(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        public string SubstituirNFSe(string cabec, string msg)
        {
            throw new NotImplementedException();
        }

        #endregion Não Utilizados

        #endregion Methods
    }
}
