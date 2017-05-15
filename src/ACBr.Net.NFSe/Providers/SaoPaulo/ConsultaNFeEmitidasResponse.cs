using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "ConsultaNFeEmitidasResponse", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class ConsultaNFeEmitidasResponse
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public string RetornoXML;

        public ConsultaNFeEmitidasResponse()
        {
        }

        public ConsultaNFeEmitidasResponse(string RetornoXML)
        {
            this.RetornoXML = RetornoXML;
        }
    }
}
