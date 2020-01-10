using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "ConsultarNfsePorRpsRequest", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class ConsultarNfsePorRpsRequest : RequestBase
    {
        #region Constructors

        public ConsultarNfsePorRpsRequest()
        {
        }

        public ConsultarNfsePorRpsRequest(string nfseCabecMsg, string nfseDadosMsg)
        {
            this.nfseCabecMsg = nfseCabecMsg;
            this.nfseDadosMsg = nfseDadosMsg;
        }

        #endregion Constructors
    }
}