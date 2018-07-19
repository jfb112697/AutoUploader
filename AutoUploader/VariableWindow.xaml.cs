using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoUploader
{
    /// <summary>
    /// Interaction logic for VariableWindow.xaml
    /// </summary>
    public partial class VariableWindow : Window
    {
        private MainWindow main;
        List<string> variables = new List<string>() { "Player 1 Name", "Player 2 Name", "Player 1 Character", "Player 2 Character" };
        List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
        Dictionary<string, int> varIndex = new Dictionary<string, int>();
        private string varFilePath;
        public VariableWindow(MainWindow main, string varFilePath)
        {
            this.varFilePath = varFilePath;
            this.main = main;
            values = main.parser.parse();
            InitializeComponent();
            foreach(var v in variables)
            {
                wrapPanel.Children.Add(new GroupBox() { MinWidth = 150, MaxWidth = 250, Content = new ComboBox() { ItemsSource = values, DisplayMemberPath = "Value"}, Header = v });
            }
        }

        private void variableButton_Click(object sender, RoutedEventArgs e)
        {
            wrapPanel.Children.Add(new GroupBox() { MinWidth = 150, MaxWidth = 250, Content = new ComboBox() { ItemsSource = values }, Header = variableBox.Text });
            variableBox.Text = "";
        }

      

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(Control c in wrapPanel.Children)
            {
                GroupBox g = c as GroupBox;
                string varName = g.Header.ToString();
                ComboBox combo = g.Content as ComboBox;
                string value = combo.Text;
                if (value != "")
                {
                    int? index = values.IndexOf(values.First(k => k.Value == value));
                    if (index != null)
                    {
                        varIndex.Add(varName, (int)index);
                    }
                }
            }
            main.VariableIndex = varIndex;
            this.Close();
        }
    }
}
