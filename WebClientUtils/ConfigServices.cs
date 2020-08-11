using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.ServiceModel.Description;

namespace WebClientUtils
{
    public class ConfigServices
    {
        private static string sConfigFileEnv = "WEBCLIENTSETTINGSFILE";
        static System.Configuration.Configuration config = null;

        public static string _servername = "";

        public static ServiceEndpoint GetServiceEndpoint (Type oContractType)
        {
            ServiceEndpoint oSEP = null;
            // Ensure that the Config file already open
            if ( !InitConfig() )
                return oSEP;

            ContractDescription desc = ContractDescription.GetContract(oContractType);

            if (desc != null)
            {
                // Get the Endpoint Element from App.config
                var oEpElem = GetEndpointElement(desc.Name);
                if (oEpElem != null && (oEpElem.Binding.Length > 0) && (oEpElem.BindingConfiguration.Length > 0))
                {
                    // Get the binding as specified in the endpoint
                    var oBinding = ResolveBinding(oEpElem.BindingConfiguration, oEpElem.Binding);
                    if (oBinding != null)
                    {
                        //Create the EndpointAddress
                        var endpointAddress = new EndpointAddress (oEpElem.Address);
                        _servername = oEpElem.Address.Host;
                        
                        // Create the Binding
                        oSEP = new ServiceEndpoint(desc, oBinding, endpointAddress);
                        
                        // Check if any Extensions are required for this endpoint
                        if (oEpElem.BehaviorConfiguration.Length > 0)
                        {
                            var endpointBehaviors = ResolveEndpointBehavior(oEpElem.BehaviorConfiguration);
                            foreach (var oBeh in endpointBehaviors)
                            {
                                if (oSEP.Behaviors.Contains(oBeh))
                                {
                                    oSEP.Behaviors.Remove(oBeh);
                                }
                                oSEP.Behaviors.Add(oBeh);
                            }
                        }
                    }
                }
            }
            return oSEP;
        }

        private static bool InitConfig(bool bReload = false)
        {
            if ( bReload || config == null )
            {
                UriBuilder uri = null;

                // Get the Config file from Environment
                string sConfigFile = "";
                try
                {
                    sConfigFile = Environment.GetEnvironmentVariable(sConfigFileEnv);
                    if ( sConfigFile.Length > 0 )
                        sConfigFile = Environment.ExpandEnvironmentVariables(sConfigFile);
                    if (sConfigFile.Length > 0)
                        uri = new UriBuilder(sConfigFile);
                }
                catch ( Exception ex )
                {
                    Console.Writeline("Error getting the Environment variable '{0}' ({1})", sConfigFileEnv, ex.Message);
                }

                if ( uri == null )
                {
                    // Get the D1l config
                    uri = new UriBuilder (Assembly.GetExecutingAssembly().CodeBase + ".config");
                }
                if (System.IO.File.Exists(uri.Path))
                {
                    _config  = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(new System.Configuration.ExeConfigurationFileMap() { ExeConfigFilename = uri.Path },
                                System.Configuration.ConfigurationUserLevel.None);
                }
                else
                {
                }
            }
            return (_config != null);
        }

        public static Configuration GetConfig()
        {
            InitConfig();
            return _config;
        }

        private static StandardBindingElement GetBindingElement(string bindingName, string sBindingType, out Type oBindingType)
        {
            oBindingType = null;
            StandardBindingElement bindingElement = null;
            
            var serviceModel = ServiceModelSectionGroup.GetSectionGroup(_config);

            foreach ( var bindingCollection in serviceModel.Bindings.BindingCollections )
            {
                if (bindingCollection.BindingName == sBindingType)
                {
                    foreach (var bindingElem in bindingCollection.ConfiguredBindings)
                    {
                        if (bindingElem.Name == bindingName)
                        {
                            bindingElement = (StandardBindingElement)bindingElem;
                            oBindingType = bindingCollection.BindingType;
                            break;
                        }
                    }
                }
                if (bindingElement !=  null)
                    break;
            }
            return bindingElement;
        }
        
        private static ChannelEndpointElement GetEndpointElement(string endpointContractName)
        {
            ChannelEndpointElement endpointElement = null;

            var serviceModel = ServiceModelSectionGroup.GetSectionGroup(_config);
            foreach(ChannelEndpointElement endpoint in servicemodelsamples.Client.Endpoints)
            {
                if (endpoint.Contract == endpointContractName)
                {
                    endpointElement == endpoint;
                    break;
                }
            }
            return endpointElement;
        }

        //EndpointIdentity endpointIdentity  = EndpointIdentity.CreateUpnIdentity(WindowsIdentity.GetCurrent().Name);
        //EndpointAddress endpointAddress = new EndpointAddress (new Uri("http://localhost:8003/servicemodelsamples/service/incode/identity")
        //,endpointIdentity, addressHeaders);
        //EndpointIdentity identity  = endpointAddress.Identity;
        
        private static Binding ResolveBinding(string sBindingName, string sBindingType)
        {
            Binding binding = null;
            
            Type oBindingType = null;
            var bindingElement = GetBindingElement(sBindingName, sBindingType, out oBindingType);
            
            if ( (bindingElement != null) && (OBindingType != null) )
            {
                binding  = (Binding)Activator.CreateInstance(oBindingType);
                binding.Name = bindingElement.Name;
                bindingElement.ApplyConfiguration(binding);
            }

            return binding;
        }

        private static List<IEndpointBehavior> ResolveEndpointBehavior(string name)
        {
            List<IEndpointBehavior> endpointBehaviors = new List<IEndpointBehavior>();
            
            var serviceModel = ServiceModelSectionGroup.GetSectionGroup(_config);
            var behaviorsSection = serviceModel.Behaviors;

            if (behaviorsSection.EndpointBehaviors.Count > 0 && behavíorsSection.EndpointBehaviors[0].Name == name )
            {
                var behaviorCollectionElement = behaviorsSection.EndpointBehaviors[0];
                
                foreach (BehaviorExtensionElement behaviorExtension in behaviorCollectionElement)
                {
                    object extension = behaviorExtension.GetType().InvokeMember ("CreateBehavior",
                    BindingFlags. InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, behaviorExtension, null);

                    endpointBehaviors.Add((IEndpointBehavior)extension);
                }
            }

            return endpointBehaviors;
        }
    }
}

