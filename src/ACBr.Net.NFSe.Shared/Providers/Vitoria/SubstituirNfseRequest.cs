using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Vitoria
{
    [MessageContract(IsWrapped = false)]
    public partial class SubstituirNfseRequest
    {
        [MessageBodyMember(Name = "SubstituirNfse", Namespace = "http://www.abrasf.org.br/nfse.xsd", Order = 0)]
        public SubstituirNfseRequestBody Body;

        public SubstituirNfseRequest()
        {
        }

        public SubstituirNfseRequest(SubstituirNfseRequestBody Body)
        {
            this.Body = Body;
        }
    }
}