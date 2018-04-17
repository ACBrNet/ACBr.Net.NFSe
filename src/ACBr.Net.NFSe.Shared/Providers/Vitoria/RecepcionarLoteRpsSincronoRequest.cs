using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Vitoria
{
    [MessageContract(IsWrapped = false)]
    public partial class RecepcionarLoteRpsSincronoRequest
    {
        [MessageBodyMember(Name = "RecepcionarLoteRpsSincrono", Namespace = "http://www.abrasf.org.br/nfse.xsd", Order = 0)]
        public RecepcionarLoteRpsSincronoRequestBody Body;

        public RecepcionarLoteRpsSincronoRequest()
        {
        }

        public RecepcionarLoteRpsSincronoRequest(RecepcionarLoteRpsSincronoRequestBody Body)
        {
            this.Body = Body;
        }
    }
}