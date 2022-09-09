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
        private readonly RazorTemplateEngine _razorTemplateEngine;

        public MainWindow(RazorTemplateEngine razorTemplateEngine)
        {
            _razorTemplateEngine = razorTemplateEngine ?? throw new ArgumentNullException(nameof(razorTemplateEngine));

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

                var html = await _razorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);
                textBlock.Text = html;

                browser.NavigateToString(html);
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }
    }
}
