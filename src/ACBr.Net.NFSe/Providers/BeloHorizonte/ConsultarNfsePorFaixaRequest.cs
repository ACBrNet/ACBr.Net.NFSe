using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "ConsultarNfsePorFaixaRequest", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class ConsultarNfsePorFaixaRequest : RequestBase
    {
        #region Constructors

        public ConsultarNfsePorFaixaRequest()
        {
        }

        public ConsultarNfsePorFaixaRequest(string nfseCabecMsg, string nfseDadosMsg)
        {
            this.nfseCabecMsg = nfseCabecMsg;
            this.nfseDadosMsg = nfseDadosMsg;
        }

        #endregion Constructors
    }
}