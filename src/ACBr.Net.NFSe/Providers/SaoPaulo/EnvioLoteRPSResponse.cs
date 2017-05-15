using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "EnvioLoteRPSResponse", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class EnvioLoteRPSResponse
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public string RetornoXML;

        public EnvioLoteRPSResponse()
        {
        }

        public EnvioLoteRPSResponse(string RetornoXML)
        {
            this.RetornoXML = RetornoXML;
        }
    }
}
