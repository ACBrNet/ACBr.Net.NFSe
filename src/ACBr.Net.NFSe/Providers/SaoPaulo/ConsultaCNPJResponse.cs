using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "ConsultaCNPJResponse", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class ConsultaCNPJResponse
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public string RetornoXML;

        public ConsultaCNPJResponse()
        {
        }

        public ConsultaCNPJResponse(string RetornoXML)
        {
            this.RetornoXML = RetornoXML;
        }
    }
}
