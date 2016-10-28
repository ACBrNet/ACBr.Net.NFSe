using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;

namespace ACBr.Net.NFSe
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId(nameof(ACBrNFSeProxy))]
    [ComVisible(true)]
    public class ACBrNFSeProxy
    {
        readonly public string Versao = "ACBr.Net.NFSe 1.00";
        private ACBrNFSe ACBrBFSe = new ACBrNFSe();
        private NotaFiscal NFSe;
        public RetornoWebservice MensagemRetorno;

        #region Setup do Componente
        public void SetupWebService(int codigoMunicipio, int tipoAmbiente, string certificado, string senha)
        {
            ACBrBFSe.Configuracoes.WebServices.CodMunicipio = codigoMunicipio;
            ACBrBFSe.Configuracoes.WebServices.Ambiente = (DFe.Core.Common.TipoAmbiente)tipoAmbiente;
            ACBrBFSe.Configuracoes.Certificados.Certificado = certificado;
            ACBrBFSe.Configuracoes.Certificados.Senha = senha;
        }

        public void SetupPrestador(string cpfCnpj, string inscricaoMunicipal,
                              string razaoSocial, string nomeFantasia,
                              string tipoLogradouro, string logradouro, string numero, string complemento, string bairro,
                              string codigoMunicipio, string nomeMunicipio, string uf, string cep,
                              string telefone, string email)
        {
            ACBrBFSe.Configuracoes.PrestadorPadrao.CpfCnpj = cpfCnpj;
            ACBrBFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal = inscricaoMunicipal;
            ACBrBFSe.Configuracoes.PrestadorPadrao.RazaoSocial = razaoSocial;
            ACBrBFSe.Configuracoes.PrestadorPadrao.NomeFantasia = nomeFantasia;

            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.TipoLogradouro = tipoLogradouro;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Logradouro = logradouro;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Numero = numero;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Complemento = complemento;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Bairro = bairro;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio = codigoMunicipio;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Municipio = nomeMunicipio;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Uf = uf;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Cep = cep;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.CodigoPais = 1058;
            ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Pais = "BRASIL";

            ACBrBFSe.Configuracoes.PrestadorPadrao.DadosContato.Telefone = telefone;
            ACBrBFSe.Configuracoes.PrestadorPadrao.DadosContato.Email = email;
        }
        #endregion Setup do Componente

        #region Métodos para montar o RPS
        public void RPS(string numeroRPS, string serieRPS, int tipoRPS, DateTime dataEmissaoRPS, int situacaoRPS,
                       string numeroRPSSubstituido, string serieRPSSubstituido, int tipoRPSSubstituido,
                       int naturezaOperacao, int regimeEspecialTributacao, int incentivadorCultural)
        {
            NFSe = ACBrBFSe.NotasFiscais.AddNew();
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
            // Por padrão, já define os dados do prestador, utilizando os dados do prestador padrão
            NFSe.Prestador.CpfCnpj = ACBrBFSe.Configuracoes.PrestadorPadrao.CpfCnpj;
            NFSe.Prestador.InscricaoMunicipal = ACBrBFSe.Configuracoes.PrestadorPadrao.InscricaoMunicipal;
            NFSe.Prestador.RazaoSocial = ACBrBFSe.Configuracoes.PrestadorPadrao.RazaoSocial;
            NFSe.Prestador.NomeFantasia = ACBrBFSe.Configuracoes.PrestadorPadrao.NomeFantasia;
            NFSe.Prestador.Endereco.TipoLogradouro = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.TipoLogradouro;
            NFSe.Prestador.Endereco.Logradouro = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Logradouro;
            NFSe.Prestador.Endereco.Numero = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Numero;
            NFSe.Prestador.Endereco.Complemento = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Complemento;
            NFSe.Prestador.Endereco.Bairro = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Bairro;
            NFSe.Prestador.Endereco.CodigoMunicipio = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio;
            NFSe.Prestador.Endereco.Municipio = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Municipio;
            NFSe.Prestador.Endereco.Uf = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Uf;
            NFSe.Prestador.Endereco.Cep = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Cep;
            NFSe.Prestador.Endereco.CodigoPais = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.CodigoPais;
            NFSe.Prestador.Endereco.Pais = ACBrBFSe.Configuracoes.PrestadorPadrao.Endereco.Pais;
            NFSe.Prestador.DadosContato.Telefone = ACBrBFSe.Configuracoes.PrestadorPadrao.DadosContato.Telefone;
            NFSe.Prestador.DadosContato.Email = ACBrBFSe.Configuracoes.PrestadorPadrao.DadosContato.Email;

        }
        public void RPSServico(string itemListaServico, string codigoTributacaoMunicipio, string codigoCNAE, string codigoMunicipio, string discriminacao)
        {
            NFSe.Servico.ItemListaServico = itemListaServico;
            NFSe.Servico.CodigoTributacaoMunicipio = codigoTributacaoMunicipio;
            NFSe.Servico.CodigoCnae = codigoCNAE;
            NFSe.Servico.CodigoMunicipio = codigoMunicipio;
            NFSe.Servico.Discriminacao = discriminacao;
        }
        public void RPSValores(decimal valorServicos, decimal valorDeducoes, decimal valorPis, decimal valorCofins,
                               decimal valorInss, decimal valorIr, decimal valorCsll, int issRetido, decimal valorIss,
                               decimal valorOutrasRetencoes, decimal valorBaseCalculo, decimal aliquota, decimal valorLiquidoNFSe,
                               decimal valorIssRetido, decimal valorDescontoCondicionado, decimal valorDescontoIncondicionado,
                               decimal valorCredito)
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
        public void RPSPrestador(string cpfCnpj, string inscricaoMunicipal,
                              string razaoSocial, string nomeFantasia,
                              string tipoLogradouro, string logradouro, string numero, string complemento, string bairro,
                              string codigoMunicipio, string nomeMunicipio, string uf, string cep,
                              string telefone, string email)
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

            NFSe.Prestador.DadosContato.Telefone = telefone;
            NFSe.Prestador.DadosContato.Email = email;
        }
        public void RPSTomador(string cpfCnpj, string inscricaoMunicipal,
                              string razaoSocial,
                              string tipoLogradouro, string logradouro, string numero, string complemento, string bairro,
                              string codigoMunicipio, string nomeMunicipio, string uf, string cep,
                              string telefone, string email)
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

            NFSe.Tomador.DadosContato.Telefone = telefone;
            NFSe.Tomador.DadosContato.Email = email;
        }
        public void RPSIntermediario(string cpfCnpj, string inscricaoMunicipal, string razaoSocial, string email, int issRetido)
        {
            NFSe.Intermediario.CpfCnpj = cpfCnpj;
            NFSe.Intermediario.InscricaoMunicipal = inscricaoMunicipal;
            NFSe.Intermediario.RazaoSocial = razaoSocial;
            NFSe.Intermediario.EMail = email;
            NFSe.Intermediario.IssRetido = (SituacaoTributaria)issRetido;
        }
        public void RPSOrgaoGerador(string codigoMunicipio, string uf)
        {
            NFSe.OrgaoGerador.CodigoMunicipio = codigoMunicipio;
            NFSe.OrgaoGerador.Uf = uf;
        }
        public void RPSConstrucaoCivil(string codigoObra, string artObra)
        {
            NFSe.ConstrucaoCivil.CodigoObra = codigoObra;
            NFSe.ConstrucaoCivil.Art = artObra;
        }
        #endregion Métodos para montar o RPS

        #region Webservices
        public bool EnviarLote(int numeroLote, bool sincrono, bool imprimir, ref string mensagemAviso, ref string mensagemErro)
        {
            mensagemAviso = "";
            mensagemErro = "";
            MensagemRetorno = ACBrBFSe.Enviar(numeroLote, sincrono, imprimir);
            return TrataMensagemRetorno(ref mensagemAviso, ref mensagemErro);
        }
        public bool ConsultarSituacaoLote(int numeroLote, string protocolo, ref string mensagemAviso, ref string mensagemErro)
        {
            mensagemAviso = "";
            mensagemErro = "";
            MensagemRetorno = ACBrBFSe.ConsultarSituacao(numeroLote, protocolo);
            return TrataMensagemRetorno(ref mensagemAviso, ref mensagemErro);
        }
        public bool ConsultarLote(int numeroLote, string protocolo, ref string mensagemAviso, ref string mensagemErro)
        {
            mensagemAviso = "";
            mensagemErro = "";
            MensagemRetorno = ACBrBFSe.ConsultarLoteRps(protocolo, numeroLote);
            return TrataMensagemRetorno(ref mensagemAviso, ref mensagemErro);
        }
        public bool CancelarNFSe(string codigoCancelamento, string numeroNFSe, string motivoCancelamento, ref string mensagemAviso, ref string mensagemErro)
        {
            mensagemAviso = "";
            mensagemErro = "";
            MensagemRetorno = ACBrBFSe.CancelaNFSe(codigoCancelamento, numeroNFSe, motivoCancelamento);
            return TrataMensagemRetorno(ref mensagemAviso, ref mensagemErro);
        }
        public bool ConsultaNFSeRps(string numeroRps, string serieRps, int tipoRps, ref string mensagemAviso, ref string mensagemErro)
        {
            mensagemAviso = "";
            mensagemErro = "";
            MensagemRetorno = ACBrBFSe.ConsultaNFSeRps(numeroRps, serieRps, (TipoRps)tipoRps);
            return TrataMensagemRetorno(ref mensagemAviso, ref mensagemErro);
        }
        public bool ConsultaNFSe(DateTime dataInicial, DateTime dataFinal, ref string mensagemAviso, ref string mensagemErro)
        {
            mensagemAviso = "";
            mensagemErro = "";
            MensagemRetorno = ACBrBFSe.ConsultaNFSe(dataInicial, dataFinal);
            return TrataMensagemRetorno(ref mensagemAviso, ref mensagemErro);
        }
        #endregion Webservices

        #region Métodos de Apoio
        public string SelecionarCertificado()
        {
            return ACBrBFSe.Configuracoes.Certificados.SelecionarCertificado();
        }
        #endregion Métodos de Apoio

        #region Métodos privados
        private bool TrataMensagemRetorno(ref string mensagemAviso, ref string mensagemErro)
        {
            if (MensagemRetorno.Erros.Count > 0)
            {
                var builder = new System.Text.StringBuilder();
                foreach (var erro in MensagemRetorno.Erros)
                {
                    builder.Append("[" + erro.Codigo.ToString() + "] " + erro.Descricao + " / " + erro.Correcao);
                }
                if (builder.Length > 0)
                {
                    mensagemErro = builder.ToString();
                    return false;
                }
            }
            if (MensagemRetorno.Sucesso == false)
            {
                mensagemErro = "Ocorreu um erro não identificado.";
                return false;
            }
            return true;
        }
        #endregion
    }
}
