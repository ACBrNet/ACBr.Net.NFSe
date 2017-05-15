using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.SaoPaulo
{
    [MessageContractAttribute(WrapperName = "ConsultaCNPJRequest", WrapperNamespace = "http://www.prefeitura.sp.gov.br/nfe", IsWrapped = true)]
    public partial class ConsultaCNPJRequest
    {

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 0)]
        public int VersaoSchema;

        [MessageBodyMemberAttribute(Namespace = "http://www.prefeitura.sp.gov.br/nfe", Order = 1)]
        public string MensagemXML;

        public ConsultaCNPJRequest()
        {
        }

        public ConsultaCNPJRequest(int VersaoSchema, string MensagemXML)
        {
            this.VersaoSchema = VersaoSchema;
            this.MensagemXML = MensagemXML;
        }
    }
}
