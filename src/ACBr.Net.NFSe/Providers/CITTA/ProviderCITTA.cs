// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Felipe Silveira (Transis Software)
// Created          : 26-08-2021
//
// Last Modified By : Felipe Silveira (Transis Software)
// Last Modified On : 26-08-2021
// ***********************************************************************
// <copyright file="ProviderISSe.cs" company="ACBr.Net">
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
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System.Text;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Providers
{
    internal sealed class ProviderCITTA : ProviderABRASF201
    {
        #region Constructors

        public ProviderCITTA(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "CITTA";
        }

        #endregion Constructors

        #region Methods

        #region Protected Methods

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Nenhuma RPS informada." });
            if (notas.Count > 1) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Apenas uma RPS pode ser enviada em modo Sincrono." });
            if (retornoWebservice.Erros.Count > 0) return;

            var xmlLoteRps = new StringBuilder();

            foreach (var nota in notas)
            {
                var xmlRps = WriteXmlRps(nota, false, false);
                GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);
                xmlLoteRps.Append(xmlRps);
            }

            var xmlLote = new StringBuilder();
            xmlLote.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlLote.Append("<nfse:RecepcionarLoteRpsSincronoEnvio xmlns:nfse=\"http://nfse.abrasf.org.br\" xmlns:nfse1=\"http://nfse.citta.com.br\">");
            xmlLote.Append($"<LoteRps Id=\"L{retornoWebservice.Lote}\" {GetVersao()}>");
            xmlLote.Append($"<NumeroLote>{retornoWebservice.Lote}</NumeroLote>");
            if (UsaPrestadorEnvio) xmlLote.Append("<Prestador>");
            xmlLote.Append("<CpfCnpj>");
            xmlLote.Append(Configuracoes.PrestadorPadrao.CpfCnpj.IsCNPJ()
                ? $"<Cnpj>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(14)}</Cnpj>"
                : $"<Cpf>{Configuracoes.PrestadorPadrao.CpfCnpj.ZeroFill(11)}</Cpf>");
            xmlLote.Append("</CpfCnpj>");
            if (!Configuracoes.PrestadorPadrao.InscricaoMunicipal.IsEmpty()) xmlLote.Append($"<InscricaoMunicipal>{Configuracoes.PrestadorPadrao.InscricaoMunicipal}</InscricaoMunicipal>");
            if (UsaPrestadorEnvio) xmlLote.Append("</Prestador>");
            xmlLote.Append($"<QuantidadeRps>{notas.Count}</QuantidadeRps>");
            xmlLote.Append("<ListaRps>");
            xmlLote.Append(xmlLoteRps.Replace("InfDeclaracaoPrestacaoServico", "nfse1:InfDeclaracaoPrestacaoServico"));
            xmlLote.Append("</ListaRps>");
            xmlLote.Append("</LoteRps>");
            xmlLote.Append("</nfse:RecepcionarLoteRpsSincronoEnvio>");
            string xml = xmlLote.ToString();
            retornoWebservice.XmlEnvio = xml;
        }

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "EnviarLoteRpsEnvio", "LoteRps", Certificado);
            //NAO PRECISA ASSINAR
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            //retornoWebservice.XmlEnvio = XmlSigning.AssinarXml(retornoWebservice.XmlEnvio, "nfse:RecepcionarLoteRpsSincronoEnvio", "LoteRps", Certificado);
            //NAO PRECISA ASSINAR
        }

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new CITTAServiceClient(this, tipo);
        }

        protected override bool PrecisaValidarSchema(TipoUrl tipo)
        {
            //NAO CONSEGUI OBTER OS SCHEMAS DESTE PROVEDOR ATE O DIA 24/08/21
            return false;
        }

        #endregion Protected Methods

        #region RPS

        protected override XElement WriteRps(NotaServico nota)
        {
            var rootRps = new XElement("rps");

            var infServico = new XElement("InfDeclaracaoPrestacaoServico", new XAttribute("Id", $"R{nota.IdentificacaoRps.Numero.OnlyNumbers()}"));
            rootRps.Add(infServico);

            infServico.Add(WriteRpsRps(nota));

            infServico.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.Competencia));

            infServico.AddChild(WriteServicosRps(nota));
            infServico.AddChild(WritePrestadorRps(nota));
            infServico.AddChild(WriteTomadorRps(nota));
            infServico.AddChild(WriteIntermediarioRps(nota));
            infServico.AddChild(WriteConstrucaoCivilRps(nota));

            string regimeEspecialTributacao;
            string optanteSimplesNacional;
            if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
            {
                regimeEspecialTributacao = "6";
                optanteSimplesNacional = "1";
            }
            else
            {
                regimeEspecialTributacao = ((int)nota.RegimeEspecialTributacao).ToString();
                optanteSimplesNacional = "2";
            }

            if (nota.RegimeEspecialTributacao != RegimeEspecialTributacao.Nenhum)
                infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria, regimeEspecialTributacao));

            infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria, optanteSimplesNacional));
            infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2));

            return rootRps;
        }

        protected override XElement WriteRpsRps(NotaServico nota)
        {
            //var rps = new XElement("Rps");
            var rps = new XElement("Rps", new XAttribute("Id", "S1"));

            rps.Add(WriteIdentificacaoRps(nota));

            rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Situacao + 1));

            rps.AddChild(WriteSubstituidoRps(nota));

            return rps;
        }

        #endregion


        #endregion Methods
    }
}