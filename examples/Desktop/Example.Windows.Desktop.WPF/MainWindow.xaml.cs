using ExampleRazorTemplatesLibrary.Models;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Windows;

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

            renderButton.Click += RenderButton_Click;
        }

        private async void RenderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = new ExampleModel()
                {
                    PlainText = "This text is rendered from Razor View using Razor.Templating.Core Library",
                    HtmlContent = "<em>You can do awesome stuff like Reporting, Invoicing, etc,. Try it today</em>"
                };

                var viewData = new Dictionary<string, object>
                {
                    ["Value1"] = "1",
                    ["Value2"] = "2"
                };

                var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);
                textBlock.Text = html;

                browser.NavigateToString(html);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
