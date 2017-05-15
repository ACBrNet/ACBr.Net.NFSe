using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "ConsultaLoteRequest", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class ConsultaLoteRequest
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public int VersaoSchema;

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 1)]
        public string MensagemXML;

        public ConsultaLoteRequest()
        {
        }

        public ConsultaLoteRequest(int VersaoSchema, string MensagemXML)
        {
            this.VersaoSchema = VersaoSchema;
            this.MensagemXML = MensagemXML;
        }
    }
}
