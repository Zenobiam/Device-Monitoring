using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Linq;

namespace MonitoringDevices
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _previousXml;

        public MainWindow()
        {
            InitializeComponent();

            Task.Factory.StartNew(() => Start());
        }

        private void Start()
        {
            while (true)
            {
                try
                {
                    XDocument xml = GetXml();

                    if (xml == null)
                        continue;

                    string newStringXml = xml.ToString();

                    if (_previousXml == newStringXml)
                        continue;

                    _previousXml = newStringXml;

                    Dispatcher.Invoke(new Action(() => 
                    {
                        nodeTreeView.Items.Clear();
                        nodeTreeView.Items.Add(SetTreeView(xml.Root, new TreeViewItem()));
                    }));
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        //Функция получает директорию и берет в ней первый файл который ожидается как (.xml) и возвращает его содержимое  
        private static XDocument GetXml()
        {
            try
            {
                var di = new DirectoryInfo("xml");

                if (!di.Exists)
                    di.Create();

                var files = di.GetFiles();

                FileInfo file = null;

                if (files != null && files.Any())
                {
                    file = files.FirstOrDefault();
                }

                if (file == null)
                    return null;

                string text = "";

                using (var sr = new StreamReader(file.FullName))
                {
                    text = sr.ReadToEnd();
                }

                return XDocument.Parse(text);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //Рекурсивная функция проходит по всем элементам в ветвях и выстраивает дерево
        private TreeViewItem SetTreeView(XElement element, TreeViewItem item)
        {
            item = new TreeViewItem
            {
                Header = new TextBlock
                {
                    Text = element.Name.LocalName,
                    Style = (Style)FindResource("headerStyle")
                },
                IsExpanded = true
            };

            var elements = element.Elements();
            if (elements != null && elements.Any())
            {
                foreach (var el in elements)
                {
                    TreeViewItem child = null;
                    item.Items.Add(SetTreeView(el, child));
                }
            }
            else if (!string.IsNullOrWhiteSpace(element.Value))
            {
                item.Items.Add(new TextBlock
                {
                    Text = element.Value,
                    Style = (Style)FindResource("valueStyle")
                });
            }
            else
            {
                item.Items.Add(new TextBlock
                {
                    Text = element.Value,
                    Style = (Style)FindResource("notValueStyle")
                });
                item.IsExpanded = false;
            }

            var attr = element.Attributes();
            if (attr != null && attr.Any())
            {
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal, Style = (Style)FindResource("headerWithAttrStackPanel") };

                panel.Children.Add(new TextBlock { Text = element.Name.LocalName, Style = (Style)FindResource("headerWithArrtTextStyle") });

                foreach (var a in attr)
                {
                    panel.Children.Add(new TextBlock { Text = a.Name.LocalName, Style = (Style)FindResource("attrNameStyle") });
                    panel.Children.Add(new TextBlock { Text = $"=\"{ a.Value}\"", Style = (Style)FindResource("attrValStyle") });
                }

                item.Header = panel;

                //item.Header = new TextBlock { Text = $"{element.Name.LocalName} {string.Join(" ", attr.Select(x => $"{x.Name} = {x.Value}"))}" };
            }


            return item;
        }

        //private static Node RecFoo(XElement element, Node node)
        //{
        //    node = new Node { Name = element.Name.LocalName, Value = element.Value };
        //    var elements = element.Elements();
        //    if (elements != null && elements.Any())
        //    {
        //        foreach (var item in elements)
        //        {
        //            Node child = null;
        //            node.Children.Add(RecFoo(item, child));
        //        }
        //    }
        //    return node;
        //}
    }
}
