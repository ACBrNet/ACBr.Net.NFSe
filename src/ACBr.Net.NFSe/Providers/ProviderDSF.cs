// ***********************************************************************
// Assembly         : G2i.NFSe
// Author           : RFTD
// Created          : 10-02-2014
//
// Last Modified By : RFTD
// Last Modified On : 10-02-2014
// ***********************************************************************
// <copyright file="ProviderDSF.cs" company="ACBr.Net">
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Util;

namespace ACBr.Net.NFSe.Providers
{
    /// <summary>
    /// Class ProviderDSF. This class cannot be inherited.
    /// </summary>
    internal sealed class ProviderDSF : ProviderBase
    {
		#region Propriedades

	    public string Situacao { get; set; }

	    public string Recolhimento { get; set; }

		public string Tributacao { get; set; }

	    public string Operacao { get; set; }

	    public string Assinatura { get; set; }

		#endregion Propriedades

		#region Methods

		/// <summary>
		/// Gets the XML.
		/// </summary>
		/// <param name="nota">The nota.</param>
		/// <param name="identado">if set to <c>true</c> [identado].</param>
		/// <param name="showDeclaration">if set to <c>true</c> [show declaration].</param>
		/// <returns>System.String.</returns>
		public override string GetXmlRPS(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
        {
            Xmldoc.RemoveAll();
            var dec = Xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            Xmldoc.AppendChild(dec);

			GerarCampo(nota);
			var nfse = nota.NFSe;

			var rpsTag = Xmldoc.CreateElement("RPS");
            rpsTag.SetAttribute("Id", $"rps:{nfse.InfId.Id}");

			

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Assinatura", 1, 2000, 1, Assinatura));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalPrestador", 01, 11,  1, nfse.Prestador.InscricaoMunicipal.OnlyNumbers()));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "RazaoSocialPrestador", 1, 120, 1, RetirarAcentos ? nfse.Prestador.RazaoSocial.RemoveAccent() : nfse.Prestador.RazaoSocial));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoRPS", 1, 20,  1, "RPS"));
            
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SerieRPS", 01, 2, 1,
				nfse.IdentificacaoRps.Serie.IsEmpty() ? "NF" : nfse.IdentificacaoRps.Serie));

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroRPS", 1, 12, 1, nfse.IdentificacaoRps.Numero));
            rpsTag.AddTag(AdicionarTag(TipoCampo.DatHor, "", "DataEmissaoRPS", 1, 21,  1, nfse.IdentificacaoRps.DataEmissaoRps));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SituacaoRPS", 1, 1, 1, Situacao));
            if (!nfse.RpsSubstituido.NumeroRps.IsEmpty())
            {
                rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SerieRPSSubstituido", 0, 2, 1, "NF"));
                rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroRPSSubstituido", 0, 2, 1, nfse.RpsSubstituido.NumeroRps));
                rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroNFSeSubstituida", 0, 2, 1, nfse.RpsSubstituido.NumeroNfse));
                rpsTag.AddTag(AdicionarTag(TipoCampo.Dat, "", "DataEmissaoNFSeSubstituida", 0, 2, 1, nfse.RpsSubstituido.DataEmissaoNfseSubstituida));
            }

            rpsTag.AddTag(AdicionarTag(TipoCampo.Int, "", "SeriePrestacao", 01, 02, 1, 
                nfse.IdentificacaoRps.SeriePrestacao.IsEmpty() ? "99" : nfse.IdentificacaoRps.SeriePrestacao.OnlyNumbers()));

            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalTomador", 1, 11,  1, nfse.Tomador.InscricaoMunicipal.OnlyNumbers()));
            rpsTag.AddTag(AdicionarTagCNPJCPF("CPFCNPJTomador", "CPFCNPJTomador", nfse.Tomador.CpfCnpj));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "RazaoSocialTomador", 1, 120, 1, RetirarAcentos ? nfse.Tomador.RazaoSocial.RemoveAccent() : nfse.Tomador.RazaoSocial));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DocTomadorEstrangeiro", 0, 20,  1, nfse.Tomador.DocTomadorEstrangeiro));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoLogradouroTomador", 0, 10,  1, nfse.Tomador.Endereco.TipoLogradouro));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "LogradouroTomador", 1, 50, 1, RetirarAcentos ? nfse.Tomador.Endereco.Logradouro.RemoveAccent() : nfse.Tomador.Endereco.Logradouro));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroEnderecoTomador", 1, 9,  1, nfse.Tomador.Endereco.Numero));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "ComplementoEnderecoTomador", 1, 30, 0, RetirarAcentos ? nfse.Tomador.Endereco.Complemento.RemoveAccent() : nfse.Tomador.Endereco.Complemento));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoBairroTomador", 0, 10,  1, nfse.Tomador.Endereco.TipoBairro));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "BairroTomador", 1, 50, 1, RetirarAcentos ? nfse.Tomador.Endereco.Bairro.RemoveAccent() : nfse.Tomador.Endereco.Bairro));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CidadeTomador", 1, 10,  1, nfse.Tomador.Endereco.CodigoMunicipio));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CidadeTomadorDescricao", 1, 50, 1, RetirarAcentos ? nfse.Tomador.Endereco.Municipio.RemoveAccent() : nfse.Tomador.Endereco.Municipio));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CEPTomador", 1, 8,  1, nfse.Tomador.Endereco.CEP.OnlyNumbers()));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "EmailTomador", 1, 60,  1, nfse.Tomador.Contato.Email));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CodigoAtividade", 1, 9,  1, nfse.Servico.CodigoCnae));
            rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "AliquotaAtividade", 1, 11,  1, nfse.Servico.Valores.Aliquota));

			//valores serviço
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoRecolhimento", 01, 01,  1, Recolhimento)); 
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacao", 1, 10,  1, nfse.Servico.CodigoMunicipio.ZeroFill(7)));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacaoDescricao", 01, 30, 1, RetirarAcentos ? nfse.Servico.Municipio.RemoveAccent() : nfse.Servico.Municipio));
			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Operacao", 01, 01, 1, Operacao));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Tributacao",01, 01,  1, Tributacao));      
            
            //Valores
            rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorPIS", 1, 2, 1, nfse.Servico.Valores.ValorPis));
            rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorCOFINS", 1, 2, 1, nfse.Servico.Valores.ValorCofins));
            rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorINSS", 1, 2, 1, nfse.Servico.Valores.ValorInss));
            rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorIR", 1, 2, 1, nfse.Servico.Valores.ValorIR));
            rpsTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorCSLL", 1, 2, 1, nfse.Servico.Valores.ValorCsll));

            //Aliquotas
            rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaPIS", 1, 2, 1, nfse.Servico.Valores.AliquotaPis));
            rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaCOFINS", 1, 2, 1, nfse.Servico.Valores.AliquotaCofins));
            rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaINSS", 1, 2, 1, nfse.Servico.Valores.AliquotaInss));
            rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaIR", 1, 2, 1, nfse.Servico.Valores.AliquotaIR));
            rpsTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaCSLL", 1, 2, 1, nfse.Servico.Valores.AliquotaCsll));

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DescricaoRPS", 1, 1500, 1, RetirarAcentos ? nfse.Servico.Descricao.RemoveAccent() : nfse.Servico.Descricao));

			rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DDDPrestador", 0, 3, 1, nfse.Prestador.Contato.DDD.OnlyNumbers()));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TelefonePrestador", 0, 8, 1, nfse.Prestador.Contato.Telefone.OnlyNumbers()));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DDDTomador", 0, 03, 1, nfse.Tomador.Contato.DDD.OnlyNumbers()));
            rpsTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TelefoneTomador", 0, 8, 1, nfse.Tomador.Contato.Telefone.OnlyNumbers()));

			if (!nfse.IntermediarioServico.CpfCnpj.IsEmpty())
                rpsTag.AddTag(AdicionarTagCNPJCPF("CPFCNPJIntermediario", "CPFCNPJIntermediario", nfse.IntermediarioServico.CpfCnpj));


			rpsTag.AddTag(GerarServicos(nfse.Servico.ItensServico));
            if (nfse.Servico.Deducoes.Count > 0)
                rpsTag.AddTag(GerarDeducoes(nfse.Servico.Deducoes));

            Xmldoc.AddTag(rpsTag);
            return Xmldoc.AsString(identado, showDeclaration);
        }

        /// <summary>
        /// Gets the XML.
        /// </summary>
        /// <param name="nota">The nota.</param>
        /// <param name="identado">if set to <c>true</c> [identado].</param>
        /// <param name="showDeclaration">if set to <c>true</c> [show declaration].</param>
        /// <returns>System.String.</returns>
        public override string GetXmlNFSe(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
        {
            Xmldoc.RemoveAll();
            var dec = Xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            Xmldoc.AppendChild(dec);

			GerarCampo(nota);
			var nfse = nota.NFSe;

			var notaTag = Xmldoc.CreateElement("Nota");
            notaTag.AddTag(AdicionarTag(TipoCampo.Int, "", "NumeroNota", 1, 11, 1, nfse.Numero));
            notaTag.AddTag(AdicionarTag(TipoCampo.DatHor, "", "DataProcessamento", 1, 21, 1, nfse.DhRecebimento));
            notaTag.AddTag(AdicionarTag(TipoCampo.Int, "", "NumeroLote", 1, 11, 1, nfse.NumeroLote));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CodigoVerificacao", 1, 200, 1, nfse.CodigoVerificacao));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Assinatura", 1, 2000, 1, nfse.Assinatura));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalPrestador", 01, 11,  1, nfse.Prestador.InscricaoMunicipal.OnlyNumbers()));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "RazaoSocialPrestador", 1, 120, 1, RetirarAcentos ? nfse.Prestador.RazaoSocial.RemoveAccent() : nfse.Prestador.RazaoSocial));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoRPS", 1, 20,  1, "RPS"));
            
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SerieRPS", 01, 02, 1,
				nfse.IdentificacaoRps.Serie.IsEmpty() ? "NF" : nfse.IdentificacaoRps.Serie));

			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroRPS", 1, 12, 1, nfse.IdentificacaoRps.Serie));
            notaTag.AddTag(AdicionarTag(TipoCampo.DatHor, "", "DataEmissaoRPS", 1, 21,  1, nfse.IdentificacaoRps.DataEmissaoRps));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SituacaoRPS", 1, 1, 1, Situacao));
            if (!nfse.RpsSubstituido.NumeroRps.IsEmpty())
            {
                notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "SerieRPSSubstituido", 0, 2, 1, "NF"));
                notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroRPSSubstituido", 0, 2, 1, nfse.RpsSubstituido.NumeroRps));
                notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroNFSeSubstituida", 0, 2, 1, nfse.RpsSubstituido.NumeroNfse));
                notaTag.AddTag(AdicionarTag(TipoCampo.Dat, "", "DataEmissaoNFSeSubstituida", 0, 2, 1, nfse.RpsSubstituido.DataEmissaoNfseSubstituida));
            }

            notaTag.AddTag(AdicionarTag(TipoCampo.Int, "", "SeriePrestacao", 1, 2, 1,
				nfse.IdentificacaoRps.SeriePrestacao.IsEmpty() ? "99" : nfse.IdentificacaoRps.SeriePrestacao));

			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "InscricaoMunicipalTomador", 1, 11, 1, nfse.Tomador.InscricaoMunicipal.OnlyNumbers()));
            notaTag.AddTag(AdicionarTagCNPJCPF("CPFCNPJTomador", "CPFCNPJTomador", nfse.Tomador.CpfCnpj));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "RazaoSocialTomador", 1, 120, 1, RetirarAcentos ? nfse.Tomador.RazaoSocial.RemoveAccent() : nfse.Tomador.RazaoSocial));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DocTomadorEstrangeiro", 0, 20,  1, nfse.Tomador.DocTomadorEstrangeiro));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoLogradouroTomador", 0, 10,  1, nfse.Tomador.Endereco.TipoLogradouro));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "LogradouroTomador", 1, 50, 1, RetirarAcentos ? nfse.Tomador.Endereco.Logradouro.RemoveAccent() : nfse.Tomador.Endereco.Logradouro));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroEnderecoTomador", 1, 9,  1, nfse.Tomador.Endereco.Numero));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "ComplementoEnderecoTomador", 1, 30, 0, RetirarAcentos ? nfse.Tomador.Endereco.Complemento.RemoveAccent() : nfse.Tomador.Endereco.Complemento));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoBairroTomador", 0, 10,  1, nfse.Tomador.Endereco.TipoBairro));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "BairroTomador", 1, 50,  1, RetirarAcentos ? nfse.Tomador.Endereco.Bairro.RemoveAccent() : nfse.Tomador.Endereco.Bairro));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CidadeTomador", 1, 10,  1, nfse.Tomador.Endereco.CodigoMunicipio));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CidadeTomadorDescricao", 1, 50, 1, RetirarAcentos ? nfse.Tomador.Endereco.Municipio.RemoveAccent() : nfse.Tomador.Endereco.Municipio));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CEPTomador", 1, 8,  1, nfse.Tomador.Endereco.CEP.OnlyNumbers()));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "EmailTomador", 1, 60,  1, nfse.Tomador.Contato.Email));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CodigoAtividade", 1, 9,  1, nfse.Servico.CodigoCnae));
            notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "AliquotaAtividade", 1, 11,  1, nfse.Servico.Valores.Aliquota));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoRecolhimento",01, 01,  1, Recolhimento)); 
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacao",          01, 10,  1, nfse.Servico.CodigoMunicipio.ZeroFill(7)));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MunicipioPrestacaoDescricao", 01, 30, 1, RetirarAcentos ? nfse.Servico.Municipio.RemoveAccent() : nfse.Servico.Municipio));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Operacao", 01, 01,  1, Operacao));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Tributacao",01, 01,  1, Tributacao));      
            
            //Valores
            notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorPIS", 1, 2, 1, nfse.Servico.Valores.ValorPis));
            notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorCOFINS", 1, 2, 1, nfse.Servico.Valores.ValorCofins));
            notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorINSS", 1, 2, 1, nfse.Servico.Valores.ValorInss));
            notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorIR", 1, 2, 1, nfse.Servico.Valores.ValorIR));
            notaTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorCSLL", 1, 2, 1, nfse.Servico.Valores.ValorCsll));

            //Aliquotas criar propriedades
            notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaPIS", 1, 2, 1, nfse.Servico.Valores.AliquotaPis));
            notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaCOFINS", 1, 2, 1, nfse.Servico.Valores.AliquotaCofins));
            notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaINSS", 1, 2, 1, nfse.Servico.Valores.AliquotaInss));
            notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaIR", 1, 2, 1, nfse.Servico.Valores.AliquotaIR));
            notaTag.AddTag(AdicionarTag(TipoCampo.De4, "", "AliquotaCSLL", 1, 2, 1, nfse.Servico.Valores.AliquotaCsll));

            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DescricaoRPS", 1, 1500, 1, RetirarAcentos ? nfse.Servico.Descricao.RemoveAccent() : nfse.Servico.Descricao));

			notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DDDPrestador", 0, 3, 1, nfse.Prestador.Contato.DDD.OnlyNumbers()));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TelefonePrestador", 0, 8, 1, nfse.Prestador.Contato.Telefone.OnlyNumbers()));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DDDTomador", 0, 03, 1, nfse.Tomador.Contato.DDD.OnlyNumbers()));
            notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TelefoneTomador", 0, 8, 1, nfse.Tomador.Contato.Telefone.OnlyNumbers()));

			if (nfse.Status == StatusRps.Cancelado)
                notaTag.AddTag(AdicionarTag(TipoCampo.Str, "", "MotCancelamento", 1, 80, 1, RetirarAcentos ? nfse.MotivoCancelamto.RemoveAccent() : nfse.MotivoCancelamto));

			if (!nfse.IntermediarioServico.CpfCnpj.IsEmpty())
                notaTag.AddTag(AdicionarTagCNPJCPF("CPFCNPJIntermediario", "CPFCNPJIntermediario", nfse.IntermediarioServico.CpfCnpj));

			notaTag.AddTag(GerarServicos(nfse.Servico.ItensServico));
            if (nfse.Servico.Deducoes.Count > 0)
                notaTag.AddTag(GerarDeducoes(nfse.Servico.Deducoes));

            Xmldoc.AddTag(notaTag);
            return Xmldoc.AsString(identado, showDeclaration);
        }

        #endregion Methods

        #region Private Methods

	    private void GerarCampo(NotaFiscal nota)
	    {
		    var nfse = nota.NFSe;
			
			Recolhimento = nfse.Servico.Valores.IssRetido == SituacaoTributaria.Normal ? "A" : "R";
			Situacao = nfse.Status == StatusRps.Normal ? "N" : "C";
			
			switch (nfse.NaturezaOperacao)
			{
				case NaturezaOperacao.NO3:
				case NaturezaOperacao.NO4:
					Operacao = "C";
					break;

				case NaturezaOperacao.NO7:
					Operacao = "A";
					break;

				default:
					Operacao = nfse.DeducaoMateriais == NFSeSimNao.Sim ? "B" : "A";
					break;
			}

			switch (nfse.TipoTributacao)
			{
				case TipoTributacao.Isenta:
					Tributacao = "C";
					break;

				case TipoTributacao.Imune:
					Tributacao = "F";
					break;

				case TipoTributacao.DepositoEmJuizo:
					Tributacao = "K";
					break;

				case TipoTributacao.NaoIncide:
					Tributacao = "E";
					break;
					
				case TipoTributacao.NaoTributavel:
					Tributacao = "N";
					break;

				case TipoTributacao.TributavelFixo:
					Tributacao = "G";
					break;

				//Tributavel
				default:
					Tributacao = "T";
					break;
			}

			if(nfse.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
				Tributacao = "H";

			if(nfse.RegimeEspecialTributacao == RegimeEspecialTributacao.MicroEmpresarioIndividual)
				Tributacao = "M";

			var valor = nfse.Servico.Valores.ValorServicos - nfse.Servico.Valores.ValorDeducoes;
			var rec = nfse.Servico.Valores.IssRetido == SituacaoTributaria.Normal ? "N" : "S";
			var assinatura = $"{nfse.Prestador.InscricaoMunicipal.ZeroFill(11)}{nfse.IdentificacaoRps.Serie.FillLeft(5)}" +
							 $"{nfse.IdentificacaoRps.Numero.ZeroFill(12)}{nfse.IdentificacaoRps.DataEmissaoRps:yyyyMMdd}{Tributacao} " +
							 $"{Situacao}{rec}{Math.Round(valor * 100).ToString().ZeroFill(15)}" +
							 $"{Math.Round(nfse.Servico.Valores.ValorDeducoes * 100).ToString().ZeroFill(15)}" +
							 $"{nfse.Servico.CodigoCnae.ZeroFill(10)}{nfse.Tomador.CpfCnpj.ZeroFill(14)}";

			Assinatura = assinatura.ToSha1Hash().ToLowerInvariant();
		}

        private XmlElement GerarServicos(IEnumerable<Servico> servicos)
        {
            var itensTag = Xmldoc.CreateElement("Itens");

            foreach (var servico in servicos)
            {
                var itemTag = Xmldoc.CreateElement("Item");
                var sTributavel = servico.Tributavel == NFSeSimNao.Sim ? "S" : "N";
                itemTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DiscriminacaoServico", 1, 80, 1, RetirarAcentos ? servico.Descricao.RemoveAccent() : servico.Descricao));
                itemTag.AddTag(AdicionarTag(TipoCampo.De4, "", "Quantidade", 1, 15, 1, servico.Quantidade));
                itemTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorUnitario", 1, 20, 1, servico.ValorUnitario));
                itemTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorTotal", 1, 18, 1, servico.ValorTotal));
                itemTag.AddTag(AdicionarTag(TipoCampo.Str, "", "Tributavel", 1, 1, 0, sTributavel));
                itensTag.AddTag(itemTag);
            }

            return itensTag;
        }

        private XmlElement GerarDeducoes(IEnumerable<Deducao> deducoes)
        {
            var deducoesTag = Xmldoc.CreateElement("Deducoes");
            foreach (var deducao in deducoes)
            {
                var deducaoTag = Xmldoc.CreateElement("Deducao");
                deducaoTag.AddTag(AdicionarTag(TipoCampo.Str, "", "DeducaoPor", 1, 20, 1, deducao.DeducaoPor.ToString() )); 
                deducaoTag.AddTag(AdicionarTag(TipoCampo.Str, "", "TipoDeducao", 0, 255, 1, deducao.TipoDeducao.GetStr(
					new [] { TipoDeducao.Nenhum,  TipoDeducao.Materiais, TipoDeducao.Mercadorias,
							 TipoDeducao.SubEmpreitada, TipoDeducao.VeiculacaoeDivulgacao, TipoDeducao.MapadeConstCivil,
							 TipoDeducao.Servicos}, 
					new []{ "", "Despesas com Materiais", "Despesas com Mercadorias",
							"Despesas com Subempreitada", "Servicos de Veiculacao e Divulgacao", "Mapa de Const. Civil",
							"Servicos"
					})));

                deducaoTag.AddTag(AdicionarTag(TipoCampo.Str, "", "CPFCNPJReferencia", 0, 14, 1, deducao.CPFCNPJReferencia.OnlyNumbers()));
                deducaoTag.AddTag(AdicionarTag(TipoCampo.Str, "", "NumeroNFReferencia", 0, 10, 1, deducao.NumeroNFReferencia));
                deducaoTag.AddTag(AdicionarTag(TipoCampo.De2, "", "ValorTotalReferencia", 0, 18, 1, deducao.ValorTotalReferencia));
                deducaoTag.AddTag(AdicionarTag(TipoCampo.De2, "", "PercentualDeduzir", 0, 8, 1, deducao.PercentualDeduzir));
                deducoesTag.AddTag(deducaoTag);
            }

            return deducoesTag;
        }

        #endregion Private Methods
    }
}