using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "ConsultaNFeResponse", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class ConsultaNFeResponse
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public string RetornoXML;

        public ConsultaNFeResponse()
        {
        }

        public ConsultaNFeResponse(string RetornoXML)
        {
            this.RetornoXML = RetornoXML;
        }
    }
}
