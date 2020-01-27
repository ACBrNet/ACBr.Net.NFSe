using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ACBr.Net.Core;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Providers
{
    internal abstract class NFSeSOAP11ServiceClient : NFSeServiceClient<IRequestChannel>
    {
        #region Constructors

        protected NFSeSOAP11ServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado) : base(provider, tipoUrl, certificado)
        {
        }

        protected NFSeSOAP11ServiceClient(ProviderBase provider, TipoUrl tipoUrl) : base(provider, tipoUrl)
        {
        }

        #endregion Constructors

        #region Methods

        protected virtual string Execute(string soapAction, string message, string responseTag, params string[] soapNamespaces)
        {
            return Execute(soapAction, message, new[] { responseTag }, soapNamespaces);
        }

        protected virtual string Execute(string soapAction, string message, string[] responseTag, params string[] soapNamespaces)
        {
            var request = WriteSoapEnvelope(message, soapNamespaces);

            RemoteCertificateValidationCallback validation = null;
            var naoValidarCertificado = !ValidarCertificadoServidor();

            if (naoValidarCertificado)
            {
                validation = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var ret = string.Empty;

            try
            {
                var requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers["SOAPAction"] = soapAction;

                request.Properties[HttpRequestMessageProperty.Name] = requestMessage;

                lock (serviceLock)
                {
                    var response = Channel.Request(request);
                    Guard.Against<ACBrDFeException>(response == null, "Nenhum retorno do webservice.");
                    var reader = response.GetReaderAtBodyContents();
                    ret = reader.ReadOuterXml();
                }
            }
            finally
            {
                if (naoValidarCertificado)
                    ServicePointManager.ServerCertificateValidationCallback = validation;
            }

            var xmlDocument = XDocument.Parse(ret);
            return TratarRetorno(xmlDocument, responseTag);
        }

        protected virtual Message WriteSoapEnvelope(string message, string[] soapNamespaces)
        {
            var envelope = new StringBuilder();
            envelope.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"");

            foreach (var ns in soapNamespaces)
            {
                envelope.Append($" {ns}");
            }

            envelope.Append(">");
            envelope.Append("<soapenv:Header/>");
            envelope.Append("<soapenv:Body>");
            envelope.Append(message);
            envelope.Append("</soapenv:Body>");
            envelope.Append("</soapenv:Envelope>");

            return Message.CreateMessage(XmlReader.Create(new StringReader(envelope.ToString())), int.MaxValue, Endpoint.Binding.MessageVersion);
        }

        protected virtual bool ValidarCertificadoServidor()
        {
            return true;
        }

        protected abstract string TratarRetorno(XDocument xmlDocument, string[] responseTag);

        #endregion Methods
    }
}