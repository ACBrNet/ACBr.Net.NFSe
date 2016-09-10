using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Interfaces;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;
using System;

#region COM Interop Attributes

#if COM_INTEROP
using System.Runtime.InteropServices;
#endif

#endregion COM Interop Attributes

namespace ACBr.Net.NFSe
{
	#region COM Interop Attributes

#if COM_INTEROP

	[ComVisible(true)]
	[Guid("16145F89-1243-46EB-8628-26885CFC3DBC")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
#endif

	#endregion COM Interop Attributes

	public interface IACBrNFSe
	{
		ConfiguracoesNFSe Configuracoes { get; }

		IDANFSe DaNfSe { get; set; }

		NotaFiscalCollection NotasFiscais { get; }

		RetornoWebservice CancelaNFSe(int lote);

		RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo);

		RetornoWebservice ConsultaNFSe(DateTime inicio, DateTime fim, string numeroNfse = "", int pagina = 1, string cnpjTomador = "", string imTomador = "", string nomeInter = "", string cnpjInter = "", string imInter = "", string serie = "");

		RetornoWebservice ConsultaNFSeRps(string numero, string serie, TipoRps tipo);

		RetornoWebservice ConsultarLoteRps(string protocolo, int lote);

		RetornoWebservice ConsultarSequencialRps(string serie);

		RetornoWebservice ConsultarSituacao(int lote, string protocolo = "");

		RetornoWebservice Enviar(int lote, bool sincrono = false, bool imprimir = false);

		RetornoWebservice SubstituirNFSe(string codigoCancelamento, string numeroNFSe, string motivo);
	}
}