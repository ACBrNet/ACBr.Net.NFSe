using System.ServiceModel;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	internal class GinfesBaseRequest
	{
		#region Propriedades

		[MessageBodyMember(Name = "args0", Namespace = "", Order = 0)]
		public string Cabecalho;

		[MessageBodyMember(Name = "args1", Namespace = "", Order = 1)]
		public string Request;

		#endregion Propriedades
	}
}