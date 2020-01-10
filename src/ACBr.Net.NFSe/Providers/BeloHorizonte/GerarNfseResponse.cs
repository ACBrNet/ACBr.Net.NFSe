using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "GerarNfseResponse", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class GerarNfseResponse : ResponseBase
    {
        #region Constructors

        public GerarNfseResponse()
        {
        }

        public GerarNfseResponse(string outputXML)
        {
            this.outputXML = outputXML;
        }

        #endregion Constructors
    }
}