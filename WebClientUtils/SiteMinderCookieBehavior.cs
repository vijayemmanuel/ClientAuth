using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

namespace WebClientUtils
{
    public class SMEndpointBehaviorExtensionElement : BehaviorExtensionElement
    {
        public SMEndpointBehaviourExtensionElement() {}
        public override Type BehaviorType
        {
            get {
                return typeof(SiteMinderCookieBehavior);
            }
        }

        protected override object CreateBehavior ()
        {
            return new SiteMinderCookieBehavior();
        }
    }

    public class SiteMinderCookieBehavior : IEndpointBehavior
    {
        public SiteMinderCookieBehavior()
        {

        }

        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameeters)
        {

        }

        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.ClientRuntime behavior)
        {
            behavior.MessageInspectors.Add(new CookieMessageInspector(serviceEndpoint.Address.ToString(), serviceEndpoint.Contract.Name));
        }

        public void ApplyDispatchBehavior(ServiceModel serviceEndpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint serviceEndpoint)
        {

        }
    }

    public class CookieMessageInspector : IClientMessageInspector 
    {
        private string endPointAddress;
        private string contractName;

        public CookieMessageInspector(string endPointName, string contractName)
        {
            this.endPointAddress = endPointAddress;
            this.contractName = contractName;
        }

        public void AfterReceiveReply ( ref System.ServiceModel.Channels.Message reply, object correlationState) {}

        public object BeforeSendRequest ( ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage;
            object httpRequestMessageObject;
            string cookieValue = "";
            List<Cookie> CookieList = new List<Cookie>();

            cookieValue = ConfigServices.GetConfig().AppSettings.Settings["COOKIE_SERVICE_TYPE"].Value;

            if ()
        }
    }

}