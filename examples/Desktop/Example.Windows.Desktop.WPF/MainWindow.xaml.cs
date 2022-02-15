using ExampleRazorTemplatesLibrary.Models;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Example.Windows.Desktop.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            renderButton.Click += renderButton_Click;
        }

        private async void renderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = new ExampleModel()
                {
                    PlainText = "This text is rendered from Razor View using Razor.Templating.Core Library",
                    HtmlContent = "<em>You can do awesome stuff like Reporting, Invoicing, etc,. Try it today</em>"
                };

                var viewData = new Dictionary<string, object>();
                viewData["Value1"] = "1";
                viewData["Value2"] = "2";

                var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);
                textBlock.Text = html;

                // after a template has been rendered, XAML view can no longer be resolved
                var view = new MainWindow();
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }
    }
}
