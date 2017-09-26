using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Betha2
{
    [MessageContract(IsWrapped = false)]
    internal sealed class ConsultarLoteRpsResponse
    {
        #region Constructors

        public ConsultarLoteRpsResponse()
        {
        }

        public ConsultarLoteRpsResponse(ResponseBase response)
        {
            Response = response;
        }

        #endregion Constructors

        #region Properties

        [MessageBodyMember(Name = "ConsultarLoteRpsResponse", Namespace = "http://www.betha.com.br/e-nota-contribuinte-ws", Order = 0)]
        public ResponseBase Response { get; set; }

        #endregion Properties
    }
}