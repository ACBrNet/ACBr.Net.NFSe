using System.Runtime.Serialization;

namespace ACBr.Net.NFSe.Providers.Vitoria
{
    [DataContract(Namespace = "http://www.abrasf.org.br/nfse.xsd")]
    public partial class RecepcionarLoteRpsSincronoResponseBody
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string RecepcionarLoteRpsSincronoResult;

        public RecepcionarLoteRpsSincronoResponseBody()
        {
        }

        public RecepcionarLoteRpsSincronoResponseBody(string RecepcionarLoteRpsSincronoResult)
        {
            this.RecepcionarLoteRpsSincronoResult = RecepcionarLoteRpsSincronoResult;
        }
    }
}