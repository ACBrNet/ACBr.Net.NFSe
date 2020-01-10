using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "CancelarNfseRequest", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class CancelarNfseRequest : RequestBase
    {
        #region Constructors

        public CancelarNfseRequest()
        {
        }

        public CancelarNfseRequest(string nfseCabecMsg, string nfseDadosMsg)
        {
            this.nfseCabecMsg = nfseCabecMsg;
            this.nfseDadosMsg = nfseDadosMsg;
        }

        #endregion Constructors
    }
}