using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	[MessageContract(WrapperName = "ConsultarNfseV3", WrapperNamespace = "http://www.ginfes.com.br/", IsWrapped = true)]
	internal class ConsultarNfseRequest : GinfesBaseRequest
	{
		#region Constructors

		public ConsultarNfseRequest(string cabecalho, string request)
		{
			Cabecalho = cabecalho;
			Request = request;
		}

		#endregion Constructors
	}
}