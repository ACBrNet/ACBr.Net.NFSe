using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	[MessageContract(WrapperName = "ConsultarLoteRpsV3", WrapperNamespace = "http://www.ginfes.com.br/", IsWrapped = true)]
	internal class ConsultarLoteRequest : GinfesBaseRequest
	{
		#region Constructors

		public ConsultarLoteRequest(string cabecalho, string request)
		{
			Cabecalho = cabecalho;
			Request = request;
		}

		#endregion Constructors
	}
}