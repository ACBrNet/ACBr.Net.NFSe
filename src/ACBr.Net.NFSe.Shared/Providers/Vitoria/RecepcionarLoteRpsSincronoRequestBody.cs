using System.Runtime.Serialization;

namespace ACBr.Net.NFSe.Providers.Vitoria
{
    [DataContract(Namespace = "http://www.abrasf.org.br/nfse.xsd")]
    public partial class RecepcionarLoteRpsSincronoRequestBody
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string mensagemXML;

        public RecepcionarLoteRpsSincronoRequestBody()
        {
        }

        public RecepcionarLoteRpsSincronoRequestBody(string mensagemXML)
        {
            this.mensagemXML = mensagemXML;
        }
    }
}