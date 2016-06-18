using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers
{
	public class Evento
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Evento"/> class.
		/// </summary>
		public Evento()
		{
			IdentificacaoNfse = new IdentificacaoNfse();
			IdentificacaoRps = new IdentificacaoRps();
		}

		#endregion Constructor

		#region Propriedades
		
		public short Codigo { get; set; }
		
		public string Descricao { get; set; }
		
		public IdentificacaoRps IdentificacaoRps { get; set; }
		
		public IdentificacaoNfse IdentificacaoNfse { get; set; }
		
		#endregion Propriedades
	}
}