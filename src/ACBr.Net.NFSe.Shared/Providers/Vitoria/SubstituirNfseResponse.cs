using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Vitoria
{
    [MessageContract(IsWrapped = false)]
    public partial class SubstituirNfseResponse
    {
        [MessageBodyMember(Name = "SubstituirNfseResponse", Namespace = "http://www.abrasf.org.br/nfse.xsd", Order = 0)]
        public SubstituirNfseResponseBody Body;

        public SubstituirNfseResponse()
        {
        }

        public SubstituirNfseResponse(SubstituirNfseResponseBody Body)
        {
            this.Body = Body;
        }
    }
}