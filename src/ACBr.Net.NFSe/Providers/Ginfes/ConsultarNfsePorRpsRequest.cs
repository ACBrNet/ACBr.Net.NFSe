using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	[MessageContract(WrapperName = "ConsultarNfsePorRpsV3", WrapperNamespace = "http://www.ginfes.com.br/", IsWrapped = true)]
	internal class ConsultarNfsePorRpsRequest : GinfesBaseRequest
	{
		#region Constructors

		public ConsultarNfsePorRpsRequest(string cabecalho, string request)
		{
			Cabecalho = cabecalho;
			Request = request;
		}

		#endregion Constructors
	}
}