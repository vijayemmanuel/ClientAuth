using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WebClientUtils
{
    public class WebClientSettingsConfigSection : ConfigurationSection
    {
        // Declare a collection element represented
        // in the configuration file by the sub-section
        // <Endpoint> <add.../> </Endpoint>
        // Note: the "IsDefaultCollection false"
        // instructs the .NET Framework to build a nested
        // section like <EndpointPropertiesCollection> ...</EndpointPropertiesCollection>.
        [ConfigurationProperty("EndpointPropertiesCollection", IsDefaultCollection = false)]
        public AINEndpointPropertiesCollectionElement EndpointPropertiesCollection
        {
            get
            {
                return base["AINEndpointPropertiesCollection"] as AINEndpointPropertiesCollectionElement;
            }
        }
    }
    
    [ConfigurationCollection(typeof(EndpointPropertiesElement), AddItemName = "EndpointProperties", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class AINEndpointPropertiesCollectionElement: ConfigurationElementCollection
    {   
        protected override ConfigurationElement CreateNewElement () 
        {   
            return new EndpointPropertiesElement();
        }
    }

    protected override Object GetElementKey(ConfigurationElement element)
    {
        return ((EndpointPropertiesElement)element).Name;
    }
    new public EndpointPropertiesElement this[string Name]
    {
        get
        {
            return BaseGet(Name) as EndpointPropertiesElement;
        }
    }

    public int IndexOf (EndpointPropertiesElement element)
    {
        return BaseIndexOf (element);
    }
    public void Add(AINEndpointPropertiesElement element)
    {
        BaseAdd(element);
    }
    protected override void BaseAdd(ConfigurationElement element)
    {
        BaseAdd(element, false);        
    }
    public void Remove(AINEndpointPropertiasElement element)
    {
        if (BaseIndeof(element) >= 0)
            BaseResove(element,Name);
    }
    public void RemoveAt (int index)
    {
        BaseRemoveAt(index);
    }
    public void Remove(string name)
    {
        BaseRemove(name);
    }
    public void Clear()
    {
        BaseClear();
    }
}

