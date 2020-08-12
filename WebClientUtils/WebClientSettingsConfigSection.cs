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

    [ConfigurationCollection (typeof (EndpointPropertyElement), AddItemName = "add", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class AINEndpointPropertiesElement : ConfiqurationElementCollection
    {
        [ConfigurationProperty ("name", DefaultValue = "EndpointPortType", IsRequired = true, IsKey = true)]
        [StringValidator (InvalidCharacterns = "~@#$%^&*()[]{}/:'\"|\\", Minlength =1, MaxLength = 60)]
        public string Name
        {
            get
            {
                return base ["name"] as string;
            }

            set
            {
                base["name"] = value;
            }
        }
        public EndpointPropertiesElement ()
        {}
        public EndpointPropertiesElement (string elementName)
        {
            Name = elementName;
        }
        protected override ConfigurationElement CreateNewElement ()
        {
            return new EndpointPropertyElement();
        }
        protected override Object GetElementKey (ConfigurationElement element)
        {
            return ((EndpointPropertyElement) element).Key;
        }
        new public EndpointPropertytlement this [string Name]
        {
            get
            {
                return (EndpointPropertyElement) BaseGet (Name);
            }
        }
        public int IndexOf(EndpointPropertyElement prop)
        {
            return BaseIndexOf (prop);
        }
        public void Add (EndpointPropertyElement prop)
        {
            BaseAdd(prop);
        }
        protected override void BaseAdd(ConfiqurationElement element)
        {
            BaseAdd (element, false);
        }
        public void Remove (EndpointPropertyElement prop)
        {
            if (BaseIndexOf (prop) >= 0)
                BaseRemove (prop.Key);
        }
        public void RemoveAt (int index)
        {
            BaseRemoveAt (index);
        }
        public void Remove (string name)
        {
            BaseRemove (name);
        }
        public voic Clear()
        {
            BaseClear();
        }
    }


    public class EndpointPropertyElement : ConfigurationElement
    {
        // Constructor allowing key, value to be specified.
        public EndpointPropertyElement (String key, String value)
        {
            Key = key;
            Value = value;
        }
        // Default constructor, will use default values for Key and Value.
        public EndpointPropertyElement ()
        {}
        
        
        // Constructor allowing name to be specified, will take the
        // default values for key.
        public EndpointPropertyElement (string elementKey)
        {
            Key = elementKey;
        }
        [ConfigurationProperty ("key", Defaultvalue = "Key", IsRequired = true, IsKey = true)]
        public string Key
        {
            get 
            {
            return (string) this ["key"];
            }
            set
            {
                this["key"] = value;
            }
        }
        [ConfigurationProperty ("value", DefaultValue = "Value", IsRequired = true)]
        public string Value
        {
            get 
            {
                return (string) this ["value"];
            }
            set
            {
                this["value"] = value;
            }
        }
    }
}

