using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace ACBr.Net.NFSe.Providers
{
    internal abstract class NFSeSOAP11Charset8859ServiceClient : NFSeSOAP11ServiceClient
    {
        public NFSeSOAP11Charset8859ServiceClient(ProviderBase provider, TipoUrl tipoUrl, bool https = false) : base(provider, tipoUrl)
        {
            CustomBinding binding = new CustomBinding(new CustomTextMessageBindingElement("iso-8859-1", "text/xml", MessageVersion.Soap11), https ? new HttpsTransportBindingElement() : new HttpTransportBindingElement()); //Or  HttpsTransportBindingElement
            Endpoint.Binding = binding;
        }

        public NFSeSOAP11Charset8859ServiceClient(ProviderBase provider, TipoUrl tipoUrl, X509Certificate2 certificado, bool https = false) : base(provider, tipoUrl, certificado)
        {
            CustomBinding binding = new CustomBinding(new CustomTextMessageBindingElement("iso-8859-1", "text/xml", MessageVersion.Soap11), https ? new HttpsTransportBindingElement() : new HttpTransportBindingElement()); //Or  HttpsTransportBindingElement
            Endpoint.Binding = binding;
        }
    }

    public class CustomTextMessageBindingElement : MessageEncodingBindingElement
    {
        private MessageVersion msgVersion;
        private string mediaType;
        private string encoding;
        private XmlDictionaryReaderQuotas readerQuotas;

        private CustomTextMessageBindingElement(CustomTextMessageBindingElement binding)
            : this(binding.Encoding, binding.MediaType, binding.MessageVersion)
        {
            readerQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.CopyTo(readerQuotas);
        }

        public CustomTextMessageBindingElement(string encoding, string mediaType,
            MessageVersion msgVersion)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            if (mediaType == null)
            {
                throw new ArgumentNullException("mediaType");
            }

            if (msgVersion == null)
            {
                throw new ArgumentNullException("msgVersion");
            }

            this.msgVersion = msgVersion;
            this.mediaType = mediaType;
            this.encoding = encoding;
            readerQuotas = new XmlDictionaryReaderQuotas();
        }

        public CustomTextMessageBindingElement(string encoding, string mediaType)
            : this(encoding, mediaType, MessageVersion.Soap11WSAddressingAugust2004)
        {
        }

        public CustomTextMessageBindingElement(string encoding)
            : this(encoding, "text/xml")
        {

        }

        public CustomTextMessageBindingElement()
            : this("UTF-8")
        {
        }


        public override MessageVersion MessageVersion
        {
            get => msgVersion;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                msgVersion = value;
            }
        }


        public string MediaType
        {
            get => mediaType;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                mediaType = value;
            }
        }

        public string Encoding
        {
            get => encoding;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                encoding = value;
            }
        }

        // This encoder does not enforces any quotas for the unsecure messages. The  
        // quotas are enforced for the secure portions of messages when this encoder 
        // is used in a binding that is configured with security.  
        public XmlDictionaryReaderQuotas ReaderQuotas => readerQuotas;

        #region IMessageEncodingBindingElement Members 

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new CustomTextMessageEncoderFactory(MediaType,
                Encoding, MessageVersion);
        }

        #endregion


        public override BindingElement Clone()
        {
            return new CustomTextMessageBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        /* public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) 
         { 
             if (context == null) 
                 throw new ArgumentNullException("context"); 

             context.BindingParameters.Add(this); 
             return context.BuildInnerChannelListener<TChannel>(); 
         } 

         public override bool CanBuildChannelListener<TChannel>(BindingContext context) 
         { 
             if (context == null) 
                 throw new ArgumentNullException("context"); 

             context.BindingParameters.Add(this); 
             return context.CanBuildInnerChannelListener<TChannel>(); 
         } */

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return (T)(object)readerQuotas;
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        #region IWsdlExportExtension Members
        #endregion
    }

    public class CustomTextMessageEncoderFactory : MessageEncoderFactory
    {
        private MessageEncoder encoder;
        private MessageVersion version;
        private string mediaType;
        private string charSet;

        internal CustomTextMessageEncoderFactory(string mediaType, string charSet,
            MessageVersion version)
        {
            this.version = version;
            this.mediaType = mediaType;
            this.charSet = charSet;
            encoder = new CustomTextMessageEncoder(this);
        }

        public override MessageEncoder Encoder => encoder;

        public override MessageVersion MessageVersion => version;

        internal string MediaType => mediaType;

        internal string CharSet => charSet;
    }

    public class CustomTextMessageEncoder : MessageEncoder
    {
        private CustomTextMessageEncoderFactory factory;
        private XmlWriterSettings writerSettings;
        private string contentType;

        public CustomTextMessageEncoder(CustomTextMessageEncoderFactory factory)
        {
            this.factory = factory;

            writerSettings = new XmlWriterSettings();
            writerSettings.Encoding = Encoding.GetEncoding(factory.CharSet);
            contentType = $"{this.factory.MediaType}; charset={writerSettings.Encoding.HeaderName}";
        }

        public override string ContentType => contentType;

        public override string MediaType => factory.MediaType;

        public override MessageVersion MessageVersion => factory.MessageVersion;

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            byte[] msgContents = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
            bufferManager.ReturnBuffer(buffer.Array);

            MemoryStream stream = new MemoryStream(msgContents);
            return ReadMessage(stream, int.MaxValue);
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            XmlReader reader = XmlReader.Create(stream);
            return Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion);
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(stream, writerSettings);
            message.WriteMessage(writer);
            writer.Close();

            byte[] messageBytes = stream.GetBuffer();
            int messageLength = (int)stream.Position;
            stream.Close();

            int totalLength = messageLength + messageOffset;
            byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

            ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
            return byteArray;
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            XmlWriter writer = XmlWriter.Create(stream, writerSettings);
            message.WriteMessage(writer);
            writer.Close();
        }
    }


}
