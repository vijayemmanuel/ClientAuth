using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

namespace WebClientUtils.EndpointBehaviorExtensionElement
{
    public class SMEndpointBehaviorExtensionElement : BehaviorExtensionElement
    {
        public SMEndpointBehaviorExtensionElement() {}
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

            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                if (string.IsNullOrEmpty(httpRequestMessage.Headers["Cookie"]))
                {
                    if (cookieValue == "1")
                    {
                        SAMLService samlService =  new SAMLService();
                        if (CookieList.Count == 0)
                        {
                            CookieList = samlService.GetSAMLCookieForUri(endPointAddress);
                        }
                        for (int i = 0; i < CookieList.Count; i++)
                        {
                            if (CookieList[i].Name.Contains("shib"))
                            {
                                string shibCookie = CookieList[i].Name + "=" + CookieList[i].Value;
                                httpRequestMessage.Headers["Cookie"] = shibCookie;
                            }
                        }
                    }
                    else if (cookieValue == "0")
                    {
                        string smCookie = WebCookieServices.GetSmCookieForUri(endPointAddress, this.contractName);
                        httpRequestMessage.Headers["Cookie"] = smCookie;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (cookieValue == "1")
                {
                    SAMLService samlService =  new SAMLService();
                    httpRequestMessage = new HttpRequestMessageProperty();
                    if (CookieList.Count == 0)
                    {
                        CookieList = samlService.GetSAMLCookieForUri(endPointAddress);
                    }
                    for (int i = 0; i < CookieList.Count; i++)
                    {
                        if (CookieList[i].Name.Contains("shib"))
                        {
                            string shibCookie = CookieList[i].Name + "=" + CookieList[i].Value;
                            httpRequestMessage.Headers["Cookie"] = shibCookie;
                        }
                    }
                    httpRequestMessage = new HttpRequestMessageProperty();
                    request.Properties.Add(httpRequestMessageProperty.Name, httpRequestMessage);
                }
                else if (cookieValue == "0")
                {
                    string smCookie = WebCookieServices.GetSmCookieForUri(endPointAddress, this.contractName);
                    HttpRequestMessageProperty httpRequestMessage = new HttpRequestMessageProperty();
                    httpRequestMessage.Headers["Cookie"] = smCookie;
                    request.Properties.Add(httpRequestMessageProperty.Name, httpRequestMessage);
                }
                else
                {
                    return null;
                }
                

            }
            return null;
        }
    }

}