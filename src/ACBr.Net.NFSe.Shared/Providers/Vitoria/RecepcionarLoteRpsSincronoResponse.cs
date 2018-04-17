using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Vitoria
{
    [MessageContract(IsWrapped = false)]
    public partial class RecepcionarLoteRpsSincronoResponse
    {
        [MessageBodyMember(Name = "RecepcionarLoteRpsSincronoResponse", Namespace = "http://www.abrasf.org.br/nfse.xsd", Order = 0)]
        public RecepcionarLoteRpsSincronoResponseBody Body;

        public RecepcionarLoteRpsSincronoResponse()
        {
        }

        public RecepcionarLoteRpsSincronoResponse(RecepcionarLoteRpsSincronoResponseBody Body)
        {
            this.Body = Body;
        }
    }
}