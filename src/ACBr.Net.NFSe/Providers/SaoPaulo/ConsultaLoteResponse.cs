using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "ConsultaLoteResponse", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class ConsultaLoteResponse
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public string RetornoXML;

        public ConsultaLoteResponse()
        {
        }

        public ConsultaLoteResponse(string RetornoXML)
        {
            this.RetornoXML = RetornoXML;
        }
    }
}
