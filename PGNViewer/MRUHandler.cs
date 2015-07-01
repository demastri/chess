using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PGNViewer
{
    class MRUHandler
    {
        public List<string> MRUFiles;
        public MRUHandler()
        {
            MRUFiles = new List<string>();
        }
        public void InitList()
        {
            XmlDocument MRUDoc = new XmlDocument();
            MRUDoc.Load(System.IO.Directory.GetCurrentDirectory() + "/Resources/MRU.xml");
            foreach (XmlNode n in MRUDoc.SelectNodes("MRUList/File"))
            {
                MRUFiles.Add(n.Attributes["Path"].Value);
            }
        }
        public void UpdateList()
        {
            XmlTextWriter MRUDoc = new XmlTextWriter(System.IO.Directory.GetCurrentDirectory() + "/Resources/MRU.xml", Encoding.Default);

            MRUDoc.WriteStartElement("MRUList");
            for (int i = 0; i < MRUFiles.Count; i++)
            {
                MRUDoc.WriteStartElement("File");
                MRUDoc.WriteAttributeString("Index", (i + 1).ToString());
                MRUDoc.WriteAttributeString("Path", MRUFiles[i]);
                MRUDoc.WriteEndElement();
            }
            MRUDoc.WriteEndElement();
            MRUDoc.Close();

        }
        public void AddMRUFile(string s)
        {
            if (!MRUFiles.Contains(s))
            {
                MRUFiles.Add(s);
                UpdateList();
            }
        }
    }
}
