using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUploader
{
    public class Parser
    {
        private string _varFileName = "";
        public string VarFileName
        {
            get { return _varFileName; }
            set
            {
                _varFileName = value;
                setFileType(value);
            }
        }

        string fileType = "";
        public Parser() { }
        public List<KeyValuePair<string, string>> parse()
        {
            string input = File.ReadAllText(VarFileName);
            if(fileType == null)
            {
                return null;
            }
            switch (fileType)
            {
                case "json":
                    List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                    var jsonValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(input);
                    foreach (var v in jsonValues)
                    {
                        KeyValuePair<string, string> value = new KeyValuePair<string, string>(v.Key, v.Value + "");
                        values.Add(value);
                    }
                    return values;
                    break;
                    //TODO: Parse X/HT ML
            }
            return null;

        }

        private void setFileType(string value)
        {
            string fileName = value.ToLower();
            if (fileName.Contains("json"))
            {
                fileType = "json";
            }
            else if (fileName.Contains("html"))
            {
                fileType = "html";
            }
            else if (fileName.Contains("xml"))
            {
                fileType = "xml";
            }
            else
            {
                System.Windows.MessageBox.Show("Could not find supported file type, please navigate to a JSON, HTML or XML file.");
                fileType = null;
            }
            
        }

        /*public static List<KeyValuePair<string, string>> parseXml(string input)
        {

        }*/
    }
}
