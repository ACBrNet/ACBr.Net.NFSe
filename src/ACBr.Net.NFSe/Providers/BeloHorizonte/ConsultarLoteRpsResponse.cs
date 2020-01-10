using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "ConsultarLoteRpsResponse", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class ConsultarLoteRpsResponse : ResponseBase
    {
        #region Constructors

        public ConsultarLoteRpsResponse()
        {
        }

        public ConsultarLoteRpsResponse(string outputXML)
        {
            this.outputXML = outputXML;
        }

        #endregion Constructors
    }
}