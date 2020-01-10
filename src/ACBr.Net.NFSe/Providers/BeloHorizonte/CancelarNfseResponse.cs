using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "CancelarNfseResponse", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class CancelarNfseResponse : ResponseBase
    {
        #region Constructors

        public CancelarNfseResponse()
        {
        }

        public CancelarNfseResponse(string outputXML)
        {
            this.outputXML = outputXML;
        }

        #endregion Constructors
    }
}