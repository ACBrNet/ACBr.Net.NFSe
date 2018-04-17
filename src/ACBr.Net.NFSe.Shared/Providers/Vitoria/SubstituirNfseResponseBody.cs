using System.Runtime.Serialization;

namespace ACBr.Net.NFSe.Providers.Vitoria
{
    [DataContract(Namespace = "http://www.abrasf.org.br/nfse.xsd")]
    public partial class SubstituirNfseResponseBody
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string SubstituirNfseResult;

        public SubstituirNfseResponseBody()
        {
        }

        public SubstituirNfseResponseBody(string SubstituirNfseResult)
        {
            this.SubstituirNfseResult = SubstituirNfseResult;
        }
    }
}