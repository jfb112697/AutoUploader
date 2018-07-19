using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoUploader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, int> _variableIndex = new Dictionary<string, int>();

        public Parser parser = new Parser();

        public Dictionary<string, int> VariableIndex { get { return _variableIndex; }
            set
            {
                _variableIndex = value;
                dataGrid.ItemsSource = value;
            }
        }

        private string _recordingDirectory = "";
        private string RecordingDirectory { get { return _recordingDirectory; }
            set
            {
                _recordingDirectory = value;
                directoryBlock.Text = value;
            }
        }

        private string _varFilePath = "";
        public string VarFilePath
        {
            get { return _varFilePath; }
            set
            {
                streamControlBlock.Text = value;
                _varFilePath = value;
                this.parser.VarFileName = value;
            }
        }

        public string TitleTemplate { get; set; }

        public string DescriptionTemplate { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = false;
            dialog.FileName = "Folder Selection";
            dialog.ShowDialog();
            string[] dirElem = dialog.FileName.Split('\\');
            string recordingDir = string.Join("\\", dirElem.Take(dirElem.Count() - 1));
            RecordingDirectory = recordingDir;
        }

        private void streamContrlButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.ShowDialog();
            dialog.CheckFileExists = true;
            if(dialog.FileName != null)
            {
                VarFilePath = dialog.FileName;
            }
            VariableWindow window = new VariableWindow(this, VarFilePath);
            window.Show();
        }

        private void titleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TitleTemplate = titleBox.Text;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            dynamic draftObject = new ExpandoObject();
            draftObject.RecordingDirectory = this.RecordingDirectory;
            draftObject.StreamControlPath = this.VarFilePath;
            draftObject.TitleTemplate = this.TitleTemplate;
            draftObject.DescriptionTemplate = this.DescriptionTemplate;

            File.WriteAllText("settings.json", Newtonsoft.Json.JsonConvert.SerializeObject(draftObject));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("settings.json"))
            {
                dynamic draftObject = JObject.Parse(File.ReadAllText("settings.json"));
                if (draftObject != null)
                {
                    this.RecordingDirectory = draftObject.RecordingDirectory;
                    this.VarFilePath = draftObject.StreamControlPath;
                    titleBox.Text = draftObject.TitleTemplate;
                    descriptionBox.Text = draftObject.DescriptionTemplate;
                }
            }
        }

        private void descriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DescriptionTemplate = descriptionBox.Text; //TODO: Change this to a data binding
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = RecordingDirectory;
            watcher.Created += Watcher_Created;
        }

        StringBuilder builder = new StringBuilder() { };
        

        private  void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            long oldSize = 0;
            statusBlock.Text = "Watching " + e.FullPath;
            while (true)
            {
                FileInfo info = new FileInfo(e.FullPath);
                if(info.Length == oldSize)
                {
                    break;
                }
                oldSize = info.Length;
            }
            Regex placeholder = new Regex("{{ .* }}");
            string title = TitleTemplate + "";
            var matches = placeholder.Matches(title);
            List<KeyValuePair<string, string>> parsed = new List<KeyValuePair<string, string>>();
            foreach(Match match in matches)
            {
                string vName = match.Groups[1].Value;
                int index = VariableIndex[vName];
                title = title.Replace(match.Value, parsed[index].Value);
            }
            File.Move(e.FullPath, RecordingDirectory + "\\" + title + e.FullPath.Split('.')[1]); //TODO: Probably shell this out & build title string better
        }
    }
}
