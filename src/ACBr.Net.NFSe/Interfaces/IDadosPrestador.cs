using ACBr.Net.NFSe.Nota;

#region COM Interop Attributes

#if COM_INTEROP

using System.Runtime.InteropServices;

#endif

#endregion COM Interop Attributes

namespace ACBr.Net.NFSe.Interfaces
{
	#region COM Interop Attributes

#if COM_INTEROP

	[ComVisible(true)]
	[Guid("74E45476-A554-4CD9-9F04-2BAD7C912F26")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
#endif

	#endregion COM Interop Attributes

	public interface IDadosPrestador
	{
		string ChaveAcesso { get; set; }
		string CpfCnpj { get; set; }
		DadosContato DadosContato { get; }
		Endereco Endereco { get; }
		string FraseSecreta { get; set; }
		string InscricaoMunicipal { get; set; }
		string NomeFantasia { get; set; }
		string RazaoSocial { get; set; }
		string Senha { get; set; }
	}
}