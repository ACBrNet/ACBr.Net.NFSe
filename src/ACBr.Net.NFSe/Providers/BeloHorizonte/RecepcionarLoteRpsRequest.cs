using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "RecepcionarLoteRpsRequest", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class RecepcionarLoteRpsRequest : RequestBase
    {
        #region Constructors

        public RecepcionarLoteRpsRequest()
        {
        }

        public RecepcionarLoteRpsRequest(string nfseCabecMsg, string nfseDadosMsg)
        {
            this.nfseCabecMsg = nfseCabecMsg;
            this.nfseDadosMsg = nfseDadosMsg;
        }

        #endregion Constructors
    }
}