using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "ConsultaNFeRecebidasResponse", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class ConsultaNFeRecebidasResponse
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public string RetornoXML;

        public ConsultaNFeRecebidasResponse()
        {
        }

        public ConsultaNFeRecebidasResponse(string RetornoXML)
        {
            this.RetornoXML = RetornoXML;
        }
    }
}
