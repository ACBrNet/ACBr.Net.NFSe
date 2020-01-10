using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "ConsultarNfsePorRpsResponse", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class ConsultarNfsePorRpsResponse : ResponseBase
    {
        #region Constructors

        public ConsultarNfsePorRpsResponse()
        {
        }

        public ConsultarNfsePorRpsResponse(string outputXML)
        {
            this.outputXML = outputXML;
        }

        #endregion Constructors
    }
}