using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	[MessageContract(WrapperName = "ConsultarSituacaoLoteRpsV3", WrapperNamespace = "http://www.ginfes.com.br/", IsWrapped = true)]
	internal class ConsultarSituacaoRequest : GinfesBaseRequest
	{
		#region Constructors

		public ConsultarSituacaoRequest(string cabecalho, string request)
		{
			Cabecalho = cabecalho;
			Request = request;
		}

		#endregion Constructors
	}
}