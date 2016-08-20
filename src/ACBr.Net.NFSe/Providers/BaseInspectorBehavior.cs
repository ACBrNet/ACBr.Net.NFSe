using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ACBr.Net.NFSe.Providers
{
	internal class BaseInspectorBehavior : IEndpointBehavior
	{
		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			var messageInspector = new BaseMessageInspector();
			clientRuntime.MessageInspectors.Add(messageInspector);
		}
	}
}