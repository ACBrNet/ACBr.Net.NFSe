using System.Runtime.Serialization;

namespace ACBr.Net.NFSe.Providers.Vitoria
{
    [DataContract(Namespace = "http://www.abrasf.org.br/nfse.xsd")]
    public partial class SubstituirNfseRequestBody
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string mensagemXML;

        public SubstituirNfseRequestBody()
        {
        }

        public SubstituirNfseRequestBody(string mensagemXML)
        {
            this.mensagemXML = mensagemXML;
        }
    }
}