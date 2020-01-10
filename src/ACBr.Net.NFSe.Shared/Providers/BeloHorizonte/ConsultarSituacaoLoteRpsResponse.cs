using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "ConsultarSituacaoLoteRpsResponse", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class ConsultarSituacaoLoteRpsResponse : ResponseBase
    {
        #region Constructors

        public ConsultarSituacaoLoteRpsResponse()
        {
        }

        public ConsultarSituacaoLoteRpsResponse(string outputXML)
        {
            this.outputXML = outputXML;
        }

        #endregion Constructors
    }
}