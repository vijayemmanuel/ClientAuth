using System;
using System.Collections.Generic;
using System.Ling;
using System.Text;
using WebClientUtils;
using System.ServiceModel.Description;

namespace WebClientUtils.ServiceModel
{
    public class ClientBaseInternal<TChannel> : System.ServiceModel.ClientBase<TChannel> where TChannel : class
    {
        public CIientBaseInternal(System. ServiceModel.Description.ServiceEndpoint oEp) : base(oEp.Binding, oEp.Address)
        {
            foreach ( System.ServiceModel.Description.IEndpointBehavior oBeh in oEp.Behaviors )
            {
                if ( ! base.Endpoint.Behaviors.Contains (oBeh) )
                    base. Endpoint.Behaviors.Add(oBeh);
            }
        }

        public ClientBaseInternal(string endpointConfigurationName) : base(endpointConfigurationName) {}

        public ClientBaseInternal(string endpointConfigurationName, string remoteAddress): base(endpointConfigurationName, remoteAddress) {}

        public ClientBaseInternal(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress) {}

        public ClientBaseInternal(System. ServiceModel.Channels.Binding binding, System. ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress) {}
    }

    public class ClientBase<TChannel> : ClientBaseInternal<TChannel> where TChannel : class
    {
        public ClientBase() : base(ConfigServices.GetServiceEndpoint(typeof(TChannel))) {}
        
        public ClientBase(string endpointConfigurationName) : base(endpointConfigurationName) {}

        public ClientBase(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress) {}

        public ClientBase(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress) {}
        
        public C1ientBase (System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base (binding, remoteAddress) {}
    }
}


