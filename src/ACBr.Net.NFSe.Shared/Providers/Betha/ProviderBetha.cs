// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 05-22-2018
//
// Last Modified By : RFTD
// Last Modified On : 05-22-2018
// ***********************************************************************
// <copyright file="ProviderBetha.cs" company="ACBr.Net">
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

using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers.Betha
{
    internal sealed class ProviderBetha : ProviderABRASF
    {
        #region Constructors

        public ProviderBetha(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "Betha";
        }

        #endregion Constructors

        #region Methods

        #region Services

        public override RetornoWebservice Enviar(int lote, NotaFiscalCollection notas)

        {
            var retornoWebservice = new RetornoWebservice();

            if (lote == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lote não informado." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = GetXmlRps(nota, false, false);
                xmlLoteRps.Append(xmlRps);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlLote.Append($"<EnviarLoteRpsEnvio {GetNamespace()}>");
            xmlLote.Append($"<LoteRps Id=\"L{lote}\" xmlns=\"\">");
            xmlLote.Append($"<NumeroLote>{lote}</NumeroLote>");
            xmlLote.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps);
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append("</EnviarLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = xmlLote.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXmlTodos(retornoWebservice.XmlEnvio, "Rps", "InfRps", Certificado);
            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"lote-{lote}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.Enviar));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.Enviar))
                {
                    retornoWebservice.XmlRetorno = cliente.RecepcionarLoteRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"lote-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var xmlRoot = xmlRet.ElementAnyNs("EnviarLoteRpsEnvioResponse");
            MensagemErro(retornoWebservice, xmlRoot, "EnviarLoteRpsResposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            retornoWebservice.NumeroLote = xmlRoot.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.DataLote = xmlRoot.ElementAnyNs("DataRecebimento")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Protocolo = xmlRoot.ElementAnyNs("Protocolo")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = !retornoWebservice.NumeroLote.IsEmpty();

            if (!retornoWebservice.Sucesso) return retornoWebservice;

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            foreach (NotaFiscal nota in notas)
            {
                nota.NumeroLote = retornoWebservice.NumeroLote;
            }

            return retornoWebservice;
        }

        public override RetornoWebservice EnviarSincrono(int lote, NotaFiscalCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        public override RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (numeroNFSe.IsEmpty() || codigoCancelamento.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe/Codigo de cancelamento não informado para cancelamento." });
                return retornoWebservice;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<CancelarNfseEnvio {GetNamespace()}>");
            loteBuilder.Append("<Pedido xmlns=\"\">");
            loteBuilder.Append($"<InfPedidoCancelamento Id=\"N{numeroNFSe}\">");
            loteBuilder.Append("<IdentificacaoNfse>");
            loteBuilder.Append($"<Numero>{numeroNFSe}</Numero>");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append($"<CodigoMunicipio>{Configuracoes.PrestadorPadrao.Endereco.CodigoMunicipio}</CodigoMunicipio>");
            loteBuilder.Append("</IdentificacaoNfse>");
            loteBuilder.Append($"<CodigoCancelamento>{codigoCancelamento}</CodigoCancelamento>");
            loteBuilder.Append("</InfPedidoCancelamento>");
            loteBuilder.Append("</Pedido>");
            loteBuilder.Append("</CancelarNfseEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "Pedido", "InfPedidoCancelamento", Certificado);
            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"CanNFSe-{numeroNFSe}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.CancelaNFSe));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.CancelaNFSe))
                {
                    retornoWebservice.XmlRetorno = cliente.CancelarNFSe(GerarCabecalho(), retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"CanNFSe-{numeroNFSe}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet.Root, "CancelarNfseReposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var confirmacaoCancelamento = xmlRet.Root.ElementAnyNs("CancelarNfseReposta");
            retornoWebservice.DataLote = confirmacaoCancelamento.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
            retornoWebservice.Sucesso = confirmacaoCancelamento.ElementAnyNs("Sucesso")?.GetValue<bool>() ?? false;

            // Se a nota fiscal cancelada existir na coleção de Notas Fiscais, atualiza seu status:
            var nota = notas.FirstOrDefault(x => x.IdentificacaoNFSe.Numero.Trim() == numeroNFSe);
            if (nota == null) return retornoWebservice;

            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.Pedido.CodigoCancelamento = codigoCancelamento;
            nota.Cancelamento.DataHora = confirmacaoCancelamento.ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.Cancelamento.MotivoCancelamento = motivo;

            return retornoWebservice;
        }

        public override RetornoWebservice ConsultarSituacao(int lote, string protocolo)
        {
            var retornoWebservice = new RetornoWebservice();

            // Monta mensagem de envio
            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<ConsultarSituacaoLoteRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador xmlns=\"\">");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo xmlns=\"\">{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarSituacao-{DateTime.Now:yyyyMMddssfff}-{protocolo}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarSituacao));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultarSituacao))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarSituacaoLoteRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarSituacao-{DateTime.Now:yyyyMMddssfff}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet.Root, "ConsultarSituacaoLoteRpsResposta");

            retornoWebservice.NumeroLote = xmlRet.Root?.ElementAnyNs("ConsultarSituacaoLoteRpsResposta")?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Situacao = xmlRet.Root?.ElementAnyNs("ConsultarSituacaoLoteRpsResposta")?.ElementAnyNs("Situacao")?.GetValue<string>() ?? string.Empty;
            retornoWebservice.Sucesso = !retornoWebservice.Erros.Any();
            return retornoWebservice;
        }

        public override RetornoWebservice ConsultarLoteRps(int lote, string protocolo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            var loteBuilder = new StringBuilder();
            loteBuilder.Append($"<ConsultarLoteRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador xmlns=\"\">");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo xmlns=\"\">{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarLoteRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMddssfff}-{protocolo}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarLoteRps));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultarLoteRps))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarLoteRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarLote-{DateTime.Now:yyyyMMddssfff}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet.Root, "ConsultarLoteRpsResposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var retornoLote = xmlRet.Root.ElementAnyNs("ConsultarLoteRpsResposta");
            var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");

            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return retornoWebservice;
            }

            retornoWebservice.Sucesso = true;

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                var nfse = compNfse.ElementAnyNs("Nfse").ElementAnyNs("InfNfse");
                var numeroNFSe = nfse.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                var chaveNFSe = nfse.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
                var dataNFSe = nfse.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.Now;
                var numeroRps = nfse?.ElementAnyNs("IdentificacaoRps")?.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                GravarNFSeEmDisco(compNfse.AsString(true), $"NFSe-{numeroNFSe}-{chaveNFSe}-.xml", dataNFSe);

                var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
                if (nota == null)
                {
                    notas.Load(compNfse.ToString());
                }
                else
                {
                    nota.IdentificacaoNFSe.Numero = numeroNFSe;
                    nota.IdentificacaoNFSe.Chave = chaveNFSe;
                }
            }
            return retornoWebservice;
        }

        public override RetornoWebservice ConsultaNFSeRps(string numero, string serie, TipoRps tipo, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            if (numero.IsEmpty())
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Número da NFSe não informado para a consulta." });
                return retornoWebservice;
            }

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<ConsultarNfsePorRpsEnvio {GetNamespace()}>");
            loteBuilder.Append("<IdentificacaoRps xmlns=\"\">");
            loteBuilder.Append($"<Numero>{numero}</Numero>");
            loteBuilder.Append($"<Serie>{serie}</Serie>");
            loteBuilder.Append($"<Tipo>{(int)tipo + 1}</Tipo>");
            loteBuilder.Append("</IdentificacaoRps>");
            loteBuilder.Append("<Prestador xmlns=\"\">");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append("</ConsultarNfsePorRpsEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConNotaRps-{numero}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultaNFSeRps));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultaNFSeRps))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarNFSePorRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConNotaRps-{numero}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet.Root, "ConsultarNfseRpsResposta");
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var compNfse = xmlRet.Root.ElementAnyNs("ConsultarNfseRpsResposta")?.ElementAnyNs("CompNfse");
            if (compNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nota Fiscal não encontrada! (CompNfse)" });
                return retornoWebservice;
            }

            // Carrega a nota fiscal na coleção de Notas Fiscais
            var nota = LoadXml(compNfse.AsString());
            notas.Add(nota);

            retornoWebservice.Sucesso = true;
            return retornoWebservice;
        }

        public override RetornoWebservice ConsultaNFSe(DateTime? inicio, DateTime? fim, string numeroNfse, int pagina, string cnpjTomador,
            string imTomador, string nomeInter, string cnpjInter, string imInter, string serie, NotaFiscalCollection notas)
        {
            var retornoWebservice = new RetornoWebservice();

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append($"<ConsultarNfseEnvio {GetNamespace()}>");
            loteBuilder.Append("<Prestador xmlns=\"\">");
            loteBuilder.Append($"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>");
            loteBuilder.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");

            if (!numeroNfse.IsEmpty())
                loteBuilder.Append($"<NumeroNfse xmlns=\"\">{numeroNfse}</NumeroNfse>");

            if (inicio.HasValue && fim.HasValue)
            {
                loteBuilder.Append("<PeriodoEmissao xmlns=\"\">");
                loteBuilder.Append($"<DataInicial>{inicio:yyyy-MM-dd}</DataInicial>");
                loteBuilder.Append($"<DataFinal>{fim:yyyy-MM-dd}</DataFinal>");
                loteBuilder.Append("</PeriodoEmissao>");
            }

            if (!cnpjTomador.IsEmpty() || !imTomador.IsEmpty())
            {
                loteBuilder.Append("<Tomador xmlns=\"\">");
                loteBuilder.Append("<CpfCnpj>");
                loteBuilder.Append(cnpjTomador.IsCNPJ()
                    ? $"<Cnpj>{cnpjTomador.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{cnpjTomador.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!imTomador.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{imTomador}</InscricaoMunicipal>");
                loteBuilder.Append("</Tomador>");
            }

            if (!nomeInter.IsEmpty() && !cnpjInter.IsEmpty())
            {
                loteBuilder.Append("<IntermediarioServico xmlns=\"\">");
                loteBuilder.Append($"<RazaoSocial>{nomeInter}</RazaoSocial>");
                loteBuilder.Append(cnpjInter.IsCNPJ()
                    ? $"<Cnpj>{cnpjInter.ZeroFill(14)}</Cnpj>"
                    : $"<Cpf>{cnpjInter.ZeroFill(11)}</Cpf>");
                loteBuilder.Append("</CpfCnpj>");
                if (!imInter.IsEmpty()) loteBuilder.Append($"<InscricaoMunicipal>{imInter}</InscricaoMunicipal>");
                loteBuilder.Append("</IntermediarioServico>");
            }

            loteBuilder.Append("</ConsultarNfseEnvio>");
            retornoWebservice.XmlEnvio = loteBuilder.ToString();

            if (Configuracoes.Geral.RetirarAcentos)
            {
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();
            }

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConNota-{DateTime.Now:yyyyMMddssfff}-{numeroNfse}-env.xml");

            // Verifica Schema
            ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultaNFSe));
            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.ConsultaNFSe))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarNFSe(GerarCabecalho(), retornoWebservice.XmlEnvio);
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConNota-{DateTime.Now:yyyyMMddssfff}-{numeroNfse}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            MensagemErro(retornoWebservice, xmlRet.Root, "ConsultarNfseResposta");

            if (retornoWebservice.Erros.Any()) return retornoWebservice;

            var retornoLote = xmlRet.Root.ElementAnyNs("ConsultarNfseResposta");
            var listaNfse = retornoLote?.ElementAnyNs("ListaNfse");
            if (listaNfse == null)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Lista de NFSe não encontrada! (ListaNfse)" });
                return retornoWebservice;
            }

            foreach (var compNfse in listaNfse.ElementsAnyNs("CompNfse"))
            {
                // Carrega a nota fiscal na coleção de Notas Fiscais
                var nota = LoadXml(compNfse.AsString());
                notas.Add(nota);
            }

            retornoWebservice.Sucesso = true;

            return retornoWebservice;
        }

        #endregion Services

        #region Protected Methods

        protected override IABRASFClient GetClient(TipoUrl tipo)
        {
            switch (tipo)
            {
                case TipoUrl.CancelaNFSe: return new BethaServiceClient(this, tipo, null);
                case TipoUrl.ConsultaNFSeRps: return new BethaServiceClient(this, tipo, null);
                case TipoUrl.ConsultaNFSe: return new BethaServiceClient(this, tipo, null);
                default: return new BethaServiceClient(this, tipo);
            }
        }

        protected override string GetNamespace()
        {
            return "xmlns=\"http://www.betha.com.br/e-nota-contribuinte-ws\"";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            switch (tipo)
            {
                case TipoUrl.Enviar: return "servico_enviar_lote_rps_envio_v01.xsd";
                case TipoUrl.ConsultarSituacao: return "servico_consultar_situacao_lote_rps_envio_v01.xsd";
                case TipoUrl.ConsultarLoteRps: return "servico_consultar_lote_rps_envio_v01.xsd";
                case TipoUrl.ConsultaNFSeRps: return "servico_consultar_nfse_rps_envio_v01.xsd";
                case TipoUrl.ConsultaNFSe: return "servico_consultar_nfse_envio_v01.xsd";
                case TipoUrl.CancelaNFSe: return "servico_cancelar_nfse_envio_v01.xsd";
                default: throw new ArgumentOutOfRangeException(nameof(tipo), tipo, @"Valor incorreto ou serviço não suportado.");
            }
        }

        #endregion Protected Methods

        #endregion Methods
    }
}