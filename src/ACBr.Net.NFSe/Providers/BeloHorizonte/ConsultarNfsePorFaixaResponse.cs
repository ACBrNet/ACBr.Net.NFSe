using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.BeloHorizonte
{
    [MessageContract(WrapperName = "ConsultarNfsePorFaixaResponse", WrapperNamespace = "http://ws.bhiss.pbh.gov.br", IsWrapped = true)]
    internal sealed class ConsultarNfsePorFaixaResponse : ResponseBase
    {
        #region Constructors

        public ConsultarNfsePorFaixaResponse()
        {
        }

        public ConsultarNfsePorFaixaResponse(string outputXML)
        {
            this.outputXML = outputXML;
        }

        #endregion Constructors
    }
}