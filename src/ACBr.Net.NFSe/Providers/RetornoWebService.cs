using System;
using System.Collections.Generic;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers
{
	public class RetornoWebService
	{
		#region Constructor
		
		internal RetornoWebService()
		{
			DataEnvioLote = DateTime.Now;
			Erros = new List<Evento>();
			Alertas = new List<Evento>();
		}

		#endregion Constructor

		#region Propriedades

		public int CodCidade { get; set; }
		
		public bool Sucesso { get; set; }
		
		public string NumeroLote { get; set; }

		public string CPFCNPJRemetente { get; set; }

		public DateTime DataEnvioLote { get; set; }
		
		public long Versao { get; set; }

		public bool Assincrono { get; set; }

		public List<NotaFiscal> NotasFiscais { get; }
		
		public List<Evento> Alertas { get; }

		public List<Evento> Erros { get; }

		#endregion Propriedades
	}
}