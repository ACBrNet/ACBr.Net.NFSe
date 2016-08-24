// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 28-07-2016
//
// Last Modified By : RFTD
// Last Modified On : 19-08-2016
// ***********************************************************************
// <copyright file="ProviderGinfes.cs" company="ACBr.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2016 Grupo ACBr.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.DFe.Core.Common;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using ACBr.Net.Core.Exceptions;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
    internal sealed class ProviderGinfes : ProviderBase
    {
        #region Constructors

        public ProviderGinfes(Configuracoes config, MunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Ginfes";
        }

        #endregion Constructors

        #region Methods

        public override NotaFiscal LoadXml(XmlDocument xml)
        {
            var xmlX = xml.ToXDocument();

            var root = xmlX?.ElementAnyNs("InfNfse") ?? xmlX?.ElementAnyNs("InfRps");
            Guard.Against<XmlException>(root == null, "Xml de Nota/RPS invalida.");

            var ret = new NotaFiscal();

            // Nota Fiscal
            ret.Numero = root.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            ret.ChaveNfse = root.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            ret.DhRecebimento = root.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;

            // RPS
            var rootRps = root.ElementAnyNs("IdentificacaoRps");
            if (rootRps != null)
            {
                ret.IdentificacaoRps.Numero = rootRps.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Serie = rootRps.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
                ret.IdentificacaoRps.Tipo = rootRps.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
            }
            ret.IdentificacaoRps.DataEmissaoRps = root.ElementAnyNs("DataEmissaoRps")?.GetValue<DateTime>() ?? DateTime.MinValue;

            // Natureza da Operação
            switch (root.ElementAnyNs("NaturezaOperacao")?.GetValue<char>())
            {
                case '1':
                    ret.NaturezaOperacao = NaturezaOperacao.NT01;
                    break;
                case '2':
                    ret.NaturezaOperacao = NaturezaOperacao.NT02;
                    break;
                case '3':
                    ret.NaturezaOperacao = NaturezaOperacao.NT03;
                    break;
                case '4':
                    ret.NaturezaOperacao = NaturezaOperacao.NT04;
                    break;
                case '5':
                    ret.NaturezaOperacao = NaturezaOperacao.NT05;
                    break;
                case '6':
                    ret.NaturezaOperacao = NaturezaOperacao.NT06;
                    break;
            }

            // Simples Nacional
            if (root.ElementAnyNs("OptanteSimplesNacional")?.GetValue<char>() == '1')
            {
                ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
            }
            else
            {
                // Regime Especial de Tributaçao
                switch (root.ElementAnyNs("RegimeEspecialTributacao")?.GetValue<char>())
                {
                    case '1':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresaMunicipal;
                        break;
                    case '2':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Estimativa;
                        break;
                    case '3':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.SociedadeProfissionais;
                        break;
                    case '4':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.Cooperativa;
                        break;
                    case '5':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioIndividual;
                        break;
                    case '6':
                        ret.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioEmpresaPP;
                        break;
                }
            }

            // Regime Especial de Tributaçao
            switch (root.ElementAnyNs("IncentivadorCultural")?.GetValue<char>())
            {
                case '1':
                    ret.IncentivadorCultural = NFSeSimNao.Sim;
                    break;
                case '2':
                    ret.IncentivadorCultural = NFSeSimNao.Nao;
                    break;
            }

            ret.Competencia = root.ElementAnyNs("Competencia")?.GetValue<string>() ?? string.Empty;
            ret.RpsSubstituido.NumeroNfse = root.ElementAnyNs("NfseSubstituida")?.GetValue<string>() ?? string.Empty;
            ret.OutrasInformacoes = root.ElementAnyNs("OutrasInformacoes")?.GetValue<string>() ?? string.Empty;


            // Serviços e Valores
            var rootServico = root.ElementAnyNs("Servico");
            if (rootServico != null)
            {
                var rootServicoValores = rootServico.ElementAnyNs("Valores");
                if (rootServicoValores != null)
                {
                    ret.Servico.Valores.ValorServicos = rootServicoValores.ElementAnyNs("ValorServicos")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorDeducoes = rootServicoValores.ElementAnyNs("ValorDeducoes")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorPis = rootServicoValores.ElementAnyNs("ValorPis")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorCofins = rootServicoValores.ElementAnyNs("ValorCofins")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorInss = rootServicoValores.ElementAnyNs("ValorInss")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorIR = rootServicoValores.ElementAnyNs("ValorIR")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorCsll = rootServicoValores.ElementAnyNs("ValorCsll")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.IssRetido = SituacaoTributaria.Normal;
                    ret.Servico.Valores.ValorIss = rootServicoValores.ElementAnyNs("ValorIss")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorOutrasRetencoes = rootServicoValores.ElementAnyNs("OutrasRetencoes")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.BaseCalculo = rootServicoValores.ElementAnyNs("BaseCalculo")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.Aliquota = rootServicoValores.ElementAnyNs("Aliquota")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorLiquidoNfse = rootServicoValores.ElementAnyNs("ValorLiquidoNfse")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.ValorIssRetido = rootServicoValores.ElementAnyNs("ValorIssRetido")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.DescontoCondicionado = rootServicoValores.ElementAnyNs("DescontoCondicionado")?.GetValue<decimal>() ?? 0;
                    ret.Servico.Valores.DescontoIncondicionado = rootServicoValores.ElementAnyNs("DescontoIncondicionado")?.GetValue<decimal>() ?? 0;
                }
                ret.Servico.xItemListaServico = rootServico.ElementAnyNs("ItemListaServico")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoCnae = rootServico.ElementAnyNs("CodigoCnae")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoTributacaoMunicipio = rootServico.ElementAnyNs("CodigoTributacaoMunicipio")?.GetValue<string>() ?? string.Empty;
                ret.Servico.Discriminacao = rootServico.ElementAnyNs("Discriminacao")?.GetValue<string>() ?? string.Empty;
                ret.Servico.CodigoMunicipio = rootServico.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
            }
            ret.ValorCredito = root.ElementAnyNs("ValorCredito")?.GetValue<Decimal>() ?? 0;

            // Prestador
            var rootPrestador = root.ElementAnyNs("PrestadorServico");
            if (rootPrestador != null)
            {
                var rootPretadorIdentificacao = rootPrestador.ElementAnyNs("IdentificacaoPrestador");
                if (rootPretadorIdentificacao != null)
                {
                    ret.Prestador.CPFCNPJ = rootPretadorIdentificacao.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.InscricaoMunicipal = rootPretadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }
                ret.Prestador.RazaoSocial = rootPrestador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                ret.Prestador.NomeFantasia = rootPrestador.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;
                var rootPrestadorEndereco = rootPrestador.ElementAnyNs("Endereco");
                if (rootPrestadorEndereco != null)
                {
                    ret.Prestador.Endereco.Logradouro = rootPrestadorEndereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.Numero = rootPrestadorEndereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.Complemento = rootPrestadorEndereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.Bairro = rootPrestadorEndereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.CodigoMunicipio = rootPrestadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.UF = rootPrestadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Endereco.CEP = rootPrestadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                }
                var rootPrestadorContato = rootPrestador.ElementAnyNs("Contato");
                if (rootPrestadorContato != null)
                {
                    ret.Prestador.Contato.Telefone = rootPrestadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                    ret.Prestador.Contato.Email = rootPrestadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Tomador
            var rootTomador = root.ElementAnyNs("TomadorServico");
            if (rootTomador != null)
            {
                var rootTomadorIdentificacao = rootTomador.ElementAnyNs("IdentificacaoTomador");
                if (rootTomadorIdentificacao != null)
                {
                    var rootTomadorIdentificacaoCpfCnpj = rootTomadorIdentificacao.ElementAnyNs("CpfCnpj");
                    if (rootTomadorIdentificacaoCpfCnpj != null)
                    {
                        ret.Tomador.CpfCnpj = rootTomadorIdentificacao.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                        if (String.IsNullOrWhiteSpace(ret.Tomador.CpfCnpj))
                            ret.Tomador.CpfCnpj = rootTomadorIdentificacao.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                    }
                    ret.Tomador.InscricaoMunicipal = rootTomadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }
                ret.Tomador.RazaoSocial = rootTomador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                var rootTomadorEndereco = rootTomador.ElementAnyNs("Endereco");
                if (rootTomadorEndereco != null)
                {
                    ret.Tomador.Endereco.Logradouro = rootTomadorEndereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Numero = rootTomadorEndereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Complemento = rootTomadorEndereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.Bairro = rootTomadorEndereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.CodigoMunicipio = rootTomadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.UF = rootTomadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Endereco.CEP = rootTomadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                }
                var rootTomadorContato = rootTomador.ElementAnyNs("Contato");
                if (rootTomadorContato != null)
                {
                    ret.Tomador.Contato.Telefone = rootTomadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                    ret.Tomador.Contato.Email = rootTomadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Intermediario
            var rootIntermediarioIdentificacao = root.ElementAnyNs("IntermediarioServico");
            if (rootIntermediarioIdentificacao != null)
            {
                ret.IntermediarioServico.RazaoSocial = rootIntermediarioIdentificacao.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
                var rootIntermediarioIdentificacaoCpfCnpj = rootIntermediarioIdentificacao.ElementAnyNs("CpfCnpj");
                if (rootIntermediarioIdentificacaoCpfCnpj != null)
                {
                    ret.IntermediarioServico.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cpf")?.GetValue<string>() ?? string.Empty;
                    if (String.IsNullOrWhiteSpace(ret.IntermediarioServico.CpfCnpj))
                        ret.IntermediarioServico.CpfCnpj = rootIntermediarioIdentificacaoCpfCnpj.ElementAnyNs("Cnpj")?.GetValue<string>() ?? string.Empty;
                }
                ret.IntermediarioServico.InscricaoMunicipal = rootIntermediarioIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
            }

            // Orgão Gerador
            var rootOrgaoGerador = root.ElementAnyNs("OrgaoGerador");
            if (rootOrgaoGerador != null)
            {
                ret.OrgaoGerador.CodigoMunicipio = rootOrgaoGerador.ElementAnyNs("CodigoMunicipio")?.GetValue<string>() ?? string.Empty;
                ret.OrgaoGerador.Uf = rootOrgaoGerador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
            }


            // Construção Civil
            var rootConstrucaoCivil = root.ElementAnyNs("ConstrucaoCivil");
            if (rootConstrucaoCivil != null)
            {
                ret.ConstrucaoCivil.CodigoObra = rootConstrucaoCivil.ElementAnyNs("CodigoObra")?.GetValue<string>() ?? string.Empty;
                ret.ConstrucaoCivil.Art = rootConstrucaoCivil.ElementAnyNs("Art")?.GetValue<string>() ?? string.Empty;
            }

            return ret;
        }
        public override RetornoWebservice ConsultarSituacao(int lote, string protocolo)
        {
            var retornoWebservice = new RetornoWebservice()
            {
                Sucesso = false,
                CPFCNPJRemetente = Config.PrestadorPadrao.CPFCNPJ,
                CodCidade = Config.WebServices.CodMunicipio,
                DataEnvioLote = DateTime.Now,
                NumeroLote = "0",
                Assincrono = true
            };

            // Monta mensagem de envio
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<ConsultarSituacaoLoteRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_situacao_lote_rps_envio_v03.xsd\">");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
            loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarSituacaoLoteRpsEnvio", Certificado);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarSituacao-{DateTime.Now:yyyyMMdd}-{protocolo}-env.xml");

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, "servico_consultar_situacao_lote_rps_envio_v03.xsd");
            if (retSchema != null)
                return retSchema;

            // Recebe mensagem de retorno
            try
            {
                var cliente = GetCliente(TipoUrl.ConsultarSituacao);
                var cabecalho = GerarCabecalho();
                retornoWebservice.XmlRetorno = cliente.ConsultarSituacao(cabecalho, retornoWebservice.XmlEnvio);
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarSituacao-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet);
            if (retornoWebservice.Erros.Count > 0)
                return retornoWebservice;

            retornoWebservice.Situacao = xmlRet.Root?.ElementAnyNs("Situacao")?.GetValue<string>() ?? "0";
            retornoWebservice.Sucesso = (retornoWebservice.Situacao == "4");
            retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;

            return retornoWebservice;
        }

        public override RetornoWebservice ConsultarLoteRps(string protocolo, int lote, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice()
            {
                Sucesso = false,
                CPFCNPJRemetente = Config.PrestadorPadrao.CPFCNPJ,
                CodCidade = Config.WebServices.CodMunicipio,
                DataEnvioLote = DateTime.Now,
                NumeroLote = "0",
                Assincrono = true
            };

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<ConsultarLoteRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_lote_rps_envio_v03.xsd\">");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadorPadrao.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
            loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadorPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarLoteRpsEnvio>");

            retornoWebservice.XmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarLoteRpsEnvio", Certificado);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMdd}-{protocolo}-env.xml");

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWebservice.XmlEnvio, "servico_consultar_lote_rps_envio_v03.xsd");
            if (retSchema != null)
                return retSchema;

            // Recebe mensagem de retorno
            try
            {
                var cliente = GetCliente(TipoUrl.ConsultarLoteRps);
                var cabecalho = GerarCabecalho();
                retornoWebservice.XmlRetorno = cliente.ConsultarLoteRps(cabecalho, retornoWebservice.XmlEnvio);
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarLote-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet);
            if (retornoWebservice.Erros.Count > 0)
                return retornoWebservice;

            var retornoLote = xmlRet.ElementAnyNs("ConsultarLoteRpsResposta");
            var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");
            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return retornoWebservice;
            }

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
                var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                    continue;

                nota.Numero = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                nota.ChaveNfse = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

                var xml = GetXmlNFSe(nota);
                GravarNFSeEmDisco(xml, $"NFSe-{nota.ChaveNfse}-{nota.Numero}.xml", nota.DhRecebimento);
            }
            return retornoWebservice;
        }

        #region Private Methods

        private static void MensagemErro(RetornoWebservice retornoWs, XDocument xmlRet)
        {
            var mensagens = xmlRet.ElementAnyNs("ListaMensagemRetorno");
            if (mensagens == null) return;

            foreach (var mensagem in mensagens.ElementsAnyNs("MensagemRetorno"))
            {
                var evento = new Evento
                {
                    Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                    Descricao = mensagem?.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty,
                    Correcao = mensagem?.ElementAnyNs("Correcao")?.GetValue<string>() ?? string.Empty
                };
                retornoWs.Erros.Add(evento);
            }
        }

        private static string GerarCabecalho()
        {
            var cabecalho = new StringBuilder();
            cabecalho.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            cabecalho.Append("<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://www.ginfes.com.br/cabecalho_v03.xsd\">");
            cabecalho.Append("<versaoDados>3</versaoDados>");
            cabecalho.Append("</ns2:cabecalho>");
            return cabecalho.ToString();
        }

        private IGinfesServiceClient GetCliente(TipoUrl tipo)
        {
            var url = GetUrl(tipo);
            if (Config.WebServices.Ambiente == TipoAmbiente.Homologacao)
            {
                return new GinfesHomServiceClient(url, TimeOut, Certificado);
            }

            return new GinfesProdServiceClient(url, TimeOut, Certificado);
        }

        #endregion Private Methods

        #endregion Methods
    }
}