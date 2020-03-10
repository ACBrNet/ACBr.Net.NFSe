// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : Adriano Trentim
// Created          : 22-01-2020
//
// Last Modified By : Rafael Dias
// Last Modified On : 06-02-2020
// ***********************************************************************
// <copyright file="ProviderABRASF204.cs" company="ACBr.Net">
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

using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Classe base para trabalhar com provedores que usam o padrão ABRASF 2.04
    /// </summary>
    /// <seealso cref="ProviderBase" />
    public abstract class ProviderABRASF204 : ProviderABRASF202
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderABRASF204"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="municipio">The municipio.</param>
        protected ProviderABRASF204(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
            Name = "ABRASFv204";
            Versao = "2.04";
            UsaPrestadorEnvio = true;
        }

        #endregion Constructors

        #region Methods

        #region LoadXml

        /// <inheritdoc />
        protected override void LoadTomador(NotaFiscal nota, XElement rpsRoot)
        {
            // Tomador
            var rootTomador = rpsRoot.ElementAnyNs("TomadorServico");
            if (rootTomador == null) return;

            var tomadorIdentificacao = rootTomador.ElementAnyNs("IdentificacaoTomador");
            if (tomadorIdentificacao != null)
            {
                nota.Tomador.CpfCnpj = tomadorIdentificacao.ElementAnyNs("CpfCnpj")?.GetCPF_CNPJ();
                nota.Tomador.InscricaoMunicipal = tomadorIdentificacao.ElementAnyNs("InscricaoMunicipal")?.GetValue<string>() ?? string.Empty;
            }

            nota.Tomador.DocTomadorEstrangeiro = rootTomador.ElementAnyNs("NifTomador")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.RazaoSocial = rootTomador.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;

            var endereco = rootTomador.ElementAnyNs("Endereco");
            if (endereco != null)
            {
                nota.Tomador.Endereco.Logradouro = endereco.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                nota.Tomador.Endereco.Numero = endereco.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                nota.Tomador.Endereco.Complemento = endereco.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                nota.Tomador.Endereco.Bairro = endereco.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                nota.Tomador.Endereco.CodigoMunicipio = endereco.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                nota.Tomador.Endereco.Uf = endereco.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                nota.Tomador.Endereco.CodigoPais = endereco.ElementAnyNs("CodigoPais")?.GetValue<int>() ?? 0;
                nota.Tomador.Endereco.Cep = endereco.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
            }

            var enderecoExterior = rootTomador.ElementAnyNs("EnderecoExterior");
            if (enderecoExterior != null)
            {
                nota.Tomador.EnderecoExterior.CodigoPais = enderecoExterior.ElementAnyNs("CodigoPais")?.GetValue<int>() ?? 0;
                nota.Tomador.EnderecoExterior.EnderecoCompleto = enderecoExterior.ElementAnyNs("EnderecoCompletoExterior")?.GetValue<string>() ?? string.Empty;
            }

            var rootTomadorContato = rootTomador.ElementAnyNs("Contato");
            if (rootTomadorContato == null) return;

            nota.Tomador.DadosContato.DDD = "";
            nota.Tomador.DadosContato.Telefone = rootTomadorContato.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
            nota.Tomador.DadosContato.Email = rootTomadorContato.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
        }

        /// <inheritdoc />
        protected override void LoadPrestador(NotaFiscal nota, XElement rootNFSe)
        {
            // Endereco Prestador
            var prestadorServico = rootNFSe.ElementAnyNs("PrestadorServico");
            if (prestadorServico == null) return;

            nota.Prestador.RazaoSocial = prestadorServico.ElementAnyNs("RazaoSocial")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.NomeFantasia = prestadorServico.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;
            nota.Prestador.NomeFantasia = prestadorServico.ElementAnyNs("NomeFantasia")?.GetValue<string>() ?? string.Empty;

            // Endereco Prestador
            var enderecoPrestador = rootNFSe.ElementAnyNs("Endereco");
            if (enderecoPrestador != null)
            {
                nota.Prestador.Endereco.Logradouro = enderecoPrestador.ElementAnyNs("Endereco")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.Numero = enderecoPrestador.ElementAnyNs("Numero")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.Complemento = enderecoPrestador.ElementAnyNs("Complemento")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.Bairro = enderecoPrestador.ElementAnyNs("Bairro")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.CodigoMunicipio = enderecoPrestador.ElementAnyNs("CodigoMunicipio")?.GetValue<int>() ?? 0;
                nota.Prestador.Endereco.Uf = enderecoPrestador.ElementAnyNs("Uf")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.Endereco.Cep = enderecoPrestador.ElementAnyNs("Cep")?.GetValue<string>() ?? string.Empty;
            }

            // Contato Prestador
            var contatoPrestador = rootNFSe.ElementAnyNs("Contato");
            if (contatoPrestador != null)
            {
                nota.Prestador.DadosContato.Telefone = contatoPrestador.ElementAnyNs("Telefone")?.GetValue<string>() ?? string.Empty;
                nota.Prestador.DadosContato.Email = contatoPrestador.ElementAnyNs("Email")?.GetValue<string>() ?? string.Empty;
            }
        }

        #endregion LoadXml

        #region RPS

        protected override XElement WriteRpsRps(NotaFiscal nota)
        {
            var rps = new XElement("Rps");

            rps.Add(WriteIdentificacaoRps(nota));

            rps.AddChild(AdicionarTag(TipoCampo.Dat, "", "DataEmissao", 10, 10, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao));
            rps.AddChild(AdicionarTag(TipoCampo.Int, "", "Status", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Situacao + 1));

            rps.AddChild(WriteSubstituidoRps(nota));

            return rps;
        }

        protected override XElement WriteServicosRps(NotaFiscal nota)
        {
            var servico = new XElement("Servico");

            servico.Add(WriteValoresRps(nota));

            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "IssRetido", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.Valores.IssRetido == SituacaoTributaria.Retencao ? 1 : 2));

            if (nota.Servico.ResponsavelRetencao.HasValue)
                servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ResponsavelRetencao", 1, 1, Ocorrencia.NaoObrigatoria, (int)nota.Servico.ResponsavelRetencao + 1));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "ItemListaServico", 1, 5, Ocorrencia.Obrigatoria, nota.Servico.ItemListaServico));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoCnae", 1, 7, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoCnae));

            // Algumas prefeituras não permitem TAG Código de Tributacao
            // Sertãozinho/SP
            if (!Municipio.Codigo.IsIn(3551702))
                servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoTributacaoMunicipio", 1, 20, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoTributacaoMunicipio));

            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoNbs", 1, 9, Ocorrencia.NaoObrigatoria, nota.Servico.CodigoNbs));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "Discriminacao", 1, 2000, Ocorrencia.Obrigatoria, nota.Servico.Discriminacao));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "CodigoMunicipio", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 4, 4, Ocorrencia.MaiorQueZero, nota.Servico.CodigoPais));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "ExigibilidadeISS", 1, 1, Ocorrencia.Obrigatoria, (int)nota.Servico.ExigibilidadeIss + 1));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "IdentifNaoExigibilidade", 1, 4, Ocorrencia.NaoObrigatoria, nota.Servico.IdentifNaoExigibilidade));
            servico.AddChild(AdicionarTag(TipoCampo.Int, "", "MunicipioIncidencia", 7, 7, Ocorrencia.MaiorQueZero, nota.Servico.MunicipioIncidencia));
            servico.AddChild(AdicionarTag(TipoCampo.Str, "", "NumeroProcesso", 1, 30, Ocorrencia.NaoObrigatoria, nota.Servico.NumeroProcesso));

            return servico;
        }

        protected virtual XElement WriteValoresRps(NotaFiscal nota)
        {
            var valores = new XElement("Valores");

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorServicos", 1, 15, Ocorrencia.Obrigatoria, nota.Servico.Valores.ValorServicos));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorDeducoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorDeducoes));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorPis", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorPis));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCofins", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCofins));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorInss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorInss));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIr", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIr));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorCsll", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorCsll));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "OutrasRetencoes", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.OutrasRetencoes));

            // Algumas prefeituras não permitem TAG de ISSQN, pois são calculadas automaticamente pela prefeitura
            // Sertãozinho/SP
            if (!Municipio.Codigo.IsIn(3551702))
            {
                valores.AddChild(AdicionarTag(TipoCampo.De2, "", "ValorIss", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.ValorIss));
                valores.AddChild(AdicionarTag(TipoCampo.De4, "", "Aliquota", 1, 6, Ocorrencia.MaiorQueZero, nota.Servico.Valores.Aliquota));
            }

            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoIncondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoIncondicionado));
            valores.AddChild(AdicionarTag(TipoCampo.De2, "", "DescontoCondicionado", 1, 15, Ocorrencia.MaiorQueZero, nota.Servico.Valores.DescontoCondicionado));

            return valores;
        }

        protected override XElement WriteTomadorRps(NotaFiscal nota)
        {
            if (nota.Tomador.CpfCnpj.IsEmpty()) return null;

            var tomador = new XElement("TomadorServico");

            var idTomador = new XElement("IdentificacaoTomador");
            tomador.Add(idTomador);

            var cpfCnpjTomador = new XElement("CpfCnpj");
            idTomador.Add(cpfCnpjTomador);

            cpfCnpjTomador.AddChild(AdicionarTagCNPJCPF("", "Cpf", "Cnpj", nota.Tomador.CpfCnpj));

            idTomador.AddChild(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipal", 1, 150, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal));

            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "NifTomador", 1, 150, Ocorrencia.NaoObrigatoria, nota.Tomador.DocTomadorEstrangeiro));
            tomador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 150, Ocorrencia.Obrigatoria, nota.Tomador.RazaoSocial));

            if (nota.Tomador.EnderecoExterior.CodigoPais > 0)
            {
                var enderecoExt = new XElement("EnderecoExterior");
                tomador.Add(enderecoExt);

                enderecoExt.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoPais", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.CodigoPais));
                enderecoExt.AddChild(AdicionarTag(TipoCampo.Str, "", "EnderecoCompletoExterior", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.EnderecoExterior.EnderecoCompleto));
            }
            else
            {
                var endereco = new XElement("Endereco");
                tomador.Add(endereco);

                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Logradouro));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Numero));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Complemento));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Bairro));
                endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
                endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Uf));
                endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.Obrigatoria, nota.Tomador.Endereco.Cep));
            }

            if (nota.Prestador.DadosContato.Email.IsEmpty() && nota.Prestador.DadosContato.Telefone.IsEmpty()) return tomador;

            var contato = new XElement("Contato");
            tomador.Add(contato);

            contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Telefone", 8, 8, Ocorrencia.NaoObrigatoria, nota.Prestador.DadosContato.Telefone));
            contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 8, 8, Ocorrencia.NaoObrigatoria, nota.Prestador.DadosContato.Email));

            return tomador;
        }

        #endregion RPS

        #region NFSe

        protected override XElement WritePrestador(NotaFiscal nota)
        {
            var prestador = new XElement("PrestadorServico");
            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "RazaoSocial", 1, 150, Ocorrencia.Obrigatoria, nota.Prestador.RazaoSocial));
            prestador.AddChild(AdicionarTag(TipoCampo.Str, "", "NomeFantasia", 1, 60, Ocorrencia.NaoObrigatoria, nota.Prestador.NomeFantasia));

            var endereco = new XElement("Endereco");
            prestador.Add(endereco);

            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Endereco", 1, 125, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Logradouro));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Numero", 1, 10, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Numero));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Complemento", 1, 60, Ocorrencia.NaoObrigatoria, nota.Prestador.Endereco.Complemento));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Bairro", 1, 60, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Bairro));
            endereco.AddChild(AdicionarTag(TipoCampo.Int, "", "CodigoMunicipio", 7, 7, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.CodigoMunicipio));
            endereco.AddChild(AdicionarTag(TipoCampo.Str, "", "Uf", 2, 2, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Uf));
            endereco.AddChild(AdicionarTag(TipoCampo.StrNumber, "", "Cep", 8, 8, Ocorrencia.Obrigatoria, nota.Prestador.Endereco.Cep));

            if (nota.Prestador.DadosContato.Email.IsEmpty() && nota.Prestador.DadosContato.Telefone.IsEmpty()) return prestador;

            var contato = new XElement("Contato");
            prestador.Add(contato);

            contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Telefone", 8, 8, Ocorrencia.NaoObrigatoria, nota.Prestador.DadosContato.Telefone));
            contato.AddChild(AdicionarTag(TipoCampo.Str, "", "Email", 8, 8, Ocorrencia.NaoObrigatoria, nota.Prestador.DadosContato.Email));

            return prestador;
        }

        #endregion NFSe

        #endregion Methods
    }
}