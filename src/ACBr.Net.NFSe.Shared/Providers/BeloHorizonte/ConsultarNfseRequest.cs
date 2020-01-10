using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "ConsultarNfseRequest", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class ConsultarNfseRequest : RequestBase
    {
        #region Constructors

        public ConsultarNfseRequest()
        {
        }

        public ConsultarNfseRequest(string nfseCabecMsg, string nfseDadosMsg)
        {
            this.nfseCabecMsg = nfseCabecMsg;
            this.nfseDadosMsg = nfseDadosMsg;
        }

        #endregion Constructors
    }
}