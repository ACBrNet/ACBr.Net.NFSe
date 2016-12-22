using ACBr.Net.Core;
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ACBr.Net.NFSe.Proxy.Json
{
	[ComVisible(true)]
	[ProgId(nameof(ACBrNFSeJsonProxy))]
	[ClassInterface(ClassInterfaceType.AutoDual)]
	// ReSharper disable once InconsistentNaming
	public class ACBrNFSeJsonProxy
	{
		#region Fields

		private readonly ACBrNFSe oACBrNFSe;

		#endregion Fields

		#region Constructors

		public ACBrNFSeJsonProxy()
		{
			oACBrNFSe = new ACBrNFSe();
		}

		#endregion Constructors

		#region Propriedades

		/// <summary>
		/// Retorna a versão da Lib ACBr.Net.NFSe
		/// </summary>
		/// <value>The versao.</value>
		public string Versao
		{
			get
			{
				var asm = typeof(ACBrNFSe).Assembly;
				var versionInfo = FileVersionInfo.GetVersionInfo(asm.Location);
				return versionInfo.FileVersion;
			}
		}

		/// <summary>
		/// Retorna a versão da Lib ACBr.Net.DFe.Core
		/// </summary>
		/// <value>The versao d fe.</value>
		public string VersaoDFe
		{
			get
			{
				var asm = typeof(DFeSerializer).Assembly;
				var versionInfo = FileVersionInfo.GetVersionInfo(asm.Location);
				return versionInfo.FileVersion;
			}
		}

		/// <summary>
		/// Retorna a versão da Lib ACBr.Net.Core
		/// </summary>
		/// <value>The versao core.</value>
		public string VersaoCore
		{
			get
			{
				var asm = typeof(ACBrComponent).Assembly;
				var versionInfo = FileVersionInfo.GetVersionInfo(asm.Location);
				return versionInfo.FileVersion;
			}
		}

		/// <summary>
		/// Retorna a versão da Lib ACBr.Net.Core
		/// </summary>
		/// <value>The versao core.</value>
		public string VersaoProxy
		{
			get
			{
				var asm = typeof(ACBrNFSeJsonProxy).Assembly;
				var versionInfo = FileVersionInfo.GetVersionInfo(asm.Location);
				return versionInfo.FileVersion;
			}
		}

		/// <summary>
		/// Retrona a mesangem de retorno do servidor.
		/// </summary>
		/// <value>The mensagem retorno.</value>
		public RetornoWebservice MensagemRetorno { get; private set; }

		#endregion Propriedades

		#region Methods

		#region Setup do Componente

		public bool SetupWebService(int codigoMunicipio, int tipoAmbiente, string certificado, string senha, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				oACBrNFSe.Configuracoes.WebServices.CodigoMunicipio = codigoMunicipio;
				oACBrNFSe.Configuracoes.WebServices.Ambiente = (DFeTipoAmbiente)tipoAmbiente;
				oACBrNFSe.Configuracoes.Certificados.Certificado = certificado;
				oACBrNFSe.Configuracoes.Certificados.Senha = senha;

				// Ajusta as particularidades de cada provedor.
				var provider = ProviderManager.GetProvider(oACBrNFSe.Configuracoes);
				switch (provider.Name.ToUpper())
				{
					case "GINFES":
						// Provedor Ginfes em ambiente de Homologação não aceita acentos. (Ambiente de Produção aceita normalmente.)
						if (oACBrNFSe.Configuracoes.WebServices.Ambiente == DFeTipoAmbiente.Homologacao)
							oACBrNFSe.Configuracoes.Geral.RetirarAcentos = true;
						break;

					default:
						break;
				}
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
			return true;
		}

		public bool SetupPrestador(string cpfCnpj, string inscricaoMunicipal,
								   string razaoSocial, string nomeFantasia,
								   string tipoLogradouro, string logradouro, string numero, string complemento, string bairro,
								   int codigoMunicipio, string nomeMunicipio, string uf, string cep,
								   string ddd, string telefone, string email, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				oACBrNFSe.Configuracoes.PrestadorPadrao.CpfCnpj = cpfCnpj;
				oACBrNFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal = inscricaoMunicipal;
				oACBrNFSe.Configuracoes.PrestadorPadrao.RazaoSocial = razaoSocial;
				oACBrNFSe.Configuracoes.PrestadorPadrao.NomeFantasia = nomeFantasia;

				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.TipoLogradouro = tipoLogradouro;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.Logradouro = logradouro;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.Numero = numero;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.Complemento = complemento;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.Bairro = bairro;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio = codigoMunicipio;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.Municipio = nomeMunicipio;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.Uf = uf;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.Cep = cep;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.CodigoPais = 1058;
				oACBrNFSe.Configuracoes.PrestadorPadrao.Endereco.Pais = "BRASIL";

				oACBrNFSe.Configuracoes.PrestadorPadrao.DadosContato.DDD = ddd;
				oACBrNFSe.Configuracoes.PrestadorPadrao.DadosContato.Telefone = telefone;
				oACBrNFSe.Configuracoes.PrestadorPadrao.DadosContato.Email = email;
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
			return true;
		}

		public bool SetupArquivos(bool salvar, string pathLote, string pathRps, string pathNFSe, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				oACBrNFSe.Configuracoes.Arquivos.Salvar = salvar;
				oACBrNFSe.Configuracoes.Arquivos.PathLote = pathLote;
				oACBrNFSe.Configuracoes.Arquivos.PathNFSe = pathNFSe;
				oACBrNFSe.Configuracoes.Arquivos.PathRps = pathRps;
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
			return true;
		}

		public bool Finalizar(ref string mensagemAlerta, ref string mensagemErro)
		{
			oACBrNFSe.Dispose();
			return true;
		}

		#endregion Setup do Componente

		#region Métodos para montar o RPS

		public bool ClearNotas(ref string mensagemErro)
		{
			try
			{
				oACBrNFSe.NotasFiscais.Clear();
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
			return true;
		}

		public bool AddRPS(string rps, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				var nfse = JsonConvert.DeserializeObject<NotaFiscal>(rps);
				oACBrNFSe.NotasFiscais.Add(nfse);
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
			return true;
		}

		#endregion Métodos para montar o RPS

		#region Webservices

		public bool EnviarLote(int numeroLote, bool sincrono, bool imprimir, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				MensagemRetorno = oACBrNFSe.Enviar(numeroLote, sincrono, imprimir);
				return TrataMensagemRetornoWebservice(ref mensagemAlerta, ref mensagemErro);
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
		}

		public bool ConsultarSituacaoLote(int numeroLote, string protocolo, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				MensagemRetorno = oACBrNFSe.ConsultarSituacao(numeroLote, protocolo);
				return TrataMensagemRetornoWebservice(ref mensagemAlerta, ref mensagemErro);
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
		}

		public bool ConsultarLote(int numeroLote, string protocolo, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				MensagemRetorno = oACBrNFSe.ConsultarLoteRps(numeroLote, protocolo);
				return TrataMensagemRetornoWebservice(ref mensagemAlerta, ref mensagemErro);
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
		}

		public bool CancelarNFSe(string codigoCancelamento, string numeroNFSe, string motivoCancelamento, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				MensagemRetorno = oACBrNFSe.CancelaNFSe(codigoCancelamento, numeroNFSe, motivoCancelamento);
				return TrataMensagemRetornoWebservice(ref mensagemAlerta, ref mensagemErro);
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
		}

		public bool ConsultaNFSeRps(string numeroRps, string serieRps, int tipoRps, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				MensagemRetorno = oACBrNFSe.ConsultaNFSeRps(numeroRps, serieRps, (TipoRps)tipoRps);
				return TrataMensagemRetornoWebservice(ref mensagemAlerta, ref mensagemErro);
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
		}

		public bool ConsultaNFSePorPeriodo(DateTime dataInicial, DateTime dataFinal, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				MensagemRetorno = oACBrNFSe.ConsultaNFSe(dataInicial, dataFinal);
				return TrataMensagemRetornoWebservice(ref mensagemAlerta, ref mensagemErro);
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
		}

		public bool ConsultaNFSePorNumero(string numeroNotaFiscal, string serieNotaFiscal, ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				MensagemRetorno = oACBrNFSe.ConsultaNFSe(null, null, numeroNotaFiscal, 0, "", "", "", "", "", serieNotaFiscal);
				return TrataMensagemRetornoWebservice(ref mensagemAlerta, ref mensagemErro);
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return false;
			}
		}

		#endregion Webservices

		#region Métodos de Apoio

		public string SelecionarCertificado(ref string mensagemAlerta, ref string mensagemErro)
		{
			try
			{
				return oACBrNFSe.Configuracoes.Certificados.SelecionarCertificado();
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mensagemErro += ex.Message + Environment.NewLine;
					ex = ex.InnerException;
				}
				return "";
			}
		}

		#endregion Métodos de Apoio

		#region Métodos privados

		private bool TrataMensagemRetornoWebservice(ref string mensagemAlerta, ref string mensagemErro)
		{
			if (MensagemRetorno.Alertas.Count > 0)
			{
				var builderAlertas = new System.Text.StringBuilder();
				foreach (var alerta in MensagemRetorno.Alertas)
				{
					builderAlertas.Append("[" + alerta.Codigo.ToString() + "] " + alerta.Descricao + " / " + alerta.Correcao);
				}
				if (builderAlertas.Length > 0)
				{
					mensagemAlerta = builderAlertas.ToString();
				}
			}
			if (MensagemRetorno.Erros.Count > 0)
			{
				var builderErros = new System.Text.StringBuilder();
				foreach (var erro in MensagemRetorno.Erros)
				{
					builderErros.Append("[" + erro.Codigo.ToString() + "] " + erro.Descricao + " / " + erro.Correcao);
				}
				if (builderErros.Length == 0 & MensagemRetorno.Sucesso == false)
				{
					builderErros.Append("Ocorreu um erro não identificado.");
				}
				if (builderErros.Length > 0)
				{
					mensagemErro = builderErros.ToString();
				}
			}
			// Consideramos o retorno falso, quando não conseguimos pegar o Xml de Retorno do Webservice
			// Por exemplo:
			// FALSO: Se o usuário não digitar a senha do certificado, irá retornar falso.
			// E neste caso, a aplicação não deve fazer nada de diferente, a não ser, uma nova tentativa de consumir o webservice.
			// VERDADEIRO: Mas se houve o retorno do Webservice, sempre consideraremos como verdadeiro o retorno.
			// Ainda que o webservice devolveu uma mensagem de erro, será verdadeiro pois o webservice foi consumido.
			// E neste caso, a aplicação que está consumindo o componente ACBR.NET.NFSE deve identificar a ação a ser tomada (por exemplo, corrigir os dados de um RPS enviado).
			if (string.IsNullOrWhiteSpace(MensagemRetorno.XmlRetorno))
				return false;
			else
				return true;
		}

		#endregion Métodos privados

		#endregion Methods
	}
}