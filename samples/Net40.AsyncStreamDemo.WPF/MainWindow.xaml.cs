using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Net40.AsyncStreamDemo.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool _isWork;
        public MainWindow()
        {
            InitializeComponent();
            _isWork = false;
        }

        private async void ReadFile_Click(object sender, RoutedEventArgs e)
        {
            if (_isWork)
            {
                return;
            }
            _isWork = true;
            _button.IsEnabled = false;
            _listBox.Items.Clear();
            string path = Path.Combine(Environment.CurrentDirectory, "file.txt");
            var lines = GetLines(path);
            await foreach (var line in lines)
            {
                _listBox.Items.Add(line);
            }
            _isWork = false;
            _button.IsEnabled = true;
        }

        async IAsyncEnumerable<string> GetLines(string filePath)
        {
            string line;
            StreamReader file = new StreamReader(filePath);
            while ((line = await file.ReadLineAsync()) != null)
            {
                await TaskEx.Delay(300);
                yield return line;
            }
        }
    }
}