using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "EnvioRPSResponse", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class EnvioRPSResponse
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public string RetornoXML;

        public EnvioRPSResponse()
        {
        }

        public EnvioRPSResponse(string RetornoXML)
        {
            this.RetornoXML = RetornoXML;
        }
    }
}
