using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "TesteEnvioLoteRPSResponse", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class TesteEnvioLoteRPSResponse
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public string RetornoXML;

        public TesteEnvioLoteRPSResponse()
        {
        }

        public TesteEnvioLoteRPSResponse(string RetornoXML)
        {
            this.RetornoXML = RetornoXML;
        }
    }
}
