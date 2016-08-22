using ACBr.Net.DFe.Core.Service;
using System;
using System.Security.Cryptography.X509Certificates;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
	internal sealed class GinfesProdServiceClient : DFeWebserviceBase<IGinfesProdService>, IGinfesProdService, IGinfesServiceClient
	{
		#region Constructors

		public GinfesProdServiceClient(string url, TimeSpan? timeOut = null, X509Certificate2 certificado = null) : base(url, timeOut, certificado)
		{
		}

		#endregion Constructors

		#region Methods

		public string ConsultarSituacao(string cabecalho, string dados)
		{
			return ((IGinfesProdService)this).ConsultarSituacaoLoteRpsV3(cabecalho, dados);
		}

		public string ConsultarLoteRps(string cabecalho, string dados)
		{
			return ((IGinfesProdService)this).ConsultarLoteRpsV3(cabecalho, dados);
		}

		public string ConsultarNfsePorRps(string cabecalho, string dados)
		{
			return ((IGinfesProdService)this).ConsultarNfsePorRpsV3(cabecalho, dados);
		}

		public string ConsultarNfse(string cabecalho, string dados)
		{
			return ((IGinfesProdService)this).ConsultarNfseV3(cabecalho, dados);
		}

		public string CancelarNfse(string cabecalho, string dados)
		{
			return ((IGinfesProdService)this).CancelarNfseV3(cabecalho, dados);
		}

		public string RecepcionarLoteRps(string cabecalho, string dados)
		{
			return ((IGinfesProdService)this).RecepcionarLoteRpsV3(cabecalho, dados);
		}

		#region Interface Methods

		string IGinfesProdService.ConsultarSituacaoLoteRpsV3(string arg0, string arg1)
		{
			return Channel.ConsultarSituacaoLoteRpsV3(arg0, arg1);
		}

		string IGinfesProdService.ConsultarLoteRpsV3(string arg0, string arg1)
		{
			return Channel.ConsultarLoteRpsV3(arg0, arg1);
		}

		string IGinfesProdService.ConsultarNfsePorRpsV3(string arg0, string arg1)
		{
			return Channel.ConsultarNfsePorRpsV3(arg0, arg1);
		}

		string IGinfesProdService.ConsultarNfseV3(string arg0, string arg1)
		{
			return Channel.ConsultarNfseV3(arg0, arg1);
		}

		string IGinfesProdService.CancelarNfseV3(string arg0, string arg1)
		{
			return Channel.CancelarNfseV3(arg0, arg1);
		}

		string IGinfesProdService.RecepcionarLoteRpsV3(string arg0, string arg1)
		{
			return Channel.RecepcionarLoteRpsV3(arg0, arg1);
		}

		#endregion Interface Methods

		#endregion Methods
	}
}