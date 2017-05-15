using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "CancelamentoNFeResponse", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class CancelamentoNFeResponse
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public string RetornoXML;

        public CancelamentoNFeResponse()
        {
        }

        public CancelamentoNFeResponse(string RetornoXML)
        {
            this.RetornoXML = RetornoXML;
        }
    }
}
