using ACBr.Net.Core;
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;
using System;
using System.Runtime.InteropServices;

namespace ACBr.Net.NFSe
{
    [ComVisible(true)]
    [ProgId(nameof(ACBrNFSeProxy))]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ACBrNFSeProxy
    {
        #region Fields

        private readonly ACBrNFSe oACBrNFSe;
        private NotaFiscal NFSe;

        #endregion Fields

        #region Constructors

        public ACBrNFSeProxy()
        {
            oACBrNFSe = new ACBrNFSe();
        }

        #endregion Constructors

        #region Propriedades

        public string VersaoProxy => "ACBR.NET.NFSE 1.00";

        public string VersaoNFSe
        {
            get
            {
                var asm = typeof(ACBrNFSe).Assembly;
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(asm.Location);
                return versionInfo.FileVersion;
            }
        }

        public string VersaoDFe
        {
            get
            {
                var asm = typeof(DFe.Core.Serializer.DFeSerializer).Assembly;
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(asm.Location);
                return versionInfo.FileVersion;
            }
        }
        public string VersaoCore
        {
            get
            {
                var asm = typeof(ACBrComponent).Assembly;
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(asm.Location);
                return versionInfo.FileVersion;
            }
        }



        public RetornoWebservice MensagemRetorno { get; private set; }

        #endregion Propriedades

        #region Methods

        #region Setup do Componente

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

        public bool SetupWebService(int codigoMunicipio, int tipoAmbiente, string certificado, string senha, ref string mensagemAlerta, ref string mensagemErro)
        {
            try
            {
                oACBrNFSe.Configuracoes.WebServices.CodigoMunicipio = codigoMunicipio;
                oACBrNFSe.Configuracoes.WebServices.Ambiente = (DFeTipoAmbiente)tipoAmbiente;
                oACBrNFSe.Configuracoes.Certificados.Certificado = certificado;
                oACBrNFSe.Configuracoes.Certificados.Senha = senha;
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

        public bool Finalizar(ref string mensagemAlerta, ref string mensagemErro)
        {
            oACBrNFSe.Dispose();
            return true;
        }

        #endregion Setup do Componente

        #region Métodos para montar o RPS

        public bool RPS(string numeroRPS, string serieRPS, int tipoRPS, DateTime dataEmissaoRPS, int situacaoRPS,
                        string numeroRPSSubstituido, string serieRPSSubstituido, int tipoRPSSubstituido,
                        int naturezaOperacao, int regimeEspecialTributacao, int incentivadorCultural, ref string mensagemAlerta, ref string mensagemErro)
        {
            try
            {
                NFSe = oACBrNFSe.NotasFiscais.AddNew();
                NFSe.IdentificacaoRps.Numero = numeroRPS;
                NFSe.IdentificacaoRps.Serie = serieRPS;
                NFSe.IdentificacaoRps.Tipo = (TipoRps)tipoRPS;
                NFSe.IdentificacaoRps.DataEmissao = dataEmissaoRPS;
                NFSe.Situacao = (SituacaoNFSeRps)situacaoRPS;
                if (!string.IsNullOrWhiteSpace(numeroRPSSubstituido))
                {
                    NFSe.RpsSubstituido.NumeroRps = numeroRPSSubstituido;
                    NFSe.RpsSubstituido.Serie = serieRPSSubstituido;
                    NFSe.RpsSubstituido.Tipo = (TipoRps)tipoRPSSubstituido;
                }
                NFSe.NaturezaOperacao = (NaturezaOperacao)naturezaOperacao;
                NFSe.RegimeEspecialTributacao = (RegimeEspecialTributacao)regimeEspecialTributacao;
                NFSe.IncentivadorCultural = (NFSeSimNao)incentivadorCultural;
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

        public bool RPSServico(string itemListaServico, string codigoTributacaoMunicipio, string codigoCNAE, int codigoMunicipio,
                               string discriminacao, ref string mensagemAlerta, ref string mensagemErro)
        {
            try
            {
                NFSe.Servico.ItemListaServico = itemListaServico;
                NFSe.Servico.CodigoTributacaoMunicipio = codigoTributacaoMunicipio;
                NFSe.Servico.CodigoCnae = codigoCNAE;
                NFSe.Servico.CodigoMunicipio = codigoMunicipio;
                NFSe.Servico.Discriminacao = discriminacao;
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

        public bool RPSValores(decimal valorServicos, decimal valorDeducoes, decimal valorPis, decimal valorCofins,
                               decimal valorInss, decimal valorIr, decimal valorCsll, int issRetido, decimal valorIss,
                               decimal valorOutrasRetencoes, decimal valorBaseCalculo, decimal aliquota, decimal valorLiquidoNFSe,
                               decimal valorIssRetido, decimal valorDescontoCondicionado, decimal valorDescontoIncondicionado,
                               decimal valorCredito, ref string mensagemAlerta, ref string mensagemErro)
        {
            try
            {
                NFSe.Servico.Valores.ValorServicos = valorServicos;
                NFSe.Servico.Valores.ValorDeducoes = valorDeducoes;
                NFSe.Servico.Valores.ValorPis = valorPis;
                NFSe.Servico.Valores.ValorCofins = valorCofins;
                NFSe.Servico.Valores.ValorInss = valorInss;
                NFSe.Servico.Valores.ValorIr = valorIr;
                NFSe.Servico.Valores.ValorCsll = valorCsll;
                NFSe.Servico.Valores.IssRetido = (SituacaoTributaria)issRetido;
                NFSe.Servico.Valores.ValorIss = valorIss;
                NFSe.Servico.Valores.ValorOutrasRetencoes = valorOutrasRetencoes;
                NFSe.Servico.Valores.BaseCalculo = valorBaseCalculo;
                NFSe.Servico.Valores.Aliquota = aliquota;
                NFSe.Servico.Valores.ValorLiquidoNfse = valorLiquidoNFSe;
                NFSe.Servico.Valores.ValorIssRetido = valorIssRetido;
                NFSe.Servico.Valores.DescontoCondicionado = valorDescontoCondicionado;
                NFSe.Servico.Valores.DescontoIncondicionado = valorDescontoIncondicionado;
                NFSe.ValorCredito = valorCredito;
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

        public bool RPSPrestador(string cpfCnpj, string inscricaoMunicipal,
                              string razaoSocial, string nomeFantasia,
                              string tipoLogradouro, string logradouro, string numero, string complemento, string bairro,
                              int codigoMunicipio, string nomeMunicipio, string uf, string cep,
                              string ddd, string telefone, string email, ref string mensagemAlerta, ref string mensagemErro)
        {
            try
            {
                NFSe.Prestador.CpfCnpj = cpfCnpj;
                NFSe.Prestador.InscricaoMunicipal = inscricaoMunicipal;
                NFSe.Prestador.RazaoSocial = razaoSocial;
                NFSe.Prestador.NomeFantasia = nomeFantasia;

                NFSe.Prestador.Endereco.TipoLogradouro = tipoLogradouro;
                NFSe.Prestador.Endereco.Logradouro = logradouro;
                NFSe.Prestador.Endereco.Numero = numero;
                NFSe.Prestador.Endereco.Complemento = complemento;
                NFSe.Prestador.Endereco.Bairro = bairro;
                NFSe.Prestador.Endereco.CodigoMunicipio = codigoMunicipio;
                NFSe.Prestador.Endereco.Municipio = nomeMunicipio;
                NFSe.Prestador.Endereco.Uf = uf;
                NFSe.Prestador.Endereco.Cep = cep;
                NFSe.Prestador.Endereco.CodigoPais = 1058;
                NFSe.Prestador.Endereco.Pais = "BRASIL";

                NFSe.Prestador.DadosContato.DDD = ddd;
                NFSe.Prestador.DadosContato.Telefone = telefone;
                NFSe.Prestador.DadosContato.Email = email;
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

        public bool RPSTomador(string cpfCnpj, string inscricaoMunicipal,
                              string razaoSocial,
                              string tipoLogradouro, string logradouro, string numero, string complemento, string bairro,
                              int codigoMunicipio, string nomeMunicipio, string uf, string cep,
                              string ddd, string telefone, string email, ref string mensagemAlerta, ref string mensagemErro)
        {
            try
            {
                NFSe.Tomador.CpfCnpj = cpfCnpj;
                NFSe.Tomador.InscricaoMunicipal = inscricaoMunicipal;
                NFSe.Tomador.RazaoSocial = razaoSocial;

                NFSe.Tomador.Endereco.TipoLogradouro = tipoLogradouro;
                NFSe.Tomador.Endereco.Logradouro = logradouro;
                NFSe.Tomador.Endereco.Numero = numero;
                NFSe.Tomador.Endereco.Complemento = complemento;
                NFSe.Tomador.Endereco.Bairro = bairro;
                NFSe.Tomador.Endereco.CodigoMunicipio = codigoMunicipio;
                NFSe.Tomador.Endereco.Municipio = nomeMunicipio;
                NFSe.Tomador.Endereco.Uf = uf;
                NFSe.Tomador.Endereco.Cep = cep;
                NFSe.Tomador.Endereco.CodigoPais = 1058;
                NFSe.Tomador.Endereco.Pais = "BRASIL";

                NFSe.Tomador.DadosContato.DDD = ddd;
                NFSe.Tomador.DadosContato.Telefone = telefone;
                NFSe.Tomador.DadosContato.Email = email;
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

        public bool RPSIntermediario(string cpfCnpj, string inscricaoMunicipal, string razaoSocial, string email, int issRetido, ref string mensagemAlerta, ref string mensagemErro)
        {
            try
            {
                NFSe.Intermediario.CpfCnpj = cpfCnpj;
                NFSe.Intermediario.InscricaoMunicipal = inscricaoMunicipal;
                NFSe.Intermediario.RazaoSocial = razaoSocial;
                NFSe.Intermediario.EMail = email;
                NFSe.Intermediario.IssRetido = (SituacaoTributaria)issRetido;
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

        public bool RPSOrgaoGerador(int codigoMunicipio, string uf, ref string mensagemAlerta, ref string mensagemErro)
        {
            try
            {
                NFSe.OrgaoGerador.CodigoMunicipio = codigoMunicipio;
                NFSe.OrgaoGerador.Uf = uf;
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

        public bool RPSConstrucaoCivil(string codigoObra, string artObra, ref string mensagemAlerta, ref string mensagemErro)
        {
            try
            {
                NFSe.ConstrucaoCivil.CodigoObra = codigoObra;
                NFSe.ConstrucaoCivil.ArtObra = artObra;
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

        public bool ConsultaNFSe(DateTime dataInicial, DateTime dataFinal, ref string mensagemAlerta, ref string mensagemErro)
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