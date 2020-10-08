using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Nota;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Providers.Sigiss
{
    internal sealed class ProviderSigiss : ProviderBase
    {
        #region Internal Types

        #endregion Internal Types

        #region Fields

        #endregion Fields

        #region Constructors

        public ProviderSigiss(ConfigNFSe config, ACBrMunicipioNFSe municipio) : base(config, municipio)
        {
        }

        #endregion Constructors

        #region Methods

        #region Public

        public override NotaServico LoadXml(XDocument xml)
        {
            throw new NotImplementedException("Metodo LoadXml não implementado");
        }

        public override string WriteXmlRps(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            var notaTag = new XElement("DescricaoRps");
            xmldoc.Add(notaTag);

            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "ccm", 1, 120, Ocorrencia.Obrigatoria, Configuracoes.WebServices.Usuario)); //mesmo que nota.Prestador.InscricaoMunicipal
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cnpj", 1, 14, Ocorrencia.Obrigatoria, nota.Prestador.CpfCnpj.OnlyNumbers()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "senha", 1, 120, Ocorrencia.Obrigatoria, Configuracoes.WebServices.Senha));
            //notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "crc", 1, 120, Ocorrencia.NaoObrigatoria, "")); //nao utilizado
            //notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "crc_estado", 1, 120, Ocorrencia.NaoObrigatoria, "")); //nao utilizado
            if (nota.RegimeEspecialTributacao == RegimeEspecialTributacao.SimplesNacional)
            {
                //obrigatorio apenas para simplesnacional
                notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "aliquota_simples", 1, 11, Ocorrencia.Obrigatoria, nota.Servico.Valores.Aliquota));
            }
            notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "id_sis_legado", 1, 15, Ocorrencia.NaoObrigatoria, nota.NumeroLote)); //nao utilizado
            notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "servico", 1, 20, Ocorrencia.Obrigatoria, nota.Servico.CodigoTributacaoMunicipio));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "situacao", 2, 2, Ocorrencia.Obrigatoria, NaturezaOperacao.Sigiss.GetValue(nota.NaturezaOperacao)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "valor", 1, 15, Ocorrencia.Obrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorServicos)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "base", 1, 15, Ocorrencia.Obrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.BaseCalculo)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "descricaoNF", 0, 2000, Ocorrencia.NaoObrigatoria, nota.Servico.Descricao.RemoveAccent()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_tipo", 14, 14, Ocorrencia.NaoObrigatoria, nota.Tomador.Tipo)); //TipoTomador
            if (nota.Tomador.Tipo != TipoTomador.Sigiss.PFNI)
            {
                notaTag.AddChild(AdicionarTagCNPJCPF("", "tomador_cnpj", "tomador_cnpj", nota.Tomador.CpfCnpj ?? string.Empty));
            }
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_email", 1, 120, Ocorrencia.NaoObrigatoria, nota.Tomador.DadosContato.Email));
            notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "tomador_im", 1, 120, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoMunicipal.OnlyNumbers()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_ie", 1, 120, Ocorrencia.NaoObrigatoria, nota.Tomador.InscricaoEstadual.OnlyNumbers()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_razao", 1, 120, Ocorrencia.Obrigatoria, RetirarAcentos ? nota.Tomador.RazaoSocial.RemoveAccent() : nota.Tomador.RazaoSocial));
            //notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_fantasia", 1, 120, Ocorrencia.NaoObrigatoria, string.Empty)); //não utilizado
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_endereco", 1, 120, Ocorrencia.NaoObrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Logradouro.RemoveAccent() : nota.Tomador.Endereco.Logradouro));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_numero", 1, 9, Ocorrencia.NaoObrigatoria, string.IsNullOrEmpty(nota.Tomador.Endereco.Numero) ? "S/N" : nota.Tomador.Endereco.Numero));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_complemento", 1, 30, Ocorrencia.NaoObrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Complemento.RemoveAccent() : nota.Tomador.Endereco.Complemento));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_bairro", 1, 50, Ocorrencia.NaoObrigatoria, RetirarAcentos ? nota.Tomador.Endereco.Bairro.RemoveAccent() : nota.Tomador.Endereco.Bairro));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_CEP", 1, 8, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.Cep.OnlyNumbers()));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_cod_cidade", 1, 10, Ocorrencia.NaoObrigatoria, nota.Tomador.Endereco.CodigoMunicipio));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_fone", 0, 15, Ocorrencia.Obrigatoria, nota.Tomador.DadosContato.DDD.OnlyNumbers() + nota.Tomador.DadosContato.Telefone.OnlyNumbers()));
            //notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_ramal", 0, 15, Ocorrencia.Obrigatoria, string.Empty)); //não utilizado
            //notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "tomador_fax", 0, 15, Ocorrencia.Obrigatoria, string.Empty)); //não utilizado

            if (!string.IsNullOrEmpty(nota.IdentificacaoRps.Numero) && !string.IsNullOrEmpty(nota.IdentificacaoRps.Serie))
            {
                notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "rps_num", 1, 12, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Numero));
                notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "rps_serie", 1, 20, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.Serie));
                notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "rps_dia", 1, 2, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.Day));
                notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "rps_mes", 1, 2, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.Month));
                notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "rps_ano", 1, 4, Ocorrencia.Obrigatoria, nota.IdentificacaoRps.DataEmissao.Year));
            }

            if (nota.Tomador.Tipo == TipoTomador.Sigiss.JuridicaForaMunicipio)
            {
                notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "outro_municipio", 1, 1, Ocorrencia.Obrigatoria, nota.Servico.MunicipioIncidencia));
                if (nota.Servico.MunicipioIncidencia == 1)
                {
                    notaTag.AddChild(AdicionarTag(TipoCampo.Int, "", "cod_outro_municipio", 1, 10, Ocorrencia.Obrigatoria, nota.Servico.CodigoMunicipio));
                }
                if (nota.Servico.Valores.ValorIssRetido > 0)
                {
                    notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "retencao_iss", 1, 15, Ocorrencia.Obrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorIssRetido)));
                }
            }

            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "pis", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorPis)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "cofins", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorCofins)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "inss", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorInss)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "irrf", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorIr)));
            notaTag.AddChild(AdicionarTag(TipoCampo.Str, "", "csll", 1, 15, Ocorrencia.NaoObrigatoria, FormataDecimalModeloSigiss(nota.Servico.Valores.ValorCsll)));

            return xmldoc.Root.AsString(identado, showDeclaration, Encoding.UTF8);
        }

        public override string WriteXmlNFSe(NotaServico nota, bool identado = true, bool showDeclaration = true)
        {
            throw new NotImplementedException("Metodo WriteXmlNFSe não implementado");
        }

        #endregion Public

        #region Services

        //GerarNotaRequest
        protected override void PrepararEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            if (notas.Count > 1) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "Apenas o envio de uma nota por vez é permitido para esse serviço." });
            if (notas.Count == 0) retornoWebservice.Erros.Add(new Evento { Codigo = "0", Descricao = "RPS não informado." });
            var nota = notas.FirstOrDefault() ?? throw new Exception("Nenhuma nota para ser enviada");

            var xmlRps = WriteXmlRps(nota, false, false);
            GravarRpsEmDisco(xmlRps, $"Rps-{nota.IdentificacaoRps.DataEmissao:yyyyMMdd}-{nota.IdentificacaoRps.Numero}.xml", nota.IdentificacaoRps.DataEmissao);

            var xmlLote = new StringBuilder();
            xmlLote.Append(xmlRps);

            retornoWebservice.XmlEnvio = xmlLote.ToString();
        }

        //GerarNotaRequest
        protected override void AssinarEnviar(RetornoEnviar retornoWebservice)
        {
            //não assina
        }

        //GerarNotaResponse
        protected override void TratarRetornoEnviar(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var root = xmlRet.ElementAnyNs("GerarNotaResponse");
            var data = root.ElementAnyNs("RetornoNota") ?? throw new Exception("Elemento do xml RetornoNota não encontado");
            var errors = root.ElementAnyNs("DescricaoErros") ?? throw new Exception("Elemento do xml DescricaoErros não encontado");

            var resultado = data.ElementAnyNs("Resultado")?.GetValue<int>() ?? 0;
            var protocolo = data.ElementAnyNs("autenticidade")?.GetValue<string>() ?? string.Empty;
            //var linkImpressao = data.ElementAnyNs("LinkImpressao")?.GetValue<string>() ?? string.Empty; //não utilizado pois não tem como retornar

            if (resultado != 1 && errors.HasElements)
            {
                retornoWebservice.Sucesso = false;
                foreach (var node in errors.Descendants().Where(x => x.Name == "item"))
                {
                    var errorId = node.ElementAnyNs("id")?.Value ?? string.Empty;
                    var errorProcesso = node.ElementAnyNs("DescricaoProcesso")?.Value ?? string.Empty;
                    var errorDescricao = node.ElementAnyNs("DescricaoErro")?.Value ?? string.Empty;
                    retornoWebservice.Erros.Add(new Evento() { Codigo = errorId, Correcao = errorProcesso, Descricao = errorDescricao });
                }
            }
            else
            {
                retornoWebservice.Sucesso = true;
                retornoWebservice.Lote = 0; //Não tem lote nesse serviço
                retornoWebservice.Protocolo = protocolo;
            }
        }


        //CancelarNota
        protected override void PrepararCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            var xmldoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            var cancelamentoXml = new XElement("DadosCancelaNota");
            xmldoc.Add(cancelamentoXml);

            cancelamentoXml.AddChild(AdicionarTag(TipoCampo.Int, "", "ccm", 1, 120, Ocorrencia.Obrigatoria, Configuracoes.WebServices.Usuario.OnlyNumbers()));
            cancelamentoXml.AddChild(AdicionarTag(TipoCampo.Str, "", "cnpj", 1, 14, Ocorrencia.Obrigatoria, Configuracoes.PrestadorPadrao.CpfCnpj.OnlyNumbers()));
            cancelamentoXml.AddChild(AdicionarTag(TipoCampo.Str, "", "senha", 1, 120, Ocorrencia.Obrigatoria, Configuracoes.WebServices.Senha));
            cancelamentoXml.AddChild(AdicionarTag(TipoCampo.Int, "", "nota", 1, 15, Ocorrencia.Obrigatoria, retornoWebservice.NumeroNFSe.OnlyNumbers()));
            cancelamentoXml.AddChild(AdicionarTag(TipoCampo.Str, "", "motivo", 1, 3000, Ocorrencia.Obrigatoria, retornoWebservice.Motivo));
            cancelamentoXml.AddChild(AdicionarTag(TipoCampo.Str, "", "email", 1, 120, Ocorrencia.NaoObrigatoria, retornoWebservice.CodigoCancelamento));

            retornoWebservice.XmlEnvio = cancelamentoXml.ToString();
        }

        //CancelarNota Não utiliza
        protected override void AssinarCancelarNFSe(RetornoCancelar retornoWebservice)
        {
            //não assina
        }

        //CancelarNota
        protected override void TratarRetornoCancelarNFSe(RetornoCancelar retornoWebservice, NotaServicoCollection notas)
        {
            var xmlRet = XDocument.Parse(retornoWebservice.XmlRetorno);
            var root = xmlRet.ElementAnyNs("CancelarNotaResponse");
            var data = root.ElementAnyNs("RetornoNota") ?? throw new Exception("Elemento do xml RetornoNota não encontado");
            var errors = root.ElementAnyNs("DescricaoErros") ?? throw new Exception("Elemento do xml DescricaoErros não encontado");

            var resultado = data.ElementAnyNs("Resultado")?.GetValue<int>() ?? 0;
            //var linkImpressao = data.ElementAnyNs("LinkImpressao")?.GetValue<string>() ?? string.Empty; //não utilizado pois não tem como retornar

            if (resultado != 1 && errors.HasElements)
            {
                retornoWebservice.Sucesso = false;
                foreach (var node in errors.Descendants().Where(x => x.Name == "item"))
                {
                    var errorId = node.ElementAnyNs("id")?.Value ?? string.Empty;
                    var errorProcesso = node.ElementAnyNs("DescricaoProcesso")?.Value ?? string.Empty;
                    var errorDescricao = node.ElementAnyNs("DescricaoErro")?.Value ?? string.Empty;
                    retornoWebservice.Erros.Add(new Evento() { Codigo = errorId, Correcao = errorProcesso, Descricao = errorDescricao });
                }
            }
            else
            {
                retornoWebservice.Sucesso = true;
            }
        }


        #region Não Utilizados

        //ConsultarNotaValida - provedor suporta porem não implementado por conta da dificuldade de dados para procurar nota.
        protected override void PrepararConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada neste Provedor.");
        }

        //ConsultarNotaValida
        protected override void AssinarConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada neste Provedor.");
        }

        //ConsultarNotaValida - provedor suporta porem  não implementado por conta da dificuldade de dados para procurar nota.
        protected override void TratarRetornoConsultarSituacao(RetornoConsultarSituacao retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada neste Provedor.");
        }

        protected override void PrepararConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
        }

        protected override void AssinarConsultarNFSe(RetornoConsultarNFSe retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
        }

        protected override void TratarRetornoConsultarNFSe(RetornoConsultarNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
        }

        protected override void PrepararEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
        }

        protected override void AssinarEnviarSincrono(RetornoEnviar retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
        }

        protected override void TratarRetornoEnviarSincrono(RetornoEnviar retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor.");
        }

        protected override void PrepararConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void TratarRetornoConsultarLoteRps(RetornoConsultarLoteRps retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void PrepararConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void TratarRetornoConsultarSequencialRps(RetornoConsultarSequencialRps retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void PrepararConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void TratarRetornoConsultarNFSeRps(RetornoConsultarNFSeRps retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void PrepararCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void TratarRetornoCancelarNFSeLote(RetornoCancelarNFSeLote retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void PrepararSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void AssinarSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        protected override void TratarRetornoSubstituirNFSe(RetornoSubstituirNFSe retornoWebservice, NotaServicoCollection notas)
        {
            throw new NotImplementedException("Função não implementada/suportada neste Provedor !");
        }

        #endregion Não Utilizados

        #endregion Services

        #region Private

        protected override IServiceClient GetClient(TipoUrl tipo)
        {
            return new SigissServiceClient(this, tipo);
        }

        protected override string GerarCabecalho()
        {
            return "";
        }

        protected override string GetSchema(TipoUrl tipo)
        {
            //esse servidor nao tem schemas até o momento
            switch (tipo)
            {
                case TipoUrl.Enviar:
                    return "";

                case TipoUrl.CancelarNFSe:
                    return "";

                default:
                    throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
            }
        }

        protected override bool PrecisaValidarSchema(TipoUrl tipo)
        {
            //esse servidor nao tem schemas até o momento
            return false;
        }

        private string FormataDecimalModeloSigiss(decimal valor)
        {
            var formatado = valor.ToString("0.00").Replace(".", ",");
            var trim = formatado.Contains(",") ? formatado.TrimEnd('0').TrimEnd(',') : formatado;
            return trim;
        }

        #endregion Private

        #endregion Methods
    }
}
