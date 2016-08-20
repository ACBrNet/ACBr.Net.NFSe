using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	[MessageContract(WrapperName = "CancelarNfseV3", WrapperNamespace = "http://www.ginfes.com.br/", IsWrapped = true)]
	internal class CancelarNfseRequest : GinfesBaseRequest
	{
		#region Constructors

		public CancelarNfseRequest(string cabecalho, string request)
		{
			Cabecalho = cabecalho;
			Request = request;
		}

		#endregion Constructors
	}
}