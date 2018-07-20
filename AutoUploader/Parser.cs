using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

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
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
            string input = File.ReadAllText(VarFileName);
            if(fileType == null)
            {
                return null;
            }
            switch (fileType)
            {
                case "json":
                    
                    var jsonValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(input);
                    foreach (var v in jsonValues)
                    {
                        KeyValuePair<string, string> value = new KeyValuePair<string, string>(v.Key, v.Value + "");
                        values.Add(value);
                    }
                    return values;
                    break;
                case "xml":
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(input);
                    var nodes = document.DocumentNode.SelectNodes("//text()[normalize-space()]");
                    foreach (var node in nodes)
                    {
                        KeyValuePair<string, string> value = new KeyValuePair<string, string>(node.ParentNode.Name, node.InnerHtml);
                        values.Add(value);
                    }
                    return values;
                    break;
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
                fileType = "xml";
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
    }
}
