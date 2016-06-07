// ***********************************************************************
// Assembly         : ACBr.Net.NFSe
// Author           : RFTD
// Created          : 01-31-2016
//
// Last Modified By : RFTD
// Last Modified On : 06-07-2016
// ***********************************************************************
// <copyright file="CfgCertificados.cs" company="ACBr.Net">
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
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core;

namespace ACBr.Net.NFSe.Configuracao
{
    /// <summary>
    /// Class NFECFGCertificados. This class cannot be inherited.
    /// </summary>
    public sealed class CfgCertificados
	{
        #region Fields

        private DateTime dataVenc;
        private string subjectName;
        private string cnpj;

        #endregion Fields
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CfgCertificados"/> class.
        /// </summary>
		internal CfgCertificados()
        {
            dataVenc = DateTime.MinValue;
            Certificado = string.Empty;
            subjectName = string.Empty;
            cnpj = string.Empty;
        }

		#endregion Constructor

        #region Properties

        /// <summary>
        /// Retorna ou estabelece o certificado ou Numero de Serie.
        /// </summary>
        /// <value>O Certificado/Numero de Serie.</value>
        [Browsable(true)]
        public string Certificado { get; set; }

        /// <summary>
        /// Retorna ou estabelece a senha do certificado.
        /// </summary>
        /// <value>A senha.</value>
        [Browsable(true)]
        public string Senha { get; set; }

        /// <summary>
        /// Retorna ou estabelece o tipo de certificado a utilizar.
        /// </summary>
        /// <value>Tipo de Certificado.</value>
        [Browsable(true)]
        public TipoCertificado Tipo { get; set; }

        /// <summary>
        /// Retorna a data de vencimento do certificado.
        /// </summary>
        /// <value>A data de vencimento.</value>
        public DateTime DataVenc
        {
            get
            {
                if (dataVenc == DateTime.MinValue && !string.IsNullOrEmpty(Certificado.Trim()))
                    GetCertificado();

                return dataVenc;
            }
        }

        /// <summary>
        /// Gets or sets the name of the subject.
        /// </summary>
        /// <value>The name of the subject.</value>
        public string SubjectName
        {
            get
            {
                if (string.IsNullOrEmpty(subjectName.Trim()) && !string.IsNullOrEmpty(Certificado.Trim()))
                    GetCertificado();

                return subjectName;
            }
        }

        /// <summary>
        /// Gets or sets the CNPJ.
        /// </summary>
        /// <value>The CNPJ.</value>
        public string CNPJ
        {
            get
            {
                if (string.IsNullOrEmpty(cnpj.Trim()) && !string.IsNullOrEmpty(Certificado.Trim()))
                    GetCertificado();

                return cnpj;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Seleciona um certificado digital instalado na maquina retornando o numero de serie do mesmo.
        /// </summary>
        /// <returns>Numero de Serie.</returns>
        public string SelecionarCertificado()
        {
            var cert = CertificadoDigital.SelecionarCertificado(string.Empty);
            return cert.GetSerialNumberString();
        }

        /// <summary>
        /// Obters the certficado.
        /// </summary>
        /// <returns>X509Certificate2.</returns>
        internal X509Certificate2 ObterCertificado()
        {
            return Tipo == TipoCertificado.A1 ? CertificadoDigital.SelecionarCertificado(Certificado, Senha) :
                                                CertificadoDigital.SelecionarCertificado(Certificado);
        }

        /// <summary>
        /// Gets the certificado.
        /// </summary>
        private void GetCertificado()
        {
            var cert = ObterCertificado();
            dataVenc = cert.GetExpirationDateString().ToData();
            subjectName = cert.SubjectName.Name;

            foreach (var lines in from X509Extension extension in
                                      cert.Extensions
                                  select extension.Format(true) into s1
                                  select s1.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var t in lines)
                {
                    if (!t.Trim().StartsWith("2.16.76.1.3.3"))
                        continue;

                    var value = t.Substring(t.IndexOf('=') + 1);
                    var elements = value.Split(' ');
                    var cnpjBytes = new byte[14];
                    for (var j = 0; j < cnpjBytes.Length; j++)
                        cnpjBytes[j] = Convert.ToByte(elements[j + 2], 16);
                    cnpj = Encoding.UTF8.GetString(cnpjBytes);
                    break;
                }

                if (!string.IsNullOrEmpty(cnpj))
                    break;
            }
        }

        #endregion Methods
	}
}