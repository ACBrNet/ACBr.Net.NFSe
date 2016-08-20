using ACBr.Net.Core.Extensions;
using ACBr.Net.Core.Logging;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Linq;

namespace ACBr.Net.NFSe.Providers
{
	internal class BaseMessageInspector : IClientMessageInspector, IACBrLog
	{
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var xdoc = MessageToXml(request);
			this.Log().Debug(xdoc.AsString());
			return null;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			var xdoc = MessageToXml(reply);
			this.Log().Debug(xdoc.AsString());
		}

		private static XDocument MessageToXml(Message message)
		{
			var msgbuf = message.CreateBufferedCopy(int.MaxValue);
			var nav = msgbuf.CreateNavigator();

			//load the old message into xmldocument
			using (var ms = new MemoryStream())
			{
				using (var xw = XmlWriter.Create(ms))
				{
					nav.WriteSubtree(xw);
					xw.Flush();
					xw.Close();
				}

				ms.Position = 0;
				return XDocument.Load(XmlReader.Create(ms));
			}
		}
	}
}