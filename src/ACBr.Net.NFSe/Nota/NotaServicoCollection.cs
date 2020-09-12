// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : Rafael Dias
// Created          : 10-01-2014
//
// Last Modified By : Rafael Dias
// Last Modified On : 10-01-2014
// ***********************************************************************
// <copyright path="NotaServicoCollection.cs" company="ACBr.Net">
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

using ACBr.Net.Core;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Collection;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Providers;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Nota
{
    public sealed class NotaServicoCollection : DFeCollection<NotaServico>
    {
        #region Fields

        private readonly ConfigNFSe config;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Inicializa uma nova instacia da classe <see cref="NotaServicoCollection" />.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public NotaServicoCollection(ConfigNFSe config)
        {
            Guard.Against<ACBrException>(config == null, "Configura��es n�o podem ser nulas");

            this.config = config;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Adiciona uma nova nota fiscal na cole��o.
        /// </summary>
        /// <returns>T.</returns>
        public override NotaServico AddNew()
        {
            var nota = new NotaServico(config.PrestadorPadrao);
            Add(nota);
            return nota;
        }

        /// <summary>
        /// Carrega a NFSe/RPS do arquivo.
        /// </summary>
        /// <param name="xml">caminho do arquivo XML ou string com o XML.</param>
        /// <param name="encoding">encoding do XML.</param>
        /// <returns>NotaServico carregada.</returns>
        public NotaServico Load(string xml, Encoding encoding = null)
        {
            using (var provider = ProviderManager.GetProvider(config))
            {
                var nota = provider.LoadXml(xml, encoding);
                Add(nota);
                return nota;
            }
        }

        /// <summary>
        /// Carrega a NFSe/RPS do xml.
        /// </summary>
        /// <param name="stream">Stream do XML.</param>
        /// <returns>NotaServico carregada.</returns>
        public NotaServico Load(Stream stream)
        {
            using (var provider = ProviderManager.GetProvider(config))
            {
                var nota = provider.LoadXml(stream);
                Add(nota);
                return nota;
            }
        }

        /// <summary>
        /// Carrega a NFSe/RPS do XMLDocument.
        /// </summary>
        /// <param name="xml">XMLDocument da NFSe/RPS.</param>
        /// <returns>NotaServico carregada.</returns>
        public NotaServico Load(XDocument xml)
        {
            using (var provider = ProviderManager.GetProvider(config))
            {
                var nota = provider.LoadXml(xml);
                Add(nota);
                return nota;
            }
        }

        /// <summary>
        /// Salvar o xml da Rps/NFSe no determinado arquivo
        /// </summary>
        /// <param name="nota">A nota para salvar</param>
        /// <param name="path">Caminho onde sera salvo o arquivo.</param>
        /// <returns></returns>
        public void Save(NotaServico nota, string path = "")
        {
            using (var provider = ProviderManager.GetProvider(config))
            {
                var isNFSe = nota.IdentificacaoNFSe.Numero.IsEmpty();

                var file = isNFSe ?
                      $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}-{nota.IdentificacaoRps.Serie}.xml" :
                      $"NFSe-{nota.IdentificacaoNFSe.Chave}-{nota.IdentificacaoNFSe.Numero}.xml";

                var xmlNota = isNFSe ? provider.WriteXmlRps(nota) : provider.WriteXmlNFSe(nota);

                if (path.IsEmpty())
                    path = config.Arquivos.PathSalvar;

                path = Path.Combine(path, file);

                var doc = XDocument.Parse(xmlNota);
                doc.Save(path, SaveOptions.OmitDuplicateNamespaces);
            }
        }

        /// <summary>
        /// Salvar o xml da Rps/NFSe no determinado arquivo
        /// </summary>
        /// <param name="nota">A nota para salvar</param>
        /// <param name="stream">Stream onde sera salvo o xml</param>
        /// <returns></returns>
        public void Save(NotaServico nota, Stream stream)
        {
            using (var provider = ProviderManager.GetProvider(config))
            {
                var xmlNota = nota.IdentificacaoNFSe.Numero.IsEmpty()
                    ? provider.WriteXmlRps(nota)
                    : provider.WriteXmlNFSe(nota);

                var doc = XDocument.Parse(xmlNota);
                doc.Save(stream, SaveOptions.OmitDuplicateNamespaces);
            }
        }

        /// <summary>
        /// Gera o Xml Da Rps
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public string GetXml(NotaServico nota)
        {
            using (var provider = ProviderManager.GetProvider(config))
            {
                return nota.IdentificacaoNFSe.Numero.IsEmpty() ? provider.WriteXmlRps(nota) : provider.WriteXmlNFSe(nota);
            }
        }

        #endregion Methods
    }
}