using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	[MessageContract(WrapperName = "RecepcionarLoteRpsV3", WrapperNamespace = "http://www.ginfes.com.br/", IsWrapped = true)]
	internal class RecepcionarLoteRpsRequest : GinfesBaseRequest
	{
		#region Constructors

		public RecepcionarLoteRpsRequest(string cabecalho, string dados)
		{
			Cabecalho = cabecalho;
			Request = dados;
		}

		#endregion Constructors
	}
}