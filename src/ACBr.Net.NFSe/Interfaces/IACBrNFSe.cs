using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;
using System;

namespace ACBr.Net.NFSe.Interfaces
{
	public interface IACBrNFSe
	{
		ConfiguracoesNFSe Configuracoes { get; }

		IDANFSe DaNfSe { get; set; }

		NotaFiscalCollection NotasFiscais { get; }

		RetornoWebservice CancelaNFSe(int lote);

		RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo);

		RetornoWebservice ConsultaNFSe(DateTime? inicio, DateTime? fim, string numeroNfse = "", int pagina = 1, string cnpjTomador = "", string imTomador = "", string nomeInter = "", string cnpjInter = "", string imInter = "", string serie = "");

		RetornoWebservice ConsultaNFSeRps(string numero, string serie, TipoRps tipo);

		RetornoWebservice ConsultarLoteRps(string protocolo, int lote);

		RetornoWebservice ConsultarSequencialRps(string serie);

		RetornoWebservice ConsultarSituacao(int lote, string protocolo = "");

		RetornoWebservice Enviar(int lote, bool sincrono = false, bool imprimir = false);

		RetornoWebservice SubstituirNFSe(string codigoCancelamento, string numeroNFSe, string motivo);
	}
}