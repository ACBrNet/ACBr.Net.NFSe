// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-31-2016
//
// Last Modified By : RFTD
// Last Modified On : 07-05-2018
// ***********************************************************************
// <copyright file="ACBrNFSe.cs" company="ACBr.Net">
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
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using ACBr.Net.NFSe.Providers;
using System;
using System.ComponentModel;
using System.Net;
using ACBr.Net.Core.Logging;

#if NETFULL

using System.Drawing;

#endif

namespace ACBr.Net.NFSe
{
    // ReSharper disable once InconsistentNaming
    [TypeConverter(typeof(ACBrExpandableObjectConverter))]
#if NETFULL

    [ToolboxBitmap(typeof(ACBrNFSe), "ACBr.Net.NFSe.ACBrNFSe.bmp")]
#endif
    public sealed class ACBrNFSe : ACBrComponent, IACBrLog
    {
        #region Fields

        private SecurityProtocolType protocolType;

        #endregion Fields

        #region Propriedades

        /// <summary>
        /// Configurações do Componente
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ConfigNFSe Configuracoes { get; private set; }

        /// <summary>
        /// Componente de impressão
        /// </summary>
        [Browsable(false)]
        public ACBrDANFSeBase DANFSe { get; set; }

        /// <summary>
        /// Coleção de NFSe para processar e/ou processadas
        /// </summary>
        [Browsable(false)]
        public NotaFiscalCollection NotasFiscais { get; private set; }

        #endregion Propriedades

        #region Methods

        /// <summary>
        /// Envia as NFSe para o provedor da cidade.
        /// </summary>
        /// <param name="lote">Numero do lote.</param>
        /// <param name="sincrono">Se for informado <c>true</c> o envio será sincrono.</param>
        /// <param name="imprimir">Se for informado <c>true</c> imprime as RPS, se o envio foi executado com sucesso.</param>
        /// <returns>RetornoWebservice.</returns>
        public RetornoWebservice Enviar(int lote, bool sincrono = false, bool imprimir = false)

        {
            Guard.Against<ACBrException>(NotasFiscais.Count < 1, "ERRO: Nenhuma RPS adicionada ao Lote");

            Guard.Against<ACBrException>(NotasFiscais.Count > 50,
                $"ERRO: Conjunto de RPS transmitidos (máximo de 50 RPS) excedido.{Environment.NewLine}" +
                $"Quantidade atual: {NotasFiscais.Count}");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = protocolType;
                using (var provider = ProviderManager.GetProvider(Configuracoes))
                {
                    var ret = sincrono
                        ? provider.EnviarSincrono(lote, NotasFiscais)
                        : provider.Enviar(lote, NotasFiscais);

                    if (ret.Sucesso && DANFSe != null && imprimir)
                        DANFSe.Imprimir();

                    return ret;
                }
            }
            catch (Exception exception)
            {
                this.Log().Error("[Enviar]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Consulta a situação do lote de RPS.
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="lote">The lote.</param>
        /// <param name="protocolo">The protocolo.</param>
        /// <returns>RetornoWebservice.</returns>
        public RetornoWebservice ConsultarSituacao(int lote, string protocolo = "")
        {
            Guard.Against<ArgumentException>(lote < 1, "Lote não pode ser Zero ou negativo.");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = protocolType;
                using (var provider = ProviderManager.GetProvider(Configuracoes))
                {
                    return provider.ConsultarSituacao(lote, protocolo);
                }
            }
            catch (Exception exception)
            {
                this.Log().Error("[ConsultarSituacao]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Consulta o lote de Rps
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="protocolo">The protocolo.</param>
        /// <param name="lote">The lote.</param>
        /// <returns>RetornoWebservice.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public RetornoWebservice ConsultarLoteRps(int lote, string protocolo)
        {
            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = protocolType;
                using (var provider = ProviderManager.GetProvider(Configuracoes))
                {
                    return provider.ConsultarLoteRps(lote, protocolo, NotasFiscais);
                }
            }
            catch (Exception exception)
            {
                this.Log().Error("[ConsultarLoteRps]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Consulta o numero de sequencia dos lotes de RPS.
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="serie">The serie.</param>
        /// <returns>RetornoWebservice.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public RetornoWebservice ConsultarSequencialRps(string serie)
        {
            Guard.Against<ArgumentNullException>(serie.IsEmpty(), "Serie não pode ser vazia ou nulo.");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = protocolType;
                using (var provider = ProviderManager.GetProvider(Configuracoes))
                {
                    return provider.ConsultarSequencialRps(serie);
                }
            }
            catch (Exception exception)
            {
                this.Log().Error("[ConsultarSequencialRps]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Consulta a NFSe/RPS que atende os filtros informados.
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="numero">The numero.</param>
        /// <param name="serie">The serie.</param>
        /// <param name="tipo">The tipo.</param>
        /// <returns>RetornoWebservice.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public RetornoWebservice ConsultaNFSeRps(string numero, string serie, TipoRps tipo)
        {
            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = protocolType;
                using (var provider = ProviderManager.GetProvider(Configuracoes))
                {
                    return provider.ConsultaNFSeRps(numero, serie, tipo, NotasFiscais);
                }
            }
            catch (Exception exception)
            {
                this.Log().Error("[ConsultaNFSeRps]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Consulta as NFSe de acordo com os filtros.
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="inicio">The inicio.</param>
        /// <param name="fim">The fim.</param>
        /// <param name="numeroNfse">The numero nfse.</param>
        /// <param name="pagina">The pagina.</param>
        /// <param name="cnpjTomador">The CNPJ tomador.</param>
        /// <param name="imTomador">The im tomador.</param>
        /// <param name="nomeInter">The nome inter.</param>
        /// <param name="cnpjInter">The CNPJ inter.</param>
        /// <param name="imInter">The im inter.</param>
        /// <param name="serie">The serie.</param>
        /// <returns>RetornoWebservice.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public RetornoWebservice ConsultaNFSe(DateTime? inicio, DateTime? fim, string numeroNfse = "", int pagina = 1,
            string cnpjTomador = "", string imTomador = "", string nomeInter = "", string cnpjInter = "", string imInter = "", string serie = "")
        {
            Guard.Against<ACBrException>(inicio?.Date > fim?.Date, "A data inicial não pode ser maior que a data final.");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = protocolType;
                using (var provider = ProviderManager.GetProvider(Configuracoes))
                {
                    return provider.ConsultaNFSe(inicio, fim, numeroNfse, pagina, cnpjTomador,
                        imTomador, nomeInter, cnpjInter, imInter, serie, NotasFiscais);
                }
            }
            catch (Exception exception)
            {
                this.Log().Error("[ConsultaNFSe]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Cancela uma NFSe
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="codigoCancelamento">O codigo de cancelamento.</param>
        /// <param name="numeroNFSe">O numero da NFSe.</param>
        /// <param name="motivo">O motivo.</param>
        /// <returns>RetornoWebservice.</returns>
        public RetornoWebservice CancelaNFSe(string codigoCancelamento, string numeroNFSe, string motivo)
        {
            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = protocolType;
                using (var provider = ProviderManager.GetProvider(Configuracoes))
                {
                    return provider.CancelaNFSe(codigoCancelamento, numeroNFSe, motivo, NotasFiscais);
                }
            }
            catch (Exception exception)
            {
                this.Log().Error("[CancelaNFSe]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Cancela as NFSe que estão carregadas na lista.
        ///
        /// Obs.: Adicionar o motivo de cancelamento nas notas da lista.
        ///       Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="lote">Identificação do lote.</param>
        /// <returns>RetornoWebservice.</returns>
        public RetornoWebservice CancelaNFSe(int lote)
        {
            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = protocolType;
                Guard.Against<ArgumentException>(NotasFiscais.Count < 1, "ERRO: Nenhuma NFS-e carregada ao componente");

                using (var provider = ProviderManager.GetProvider(Configuracoes))
                {
                    return provider.CancelaNFSe(lote, NotasFiscais);
                }
            }
            catch (Exception exception)
            {
                this.Log().Error("[CancelaNFSe]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Substitui uma NFSe
        ///
        /// Obs.: Nem todos provedores suportam este metodo.
        /// </summary>
        /// <param name="codigoCancelamento">O codigo de cancelamento.</param>
        /// <param name="numeroNFSe">O numero da NFSe.</param>
        /// <param name="motivo">O motivo.</param>
        /// <returns>RetornoWebservice.</returns>
        public RetornoWebservice SubstituirNFSe(string codigoCancelamento, string numeroNFSe, string motivo)
        {
            Guard.Against<ArgumentException>(codigoCancelamento.IsEmpty(), "ERRO: Código de Cancelamento não informado");
            Guard.Against<ArgumentException>(numeroNFSe.IsEmpty(), "ERRO: Numero da NFS-e não informada");
            Guard.Against<ArgumentException>(NotasFiscais.Count < 1, "ERRO: Nenhuma RPS carregada ao componente");

            var oldProtocol = ServicePointManager.SecurityProtocol;

            try
            {
                ServicePointManager.SecurityProtocol = protocolType;
                using (var provider = ProviderManager.GetProvider(Configuracoes))
                {
                    return provider.SubstituirNFSe(codigoCancelamento, numeroNFSe, motivo, NotasFiscais);
                }
            }
            catch (Exception exception)
            {
                this.Log().Error("[SubstituirNFSe]", exception);
                throw;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = oldProtocol;
            }
        }

        /// <summary>
        /// Imprime o DANFse
        /// </summary>
        public void Imprimir()
        {
            Guard.Against<ArgumentNullException>(DANFSe == null, "Nenhum componente de impressão especificado.");
            DANFSe?.Imprimir();
        }

        /// <summary>
        /// Imprime o PDF do DANFse
        /// </summary>
        public void ImprimirPDF()
        {
            Guard.Against<ArgumentNullException>(DANFSe == null, "Nenhum componente de impressão especificado.");
            DANFSe?.ImprimirPDF();
        }

        #region Override Methods

        /// <summary>
        /// Função executada na inicialização do componente
        /// </summary>
        protected override void OnInitialize()
        {
            Configuracoes = new ConfigNFSe(this);
            NotasFiscais = new NotaFiscalCollection(Configuracoes);
            protocolType = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                           SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Função executada na desinicialização do componente
        /// </summary>
        protected override void OnDisposing()
        {
        }

        #endregion Override Methods

        #endregion Methods
    }
}