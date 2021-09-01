// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : Diego Martins
// Created          : 08-30-2021
//
// ***********************************************************************
// <copyright file="ProviderBase.cs" company="ACBr.Net">
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
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
using System.Linq;

namespace ACBr.Net.NFSe.Providers
{
    public abstract class ProviderRestBase : ProviderBase
    {
        protected ProviderRestBase(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {

        }

        public override RetornoEnviar EnviarSincrono(int lote, NotaServicoCollection notas)
        {
            var retornoWebservice = new RetornoEnviar()
            {
                Lote = lote,
                Sincrono = true
            };

            PrepararEnviarSincrono(retornoWebservice, notas);
            if (retornoWebservice.Erros.Count > 0) return retornoWebservice;

            if (Configuracoes.Geral.RetirarAcentos)
                retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

            GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"EnviarSincrono-{lote}-env.xml");

            // Verifica Schema
            if (PrecisaValidarSchema(TipoUrl.EnviarSincrono))
            {
                ValidarSchema(retornoWebservice, GetSchema(TipoUrl.EnviarSincrono));
                if (retornoWebservice.Erros.Any()) return retornoWebservice;
            }

            // Recebe mensagem de retorno
            try
            {
                using (var cliente = GetClient(TipoUrl.EnviarSincrono))
                {
                    retornoWebservice.XmlRetorno = cliente.EnviarSincrono(GerarCabecalho(), retornoWebservice.XmlEnvio);
                    retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                    retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
                }
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }

            GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"EnviarSincrono-{lote}-ret.xml");
            TratarRetornoEnviarSincrono(retornoWebservice, notas);

            return retornoWebservice;
        }

        public override RetornoConsultarNFSeRps ConsultaNFSeRps(int numero, string serie, TipoRps tipo, NotaServicoCollection notas, int anoCompetencia, int mesCompetencia)
        {
            var retornoWebservice = new RetornoConsultarNFSeRps()
            {
                NumeroRps = numero,
                Serie = serie,
                Tipo = tipo,
                AnoCompetencia = anoCompetencia,
                MesCompetencia = mesCompetencia
            };

            try
            {
                PrepararConsultarNFSeRps(retornoWebservice, notas);
                if (retornoWebservice.Erros.Any()) return retornoWebservice;

                if (Configuracoes.Geral.RetirarAcentos)
                    retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

                GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"ConsultarNFSeRps-{numero}-{serie}-env.xml");

                // Verifica Schema
                if (PrecisaValidarSchema(TipoUrl.ConsultarNFSeRps))
                {
                    ValidarSchema(retornoWebservice, GetSchema(TipoUrl.ConsultarNFSeRps));
                    if (retornoWebservice.Erros.Any()) return retornoWebservice;
                }

                // Recebe mensagem de retorno
                using (var cliente = GetClient(TipoUrl.ConsultarNFSeRps))
                {
                    retornoWebservice.XmlRetorno = cliente.ConsultarNFSeRps(GerarCabecalho(), retornoWebservice.XmlEnvio);
                    retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                    retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
                }

                GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"ConsultarNFSeRps-{numero}-{serie}-ret.xml");
                TratarRetornoConsultarNFSeRps(retornoWebservice, notas);
                return retornoWebservice;
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
        }

        public override RetornoCancelar CancelarNFSe(string codigoCancelamento, string numeroNFSe, string serieNFSe, decimal valorNFSe, string motivo, NotaServicoCollection notas)
        {
            var retornoWebservice = new RetornoCancelar()
            {
                CodigoCancelamento = codigoCancelamento,
                NumeroNFSe = numeroNFSe,
                SerieNFSe = serieNFSe,
                ValorNFSe = valorNFSe,
                Motivo = motivo
            };

            try
            {
                PrepararCancelarNFSe(retornoWebservice);
                if (retornoWebservice.Erros.Any()) return retornoWebservice;

                if (Configuracoes.Geral.RetirarAcentos)
                    retornoWebservice.XmlEnvio = retornoWebservice.XmlEnvio.RemoveAccent();

                GravarArquivoEmDisco(retornoWebservice.XmlEnvio, $"CancelarNFSe-{numeroNFSe}-env.xml");

                // Verifica Schema
                if (PrecisaValidarSchema(TipoUrl.CancelarNFSe))
                {
                    ValidarSchema(retornoWebservice, GetSchema(TipoUrl.CancelarNFSe));
                    if (retornoWebservice.Erros.Any()) return retornoWebservice;
                }

                // Recebe mensagem de retorno

                using (var cliente = GetClient(TipoUrl.CancelarNFSe))
                {
                    retornoWebservice.XmlRetorno = cliente.CancelarNFSe(GerarCabecalho(), retornoWebservice.XmlEnvio);
                    retornoWebservice.EnvelopeEnvio = cliente.EnvelopeEnvio;
                    retornoWebservice.EnvelopeRetorno = cliente.EnvelopeRetorno;
                }

                GravarArquivoEmDisco(retornoWebservice.XmlRetorno, $"CancelarNFSe-{numeroNFSe}-ret.xml");
                TratarRetornoCancelarNFSe(retornoWebservice, notas);
                return retornoWebservice;
            }
            catch (Exception ex)
            {
                retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = ex.Message });
                return retornoWebservice;
            }
        }

        protected override string GerarCabecalho()
        {
            return string.Empty;
        }

        #region não implementados

        public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            throw new NotImplementedException();
        }

        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException();
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            throw new NotImplementedException();
        }

        #endregion não implementados
    }
}