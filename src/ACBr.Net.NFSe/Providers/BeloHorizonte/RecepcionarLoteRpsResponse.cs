using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "RecepcionarLoteRpsResponse", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class RecepcionarLoteRpsResponse : ResponseBase
    {
        #region Constructors

        public RecepcionarLoteRpsResponse()
        {
        }

        public RecepcionarLoteRpsResponse(string outputXML)
        {
            this.outputXML = outputXML;
        }

        #endregion Constructors
    }
}