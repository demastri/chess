using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JPD.Utilities
{
    // generally, settings are simply key/value pairs, where for single elements they are simple nodes of type <Setting name='' value='' />
    // for multivalued/array elements, its of the type <Setting name='' ><settingValue id=1 value='' /><settingValue id=2 value='' /></setting>
    // will surface in a resources/settings.xml file
    // writes update the file appropriately

    // from an access perspective, crud ops are all we need:
    // someval = Settings[key] or Settings[key][index];
    // Settings[key] = "x"; // includes both add and update for single val
    // Settings[key].Add( value )   // explicit add for multival
    // Settings[key] = null or Settings.Remove(key, value) or Settings.Remove(key, index)

    public class Settings
    {
        public static Settings AppSettings = new Settings();

        Dictionary<string, string> singleValueSettings;
        Dictionary<string, List<string>> multiValueSettings;

        bool loading;

        private Settings()
        {
            loading = true;
            Load();
            loading = false;
        }

        public string this[string k]
        {
            get
            {
                if (singleValueSettings.ContainsKey(k))
                    return singleValueSettings[k];
                return null;
            }
            set
            {
                if (singleValueSettings.ContainsKey(k))
                    singleValueSettings.Remove(k);
                if (value != null) // it's not a clear
                    singleValueSettings.Add(k, value);
                Save();
            }
        }
        public string this[string k, int i]
        {
            get
            {
                if (multiValueSettings.ContainsKey(k) && multiValueSettings[k].Count > i)
                    return multiValueSettings[k][i];
                return null;
            }
            set
            {
                if (multiValueSettings.ContainsKey(k) && i != -1 && multiValueSettings[k].Count > i)
                {
                    // k and i are "valid" - either clear or update it...
                    if (value == null) // it's a clear
                        multiValueSettings[k].RemoveAt(i);
                    else 
                        multiValueSettings[k][i] = value;
                    Save();
                }
                else // it's a new key or index...
                    Add( k, value );
            }
        }
        public void Add(string k, string v)
        {
            if( !multiValueSettings.ContainsKey(k) )
                multiValueSettings[k] = new List<string>();
            if( v != null )
                multiValueSettings[k].Add(v);
            Save();
        }
        public void Remove(string k, string v)
        {
            if (multiValueSettings.ContainsKey(k) && multiValueSettings[k].Contains(v))
                Remove(k, multiValueSettings[k].IndexOf(v));    
            // goes through Remove(s,i) which goes through indexer - no Save needed
        }
        public void Remove(string k, int i)
        {
            multiValueSettings[k][i] = null;
        }
        public void Clear(string k)
        {
            if (multiValueSettings.ContainsKey(k))
                multiValueSettings[k].Clear();
            if (singleValueSettings.ContainsKey(k))
                singleValueSettings.Remove(k);
        }
        public int Count(string k)
        {
            if (multiValueSettings.ContainsKey(k))
                return multiValueSettings[k].Count();
            if (singleValueSettings.ContainsKey(k))
                return 1;
            return 0;
        }
        public bool Contains(string k, string v)
        {
            if (multiValueSettings.ContainsKey(k))
                return multiValueSettings[k].Contains(v);
            if (singleValueSettings.ContainsKey(k))
                return singleValueSettings[k] == v;
            return false;
        }
        List<string> SingleValueKeys { get { return singleValueSettings.Keys.ToList(); } }
        List<string> MultiValueKeys { get { return multiValueSettings.Keys.ToList(); } }

        // file management

        private void Load()
        {
            singleValueSettings = new Dictionary<string,string>();
            multiValueSettings = new Dictionary<string, List<string>>();

            XmlDocument SettingsDoc = new XmlDocument();
            SettingsDoc.Load(System.IO.Directory.GetCurrentDirectory() + "/Resources/Settings.xml");
            foreach (XmlNode n in SettingsDoc.SelectNodes("Settings/Setting"))
            {
                if (n.HasChildNodes || n.Attributes["value"] == null)
                {
                    string key = n.Attributes["name"].Value;
                    Add(key, null);
                    foreach (XmlNode kid in n.ChildNodes)
                    {
                        Add(key, kid.Attributes["value"].Value);
                    }
                }
                else
                {
                    this[n.Attributes["name"].Value] = n.Attributes["value"].Value;
                }
            }
        }

        private void Save()
        {
            if (loading)
                return;
            XmlTextWriter SettingsDoc = new XmlTextWriter(System.IO.Directory.GetCurrentDirectory() + "/Resources/Settings.xml", Encoding.Default);
            SettingsDoc.WriteStartElement("Settings");

            foreach (string k in SingleValueKeys)
            {
                SettingsDoc.WriteStartElement("Setting");
                SettingsDoc.WriteAttributeString("name", k);
                SettingsDoc.WriteAttributeString("value", this[k]);
                SettingsDoc.WriteEndElement();
            }

            foreach (string k in MultiValueKeys)
            {
                SettingsDoc.WriteStartElement("Setting");
                SettingsDoc.WriteAttributeString("name", k);

                for( int i=0; i<this.Count(k); i++ )
                {
                    SettingsDoc.WriteStartElement("SettingValue");
                    SettingsDoc.WriteAttributeString("id", (i+1).ToString());
                    SettingsDoc.WriteAttributeString("value", this[k, i]);
                    SettingsDoc.WriteEndElement();  // setting (multi element)
                }
                
                SettingsDoc.WriteEndElement();  // setting (multi)
            }

            SettingsDoc.WriteEndElement();  // settings
            SettingsDoc.Close();

        }
    }
}
