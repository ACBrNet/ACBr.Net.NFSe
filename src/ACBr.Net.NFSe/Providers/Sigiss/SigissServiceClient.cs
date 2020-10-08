using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Providers.Sigiss
{
    internal sealed class SigissServiceClient : NFSeSOAP11Charset8859ServiceClient, IServiceClient
    {
        #region Constructors

        public SigissServiceClient(ProviderSigiss provider, TipoUrl tipoUrl) : base(provider, tipoUrl, https: true)
        {

        }

        #endregion Constructors

        #region Methods

        public string Enviar(string cabec, string msg)
        {
            StringBuilder request = new StringBuilder();
            request.Append("<urn:GerarNota soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            request.Append(msg);
            request.Append("</urn:GerarNota>");

            return Execute("urn:sigiss_ws#GerarNota", request.ToString(), new string[] { "GerarNotaResponse", "RetornoNota" }, "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "xmlns:urn=\"urn: sigiss_ws\"");
        }

        //Ainda não está em utilizacao porem provider tem suporte
        public string ConsultarSituacao(string cabec, string msg)
        {
            StringBuilder request = new StringBuilder();
            request.Append("<urn:ConsultarNotaValida soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            request.Append("<DadosConsultaNota xsi:type=\"urn:tcDadosConsultaNota\">");
            request.Append(msg);
            request.Append("</DadosConsultaNota>");
            request.Append("</urn:ConsultarNotaValida>");

            return Execute("urn:sigiss_ws#ConsultarNotaValida", request.ToString(), new string[] { "GerarNotaResponse", "RetornoNota" }, "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "xmlns:urn=\"urn: sigiss_ws\"");
        }

        public string CancelarNFSe(string cabec, string msg)
        {
            StringBuilder request = new StringBuilder();
            request.Append("<urn:CancelarNota soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            request.Append(msg);
            request.Append("</urn:CancelarNota>");

            return Execute("urn:sigiss_ws#CancelarNota", request.ToString(), new string[] { "CancelarNotaResponse", "RetornoNota" }, "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "xmlns:urn=\"urn: sigiss_ws\"");
        }

        protected override string TratarRetorno(XDocument xmlDocument, string[] responseTag)
        {
            //verifica se o retorno tem os elementos corretos senão da erro.
            var element = xmlDocument.ElementAnyNs(responseTag[0]) ?? throw new ACBrDFeCommunicationException($"Primeiro Elemento ({responseTag[0]}) do xml não encontrado");
            _ = element.ElementAnyNs(responseTag[1]) ?? throw new ACBrDFeCommunicationException($"Dados ({responseTag[1]}) do xml não encontrado");
            _ = element.ElementAnyNs("DescricaoErros") ?? throw new ACBrDFeCommunicationException($"Erro ({responseTag[1]}) do xml não encontrado");

            return element.ToString();
        }

        protected override bool ValidarCertificadoServidor()
        {
            return false;
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
