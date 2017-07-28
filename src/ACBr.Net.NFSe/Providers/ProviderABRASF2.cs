// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 12-08-2016
//
// Last Modified By : RFTD
// Last Modified On : 12-08-2016
// ***********************************************************************
// <copyright file="ProviderABRASF2.cs" company="ACBr.Net">
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
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
using System.Xml;
using System.Xml.Linq;
using ACBr.Net.Core.Exceptions;

namespace ACBr.Net.NFSe.Providers
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Classe base para trabalhar com provedores que usam o padrão ABRASF V2
    /// </summary>
    /// <seealso cref="ACBr.Net.NFSe.Providers.ProviderBase" />
    public abstract class ProviderABRASF2 : ProviderBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderABRASF2"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="municipio">The municipio.</param>
        protected ProviderABRASF2(ConfiguracoesNFSe config, MunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "ABRASFv2";
        }

        #endregion Constructors

        #region Methods

        #region LoadXml

        public override NotaFiscal LoadXml(XDocument xml)
        {
            Guard.Against<XmlException>(xml == null, "Xml invalido.");

            XElement rootNFSe = null;
            XElement rootCanc = null;
            XElement rootSub = null;
            XElement rootRps;

            var rootGrupo = xml.ElementAnyNs("CompNfse");
            if (rootGrupo != null)
            {
                rootNFSe = rootGrupo.ElementAnyNs("Nfse")?.ElementAnyNs("InfNfse");
                rootSub = rootGrupo.ElementAnyNs("NfseCancelamento");
                rootCanc = rootGrupo.ElementAnyNs("NfseSubstituicao");
                rootRps = rootNFSe.ElementAnyNs("DeclaracaoPrestacaoServico")?.ElementAnyNs("InfDeclaracaoPrestacaoServico");
            }
            else
            {
                rootRps = xml.ElementAnyNs("Rps").ElementAnyNs("InfDeclaracaoPrestacaoServico");
            }

            Guard.Against<XmlException>(rootNFSe == null && rootRps == null, "Xml de RPS ou NFSe invalido.");

            var ret = new NotaFiscal();

            if (rootNFSe != null)
            {
                LoadNFSe(ret, rootNFSe);
                if (rootSub != null) LoadNFSeSub(ret, rootSub);
                if (rootCanc != null) LoadNFSeCancel(ret, rootCanc);
            }

            LoadRps(ret, rootRps);

            return ret;
        }

        protected virtual void LoadRps(NotaFiscal nota, XElement rpsRoot)
        {
            var rps = rpsRoot.ElementAnyNs("Rps");
            var ideRps = rps.ElementAnyNs("IdentificacaoRps");
            var subRps = ideRps.ElementAnyNs("RpsSubstituido");

            nota.IdentificacaoRps.Numero = ideRps.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Serie = ideRps.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoRps.Tipo = ideRps.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;

            nota.IdentificacaoRps.DataEmissao = rps.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.Situacao = rps.ElementAnyNs("Status")?.GetValue<SituacaoNFSeRps>() ?? SituacaoNFSeRps.Normal;

            if (subRps != null)
            {
                nota.RpsSubstituido.NumeroRps = subRps.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                nota.RpsSubstituido.Serie = subRps.ElementAnyNs("Serie")?.GetValue<string>() ?? string.Empty;
                nota.RpsSubstituido.Tipo = subRps.ElementAnyNs("Tipo")?.GetValue<TipoRps>() ?? TipoRps.RPS;
            }

            nota.Competencia = rpsRoot.ElementAnyNs("Competencia")?.GetValue<DateTime>() ?? DateTime.MinValue;

            var rootServico = rpsRoot.ElementAnyNs("Servico");
            if (rootServico != null)
            {
                var rootServicoValores = rootServico.ElementAnyNs("Valores");
                if (rootServicoValores != null)
                {
                    nota.Servico.Valores.ValorServicos = rootServicoValores.ElementAnyNs("ValorServicos")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.ValorDeducoes = rootServicoValores.ElementAnyNs("ValorDeducoes")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.ValorPis = rootServicoValores.ElementAnyNs("ValorPis")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.ValorCofins = rootServicoValores.ElementAnyNs("ValorCofins")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.ValorInss = rootServicoValores.ElementAnyNs("ValorInss")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.ValorIr = rootServicoValores.ElementAnyNs("ValorIr")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.ValorCsll = rootServicoValores.ElementAnyNs("ValorCsll")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.ValorOutrasRetencoes = rootServicoValores.ElementAnyNs("OutrasRetencoes")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.ValorIss = rootServicoValores.ElementAnyNs("ValorIss")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.Aliquota = rootServicoValores.ElementAnyNs("Aliquota")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.DescontoCondicionado = rootServicoValores.ElementAnyNs("DescontoCondicionado")?.GetValue<decimal>() ?? 0;
                    nota.Servico.Valores.DescontoIncondicionado = rootServicoValores.ElementAnyNs("DescontoIncondicionado")?.GetValue<decimal>() ?? 0;
                }

                nota.Servico.Valores.IssRetido = (rootServico.ElementAnyNs("IssRetido")?.GetValue<int>() ?? 0) == 1 ? SituacaoTributaria.Retencao : SituacaoTributaria.Normal;

                if (rootServico.ElementAnyNs("ResponsavelRetencao") != null)
                    nota.Servico.ResponsavelRetencao = rootServico.ElementAnyNs("ResponsavelRetencao").GetValue<int>() == 1 ? ResponsavelRetencao.Prestador : ResponsavelRetencao.Tomador;

                nota.Servico.ItemListaServico = rootServico.ElementAnyNs("ItemListaServico")?.GetValue<string>() ?? string.Empty;
                nota.Servico.CodigoCnae = rootServico.ElementAnyNs("CodigoCnae")?.GetValue<string>() ?? string.Empty;
                nota.Servico.CodigoTributacaoMunicipio = rootServico.ElementAnyNs("CodigoTributacaoMunicipio")?.GetValue<string>() ?? string.Empty;
                nota.Servico.Discriminacao = rootServico.ElementAnyNs("Discriminacao")?.GetValue<string>() ?? string.Empty;
                nota.Servico.CodigoMunicipio = rootServico.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                nota.Servico.CodigoPais = rootServico.ElementAnyNs("CodigoPais")?.GetValue<int>() ?? 0;
                nota.Servico.ExigibilidadeIss = (ExigibilidadeIss)(rootServico.ElementAnyNs("ExigibilidadeISS")?.GetValue<int>() - 1 ?? 0);
                nota.Servico.MunicipioIncidencia = rootServico.ElementAnyNs("MunicipioIncidencia")?.GetValue<int>() ?? 0;
                nota.Servico.NumeroProcesso = rootServico.ElementAnyNs("NumeroProcesso")?.GetValue<string>() ?? string.Empty;
            }

            // Prestador (RPS)
            var rootPrestador = rpsRoot.ElementAnyNs("Prestador");
            if (rootPrestador != null)
            {
                nota.Prestador.CpfCnpj = rootPrestador.ElementAnyNs("CpfCnpj")?.GetCPF_CNPJ();
                nota.Prestador.InscricaoMunicipal = rootPrestador.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
            }

            // Tomador
            var rootTomador = rpsRoot.ElementAnyNs("Tomador");
            if (rootTomador != null)
            {
                var tomadorIdentificacao = rootTomador.ElementAnyNs("IdentificacaoTomador");
                if (tomadorIdentificacao != null)
                {
                    nota.Tomador.CpfCnpj = tomadorIdentificacao.ElementAnyNs("CpfCnpj")?.GetCPF_CNPJ();
                    nota.Tomador.InscricaoMunicipal = tomadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }

                nota.Tomador.RazaoSocial = rootTomador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;

                var rootTomadorEndereco = rootTomador.ElementAnyNs("Endereco");
                if (rootTomadorEndereco != null)
                {
                    nota.Tomador.Endereco.Logradouro = rootTomadorEndereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                    nota.Tomador.Endereco.Numero = rootTomadorEndereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                    nota.Tomador.Endereco.Complemento = rootTomadorEndereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                    nota.Tomador.Endereco.Bairro = rootTomadorEndereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                    nota.Tomador.Endereco.CodigoMunicipio = rootTomadorEndereco.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                    nota.Tomador.Endereco.Uf = rootTomadorEndereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                    nota.Tomador.Endereco.CodigoPais = rootTomadorEndereco.ElementAnyNs("CodigoPais")?.GetValue<int>() ?? 0;
                    nota.Tomador.Endereco.Cep = rootTomadorEndereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
                }

                var rootTomadorContato = rootTomador.ElementAnyNs("Contato");
                if (rootTomadorContato != null)
                {
                    nota.Tomador.DadosContato.DDD = "";
                    nota.Tomador.DadosContato.Telefone = rootTomadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                    nota.Tomador.DadosContato.Email = rootTomadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Intermediario
            var rootIntermediario = rpsRoot.ElementAnyNs("Intermediario");
            if (rootIntermediario != null)
            {
                nota.Intermediario.RazaoSocial = rootIntermediario.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;

                var intermediarioIdentificacao = rootIntermediario.ElementAnyNs("IdentificacaoIntermediario");
                if (intermediarioIdentificacao != null)
                {
                    nota.Intermediario.CpfCnpj = intermediarioIdentificacao.ElementAnyNs("CpfCnpj")?.GetCPF_CNPJ();
                    nota.Intermediario.InscricaoMunicipal = intermediarioIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
                }
            }

            // Construção Civil
            var rootConstrucaoCivil = rpsRoot.ElementAnyNs("ConstrucaoCivil");
            if (rootConstrucaoCivil != null)
            {
                nota.ConstrucaoCivil.CodigoObra = rootConstrucaoCivil.ElementAnyNs("CodigoObra")?.GetValue<string>() ?? string.Empty;
                nota.ConstrucaoCivil.ArtObra = rootConstrucaoCivil.ElementAnyNs("Art")?.GetValue<string>() ?? string.Empty;
            }

            // Simples Nacional
            if (rpsRoot.ElementAnyNs("OptanteSimplesNacional")?.GetValue<int>() == 1)
            {
                nota.RegimeEspecialTributacao = RegimeEspecialTributacao.SimplesNacional;
            }
            else
            {
                // Regime Especial de Tributaçao
                switch (rpsRoot.ElementAnyNs("RegimeEspecialTributacao")?.GetValue<int>())
                {
                    case 1:
                        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresaMunicipal;
                        break;

                    case 2:
                        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.Estimativa;
                        break;

                    case 3:
                        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.SociedadeProfissionais;
                        break;

                    case 4:
                        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.Cooperativa;
                        break;

                    case 5:
                        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioIndividual;
                        break;

                    case 6:
                        nota.RegimeEspecialTributacao = RegimeEspecialTributacao.MicroEmpresarioEmpresaPP;
                        break;
                }
            }

            // Incentivador Culturalstr
            switch (rpsRoot.ElementAnyNs("IncentivadorCultural")?.GetValue<int>())
            {
                case 1:
                    nota.IncentivadorCultural = NFSeSimNao.Sim;
                    break;

                case 2:
                    nota.IncentivadorCultural = NFSeSimNao.Nao;
                    break;
            }
        }

        protected virtual void LoadNFSe(NotaFiscal nota, XElement rootNFSe)
        {
            nota.IdentificacaoNFSe.Numero = rootNFSe.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.Chave = rootNFSe.ElementAnyNs("CodigoVerificacao")?.GetValue<string>() ?? string.Empty;
            nota.IdentificacaoNFSe.DataEmissao = rootNFSe.ElementAnyNs("DataEmissao")?.GetValue<DateTime>() ?? DateTime.MinValue;

            nota.OutrasInformacoes = rootNFSe.ElementAnyNs("OutrasInformacoes")?.GetValue<string>() ?? string.Empty;

            // Valores NFSe
            var valoresNFSe = rootNFSe.ElementAnyNs("ValoresNfse");
            if (valoresNFSe != null)
            {
                nota.Servico.Valores.BaseCalculo = valoresNFSe.ElementAnyNs("BaseCalculo")?.GetValue<decimal>() ?? 0;
                nota.Servico.Valores.Aliquota = valoresNFSe.ElementAnyNs("Aliquota")?.GetValue<decimal>() ?? 0;
                nota.Servico.Valores.ValorIss = valoresNFSe.ElementAnyNs("ValorIss")?.GetValue<decimal>() ?? 0;
                nota.Servico.Valores.ValorLiquidoNfse = valoresNFSe.ElementAnyNs("ValorLiquidoNfse")?.GetValue<decimal>() ?? 0;
            }

            nota.ValorCredito = rootNFSe.ElementAnyNs("ValorCredito")?.GetValue<decimal>() ?? 0;

            // Endereco Prestador
            var enderecoPrestador = rootNFSe.ElementAnyNs("EnderecoPrestadorServico");
            if (enderecoPrestador != null)
            {
                nota.Prestador.Endereco.Logradouro = enderecoPrestador.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.Numero = enderecoPrestador.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.Complemento = enderecoPrestador.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.Bairro = enderecoPrestador.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.CodigoMunicipio = enderecoPrestador.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                nota.Prestador.Endereco.Uf = enderecoPrestador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.CodigoPais = enderecoPrestador.ElementAnyNs("CodigoPais")?.GetValue<int>() ?? 0;
                nota.Prestador.Endereco.Cep = enderecoPrestador.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
            }

            // Orgão Gerador
            var rootOrgaoGerador = rootNFSe.ElementAnyNs("OrgaoGerador");
            if (rootOrgaoGerador == null) return;

            nota.OrgaoGerador.CodigoMunicipio = rootOrgaoGerador.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
            nota.OrgaoGerador.Uf = rootOrgaoGerador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
        }

        protected virtual void LoadNFSeSub(NotaFiscal nota, XElement rootSub)
        {
            nota.RpsSubstituido.NFSeSubstituidora = rootSub.ElementAnyNs("SubstituicaoNfse")?.ElementAnyNs("NfseSubstituida")?.GetValue<string>() ?? string.Empty;
            nota.RpsSubstituido.Signature = LoadSignature(rootSub.ElementAnyNs("Signature"));
        }

        protected virtual void LoadNFSeCancel(NotaFiscal nota, XElement rootCanc)
        {
            nota.Situacao = SituacaoNFSeRps.Cancelado;
            nota.Cancelamento.DataHora = rootCanc.ElementAnyNs("Confirmacao").ElementAnyNs("DataHora")?.GetValue<DateTime>() ?? DateTime.MinValue;
            nota.Cancelamento.Signature = LoadSignature(rootCanc.ElementAnyNs("Signature"));

            nota.Cancelamento.Pedido.CodigoCancelamento = rootCanc.ElementAnyNs("Confirmacao").ElementAnyNs("Pedido").ElementAnyNs("InfPedidoCancelamento")?.ElementAnyNs("CodigoCancelamento")?.GetValue<string>() ?? string.Empty;
            nota.Cancelamento.Pedido.Signature = LoadSignature(rootCanc.ElementAnyNs("Confirmacao").ElementAnyNs("Pedido").ElementAnyNs("Signature"));
        }

        #endregion LoadXml

        #region RPS

        public override string GetXmlRps(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
        {
            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            xmlDoc.Add(GenerateRps(nota));
            return xmlDoc.AsString(identado, showDeclaration);
        }

        protected virtual XElement GenerateRps(NotaFiscal nota)
        {
            var rootRps = new XElement("Rps");

            var infServico = new XElement("InfDeclaracaoPrestacaoServico", new XAttribute("Id", $"{nota.IdentificacaoRps.Numero.OnlyNumbers()}"));
            rootRps.Add(infServico);

            var rps = new XElement("Rps");
            infServico.Add(rps);

            var indRps = new XElement("IdentificacaoRps");
            rps.Add(indRps);

            indRps.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
            indRps.AddChild(AdicionarTag(TipoCampo.Str, "", "Serie", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
            indRps.AddChild(AdicionarTag(TipoCampo.Int, "", "Tipo", 1, 1, Ocorrencia.Obrigatoria, (int)nota.IdentificacaoRps.Tipo++));

            rps.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Situacao++));

            infServico.AddChild(AdicionarTag(TipoCampo.Dat, "", "Competencia", 10, 10, Ocorrencia.Obrigatoria, nota.Competencia));

            var servico = GenerateServicosValoresRps(nota);
            infServico.Add(servico);

            var prestador = GeneratePrestadorRps(nota);
            infServico.Add(prestador);

            if (!nota.Tomador.CpfCnpj.IsEmpty())
            {
                var tomador = GenerateTomadorRps(nota);
                infServico.Add(tomador);
            }

            if (!nota.Intermediario.CpfCnpj.IsEmpty())
            {
                var intermediario = GenerateIntermediarioRps(nota);
                infServico.Add(intermediario);
            }

            if (!nota.ConstrucaoCivil.ArtObra.IsEmpty())
            {
                var construcao = GenerateConstrucaoCivilRps(nota);
                infServico.Add(construcao);
            }

            if (nota.RegimeEspecialTributacao != RegimeEspecialTributacao.Nenhum)
            {
                infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "RegimeEspecialTributacao", 1, 1, Ocorrencia.NaoObrigatoria,
                    (int)nota.RegimeEspecialTributacao));
            }

            infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "OptanteSimplesNacional", 1, 1, Ocorrencia.Obrigatoria,
                nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional ? 1 : 2));
            infServico.AddChild(AdicionarTag(TipoCampo.Int, "", "IncentivoFiscal", 1, 1, Ocorrencia.Obrigatoria, nota.IncentivadorCultural == NFSeSimNao.Sim ? 1 : 2));

            return rootRps;
        }

        protected virtual XElement GenerateServicosValoresRps(NotaFiscal nota)
        {
            var servico = new XElement("Servico");

            var valores = new XElement("Valores");
            servico.Add(valores);

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Servico.Valores.IssRetido++));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ResponsavelRetencao", 1, 1, Ocorrencia.NaoObrigatoria, (int)nota.Servico.ResponsavelRetencao++));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoPais));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ExigibilidadeISS", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Servico.ExigibilidadeIss++));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "MunicipioIncidencia", 7, 7, Ocorrencia.MaiorQueZero, nota.Servico.MunicipioIncidencia));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroProcesso", 0, 30, Ocorrencia.NaoObrigatoria, nota.Servico.NumeroProcesso));

            return servico;
        }

        protected virtual XElement GeneratePrestadorRps(NotaFiscal nota)
        {
            var prestador = new XElement("Prestador");

            var cpfCnpjPrestador = new XElement("CpfCnpj");
            prestador.Add(cpfCnpjPrestador);

            cpfCnpjPrestador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj));

            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Prestador.InscricaoMunicipal));

            return prestador;
        }

        protected virtual XElement GenerateTomadorRps(NotaFiscal nota)
        {
            var tomador = new XElement("Tomador");

            var ideTomador = new XElement("IdentificacaoTomador");
            tomador.Add(ideTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            ideTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            ideTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria, nota.Tomador.RazaoSocial));

            if (!nota.Tomador.Endereco.Logradouro.IsEmpty() ||
                !nota.Tomador.Endereco.Numero.IsEmpty() ||
                !nota.Tomador.Endereco.Complemento.IsEmpty() ||
                !nota.Tomador.Endereco.Bairro.IsEmpty() ||
                nota.Tomador.Endereco.CodigoMunicipio > 0 ||
                !nota.Tomador.Endereco.Uf.IsEmpty() ||
                nota.Tomador.Endereco.CodigoPais > 0 ||
                !nota.Tomador.Endereco.Cep.IsEmpty())
            {
                var endereco = new XElement("Endereco");
                tomador.Add(endereco);

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria,
                    nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria,
                    nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria,
                    nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria,
                    nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero,
                    nota.Tomador.Endereco.CodigoMunicipio));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero,
                    nota.Tomador.Endereco.CodigoPais));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria,
                    nota.Tomador.Endereco.Cep));
            }

            if (!nota.Tomador.DadosContato.DDD.IsEmpty() ||
                !nota.Tomador.DadosContato.Telefone.IsEmpty() ||
                !nota.Tomador.DadosContato.Email.IsEmpty())
            {
                var contato = new XElement("Contato");
                tomador.Add(contato);

                contato.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Telefone", 1, 11, Ocorrencia.NaoObrigatoria,
                    nota.Tomador.DadosContato.DDD + nota.Tomador.DadosContato.Telefone));
                contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 1, 80, Ocorrencia.NaoObrigatoria,
                    nota.Tomador.DadosContato.Email));
            }

            return tomador;
        }

        protected virtual XElement GenerateIntermediarioRps(NotaFiscal nota)
        {
            var intermediario = new XElement("Intermediario");

            var ideIntermediario = new XElement("IdentificacaoIntermediario");
            intermediario.Add(ideIntermediario);

            var cpfCnpj = new XElement("CpfCnpj");
            ideIntermediario.Add(cpfCnpj);

            cpfCnpj.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Intermediario.CpfCnpj));

            ideIntermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 15, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.InscricaoMunicipal));

            intermediario.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 115, Ocorrencia.NaoObrigatoria,
                nota.Intermediario.RazaoSocial));

            return intermediario;
        }

        protected virtual XElement GenerateConstrucaoCivilRps(NotaFiscal nota)
        {
            var construcao = new XElement("ConstrucaoCivil");

            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoObra", 1, 15, Ocorrencia.NaoObrigatoria, nota.ConstrucaoCivil.CodigoObra));
            construcao.AddChild(AdicionarTag(TipoCampo.Str, "", "Art", 1, 15, Ocorrencia.Obrigatoria, nota.ConstrucaoCivil.ArtObra));

            return construcao;
        }

        #endregion RPS

        #region NFSe

        public override string GetXmlNFSe(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
        {
            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            var compNfse = new XElement("CompNfse");

            compNfse.AddChild(GenerateNFSe(nota));
            compNfse.AddChild(GenerateNFSeCancelamento(nota));
            compNfse.AddChild(GenerateNFSeSubstituicao(nota));

            xmlDoc.AddChild(compNfse);
            return xmlDoc.AsString(identado, showDeclaration);
        }

        protected virtual XElement GenerateNFSe(NotaFiscal nota)
        {
            var nfse = new XElement("Nfse", new XAttribute("versao", "2.00"));

            var infNfse = GetInfoNFSe(nota);
            nfse.AddChild(infNfse);

            var valores = GenerateValoresNFse(nota);
            infNfse.AddChild(valores);

            infNfse.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCredito", 1, 15, Ocorrencia.MaiorQueZero, nota.ValorCredito));

            var endereco = GetEnderecoPrestador(nota);
            infNfse.AddChild(endereco);

            var orgao = GetOrgaoGerador(nota);
            infNfse.AddChild(orgao);

            var declaracao = GenerateDeclaracaoServicoNFSe(nota);
            infNfse.AddChild(declaracao);

            return nfse;
        }

        protected virtual XElement GenerateNFSeCancelamento(NotaFiscal nota)
        {
            if (nota.Situacao != SituacaoNFSeRps.Cancelado) return null;

            var cancelamento = new XElement("NfseCancelamento", new XAttribute("Versão", "2.00"));

            var cancConfirmacao = new XElement("Confirmacao", new XAttribute("Id", nota.Cancelamento.Id));
            cancelamento.AddChild(cancConfirmacao);

            var cancPedido = new XElement("Pedido");
            cancConfirmacao.AddChild(cancPedido);

            var cancInfPedido = new XElement("InfPedidoCancelamento", new XAttribute("Id", nota.Cancelamento.Id));
            cancPedido.AddChild(cancInfPedido);

            var cancIdNfSe = new XElement("IdentificacaoNfse");
            cancInfPedido.AddChild(cancIdNfSe);

            cancIdNfSe.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.Cancelamento.Pedido.IdentificacaoNFSe.Numero));
            cancIdNfSe.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Prestador.CpfCnpj.ZeroFill(14)));
            cancIdNfSe.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "InscricaoMunicipal", 1, 15, Ocorrencia.Obrigatoria, nota.Prestador.InscricaoMunicipal));
            cancIdNfSe.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoMunicipio", 1, 7, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.CodigoMunicipio));

            cancInfPedido.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "CodigoCancelamento", 1, 4, Ocorrencia.Obrigatoria, nota.Cancelamento.Pedido.CodigoCancelamento));

            cancPedido.AddChild(WriteSignature(nota.Cancelamento.Pedido.Signature));

            cancConfirmacao.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataHora", 20, 20, Ocorrencia.Obrigatoria, nota.Cancelamento.DataHora));

            return cancelamento;
        }

        protected virtual XElement GenerateNFSeSubstituicao(NotaFiscal nota)
        {
            if (nota.RpsSubstituido.NFSeSubstituidora.IsEmpty()) return null;

            var substituidora = new XElement("NfseSubstituicao", new XAttribute("Versão", "2.00"));
            var subNFSe = new XElement("SubstituicaoNfse", new XAttribute("Id", nota.RpsSubstituido.Id));
            substituidora.AddChild(subNFSe);

            subNFSe.AddChild(AdicionarTag(TipoCampo.Int, "", "NfseSubstituidora", 1, 15, Ocorrencia.Obrigatoria, nota.RpsSubstituido.NFSeSubstituidora));
            subNFSe.AddChild(WriteSignature(nota.RpsSubstituido.Signature));

            return substituidora;
        }

        protected virtual XElement GetInfoNFSe(NotaFiscal nota)
        {
            var infNfse = new XElement("InfNfse", new XAttribute("Id", $"{nota.IdentificacaoNFSe.Numero.OnlyNumbers()}"));

            infNfse.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Numero", 1, 15, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Numero));
            infNfse.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoVerificacao", 1, 5, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.Chave));
            infNfse.AddChild(AdicionarTag(TipoCampo.DatHor, "", "DataEmissao", 1, 1, Ocorrencia.Obrigatoria, nota.IdentificacaoNFSe.DataEmissao));

            infNfse.AddChild(AdicionarTag(TipoCampo.Str, "", "NfseSubstituida", 1, 15, Ocorrencia.NaoObrigatoria, nota.RpsSubstituido.NumeroNfse));
            infNfse.AddChild(AdicionarTag(TipoCampo.Str, "", "OutrasInformacoes", 1, 255, Ocorrencia.NaoObrigatoria, nota.OutrasInformacoes));

            return infNfse;
        }

        protected virtual XElement GetEnderecoPrestador(NotaFiscal nota)
        {
            if (!nota.Prestador.Endereco.Logradouro.IsEmpty() ||
                !nota.Prestador.Endereco.Numero.IsEmpty() ||
                !nota.Prestador.Endereco.Complemento.IsEmpty() ||
                !nota.Prestador.Endereco.Bairro.IsEmpty() ||
                nota.Prestador.Endereco.CodigoMunicipio > 0 ||
                !nota.Prestador.Endereco.Uf.IsEmpty() ||
                nota.Prestador.Endereco.CodigoPais > 0 ||
                !nota.Prestador.Endereco.Cep.IsEmpty())
            {
                var endereco = new XElement("EnderecoPrestadorServico");

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.MaiorQueZero, nota.Prestador.Endereco.CodigoMunicipio));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero, nota.Prestador.Endereco.CodigoPais));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Cep));

                return endereco;
            }

            return null;
        }

        protected virtual XElement GetOrgaoGerador(NotaFiscal nota)
        {
            var orgao = new XElement("OrgaoGerador");

            orgao.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria, nota.OrgaoGerador.CodigoMunicipio));
            orgao.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.Obrigatoria, nota.OrgaoGerador.Uf));

            return orgao;
        }

        protected virtual XElement GenerateValoresNFse(NotaFiscal nota)
        {
            var valores = new XElement("ValoresNfse");

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "BaseCalculo", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.BaseCalculo));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "Aliquota", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorLiquidoNfse", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorLiquidoNfse));

            return valores;
        }

        protected virtual XElement GenerateDeclaracaoServicoNFSe(NotaFiscal nota)
        {
            var declaracao = GenerateRps(nota);
            declaracao.ReplaceWith(new XElement("DeclaracaoPrestacaoServico"));
            return declaracao;
        }

        #endregion NFSe

        #region Protected Methods

        protected virtual string GetNamespace()
        {
            return "http://www.abrasf.org.br/nfse";
        }

        protected virtual string GetSchema(TipoUrl tipo)
        {
            return "nfse.xsd";
        }

        #endregion Protected Methods

        #endregion Methods
    }
}