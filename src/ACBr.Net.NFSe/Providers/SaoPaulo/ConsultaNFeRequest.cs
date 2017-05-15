using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "ConsultaNFeRequest", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class ConsultaNFeRequest
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public int VersaoSchema;

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 1)]
        public string MensagemXML;

        public ConsultaNFeRequest()
        {
        }

        public ConsultaNFeRequest(int VersaoSchema, string MensagemXML)
        {
            this.VersaoSchema = VersaoSchema;
            this.MensagemXML = MensagemXML;
        }
    }
}
