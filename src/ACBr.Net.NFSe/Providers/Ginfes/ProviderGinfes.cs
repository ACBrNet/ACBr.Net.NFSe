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
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Providers.Ginfes
{
    internal sealed class ProviderGinfes : ProviderBase
    {
        #region Constructors

        public ProviderGinfes(Configuracoes config, MunicipioNFSe municipio) : base(config, municipio)
        {
        }

        #endregion Constructors

        #region Methods

        public override RetornoWebService ConsultarSituacao(int lote, string protocolo)
        {
            var retornoWS = new RetornoWebService()
            {
                Sucesso = false,
                CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
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
            loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadoPadrao.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
            loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadoPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarSituacaoLoteRpsEnvio>");
            retornoWS.xmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarSituacaoLoteRpsEnvio", Certificado);

            GravarArquivoEmDisco(retornoWS.xmlEnvio, $"ConsultarSituacao-{DateTime.Now:yyyyMMdd}-{protocolo}-env.xml");

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWS.xmlEnvio, "GINFES", "servico_consultar_situacao_lote_rps_envio_v03.xsd");
            if (retSchema != null)
                return retSchema;

            // Recebe mensagem de retorno
            try
            {
                var cliente = GetCliente(TipoUrl.ConsultarSituacao);
                var cabecalho = GerarCabecalho();
                retornoWS.xmlRetorno = cliente.ConsultarSituacao(cabecalho, retornoWS.xmlEnvio);
            }
            catch (Exception ex)
            {
                retornoWS.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWS;
            }
            GravarArquivoEmDisco(retornoWS.xmlRetorno, $"ConsultarSituacao-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWS.xmlRetorno);
            MensagemErro(retornoWS, xmlRet);
            if (retornoWS.Erros.Count > 0)
                return retornoWS;

            retornoWS.Situacao = xmlRet.Root?.ElementAnyNs("Situacao")?.GetValue<string>() ?? "0";
            retornoWS.Sucesso = (retornoWS.Situacao == "4");
            retornoWS.NumeroLote = xmlRet.Root?.ElementAnyNs("NumeroLote")?.GetValue<string>() ?? string.Empty;

            return retornoWS;
        }


        public override RetornoWebService ConsultarLoteRps(string protocolo, int lote, NotaFiscalCollection notas)
        {
            var retornoWS = new RetornoWebService()
            {
                Sucesso = false,
                CPFCNPJRemetente = Config.PrestadoPadrao.CPFCNPJ,
                CodCidade = Config.WebServices.CodMunicipio,
                DataEnvioLote = DateTime.Now,
                NumeroLote = "0",
                Assincrono = true
            };

            var loteBuilder = new StringBuilder();
            loteBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            loteBuilder.Append("<ConsultarLoteRpsEnvio xmlns:tipos=\"http://www.ginfes.com.br/tipos_v03.xsd\" xmlns=\"http://www.ginfes.com.br/servico_consultar_lote_rps_envio_v03.xsd\">");
            loteBuilder.Append("<Prestador>");
            loteBuilder.Append($"<tipos:Cnpj>{Config.PrestadoPadrao.CPFCNPJ.ZeroFill(14)}</tipos:Cnpj>");
            loteBuilder.Append($"<tipos:InscricaoMunicipal>{Config.PrestadoPadrao.InscricaoMunicipal}</tipos:InscricaoMunicipal>");
            loteBuilder.Append("</Prestador>");
            loteBuilder.Append($"<Protocolo>{protocolo}</Protocolo>");
            loteBuilder.Append("</ConsultarLoteRpsEnvio>");
            retornoWS.xmlEnvio = CertificadoDigital.AssinarXml(loteBuilder.ToString(), "", "ConsultarLoteRpsEnvio", Certificado);

            GravarArquivoEmDisco(retornoWS.xmlEnvio, $"ConsultarLote-{DateTime.Now:yyyyMMdd}-{protocolo}-env.xml");

            // Verifica Schema
            var retSchema = ValidarSchema(retornoWS.xmlEnvio, "GINFES", "servico_consultar_lote_rps_envio_v03.xsd");
            if (retSchema != null)
                return retSchema;

            // Recebe mensagem de retorno
            try
            {
                var cliente = GetCliente(TipoUrl.ConsultarLoteRps);
                var cabecalho = GerarCabecalho();
                retornoWS.xmlRetorno = cliente.ConsultarLoteRps(cabecalho, retornoWS.xmlEnvio);
            }
            catch (Exception ex)
            {
                retornoWS.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWS;
            }
            GravarArquivoEmDisco(retornoWS.xmlRetorno, $"ConsultarLote-{DateTime.Now:yyyyMMdd}-{lote}-ret.xml");

            // Analisa mensagem de retorno
            var xmlRet = XDocument.Parse(retornoWS.xmlRetorno);
            MensagemErro(retornoWS, xmlRet);
            if (retornoWS.Erros.Count > 0)
                return retornoWS;


            var cabeçalho = xmlRet.Element("ConsultarLoteRpsResposta");
            
            // Aqui precisamos analisar cada NFSe do Lote
            // Seguindo o exemplo do DSF, as notas fiscais já estariam no parâmetro notas (terceiro parâmetro deste método) 

            return retornoWS;

            //retConsulta.Sucesso = cabeçalho?.Element("Sucesso")?.GetValue<bool>() ?? false;
            //retConsulta.CodCidade = cabeçalho?.Element("CodCidade")?.GetValue<int>() ?? 0;
            //retConsulta.NumeroLote = cabeçalho?.Element("NumeroLote")?.GetValue<string>() ?? string.Empty;
            //retConsulta.CPFCNPJRemetente = cabeçalho?.Element("CPFCNPJRemetente")?.GetValue<string>() ?? string.Empty;
            //retConsulta.DataEnvioLote = cabeçalho?.Element("DataEnvioLote")?.GetValue<DateTime>() ?? DateTime.MinValue;

            //var erros = xmlRet.Element("Erros");
            ////retConsulta.Erros.AddRange(ProcessarEventos(TipoEvento.Erros, erros));

            //var alertas = xmlRet.Element("Alertas");
            ////retConsulta.Alertas.AddRange(ProcessarEventos(TipoEvento.Alertas, alertas));

            //var nfses = xmlRet.Element("ListaNFSe");
            //if (nfses == null) return retConsulta;

            //foreach (var nfse in nfses.Elements("ConsultaNFSe"))
            //{
            //    var numeroRps = (nfse.Element("NumeroRPS")?.GetValue<string>() ?? string.Empty);
            //    var nota = notas.FirstOrDefault(x => x.IdentificacaoRps.Numero == numeroRps);
            //    if (nota == null) continue;

            //    nota.Numero = nfse.Element("NumeroNFe")?.GetValue<string>() ?? string.Empty;
            //    nota.ChaveNfse = nfse.Element("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;

            //    var nfseFile = Path.Combine(Config.Arquivos.GetPathNFSe(nota.DhRecebimento),
            //        $"NFSe-{nota.ChaveNfse}-{nota.Numero}.xml");
            //    var xml = GetXmlNFSe(nota);
            //    File.WriteAllText(nfseFile, xml, Encoding.UTF8);
            //}

            //return retConsulta;

        }


        #region Private Methods
        private void GravarArquivoEmDisco(string conteudoArquivo, string nomeArquivo)
        {
            if (Config.Geral.Salvar == false)
                return;
            nomeArquivo = Path.Combine(Config.Arquivos.GetPathLote(), nomeArquivo);
            File.WriteAllText(nomeArquivo, conteudoArquivo, Encoding.UTF8);
        }

        private static void MensagemErro(RetornoWebService retornoWS, XDocument xmlRet)
        {
            var mensagens = xmlRet.Root.ElementAnyNs("ListaMensagemRetorno");
            if (mensagens != null)
            {
                foreach (var mensagem in mensagens.ElementsAnyNs("MensagemRetorno"))
                {
                    var evento = new Evento
                    {
                        Codigo = mensagem?.ElementAnyNs("Codigo")?.GetValue<string>() ?? string.Empty,
                        Descricao = mensagem?.ElementAnyNs("Mensagem")?.GetValue<string>() ?? string.Empty,
                        Correcao = mensagem?.ElementAnyNs("Correcao")?.GetValue<string>() ?? string.Empty
                    };
                    retornoWS.Erros.Add(evento);
                }
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