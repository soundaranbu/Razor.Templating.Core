using System.Collections.Generic;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    public interface IRazorTemplateEngine
    {
        /// <summary>
        /// Renders the Razor View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view bag or view data</param>
        /// <returns>Rendered HTML string of the view</returns>
        /// <exception cref="Exceptions.ViewNotFoundException">Invalid View</exception>
        Task<string> RenderAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null);

        /// <summary>
        /// Renders the Razor View(.cshtml) To String. It does not throw exception when View is not found
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewBagOrViewData"></param>
        /// <returns></returns>
        Task<(bool ViewExists, string? RenderedView)> TryRenderAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null);

        /// <summary>
        /// Renders the Razor View(.cshtml) Without Layout to String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view bag or view data</param>
        /// <returns>Rendered HTML string of the view</returns>
        /// <exception cref="Exceptions.ViewNotFoundException">Invalid View</exception>
        Task<string> RenderPartialAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null);

        /// <summary>
        /// Renders the Razor View(.cshtml) Without Layout to String. This method does not throw exception when View is not found.
        /// If the ViewExists return false, please check the following
        /// Check whether you have added reference to the Razor Class Library that contains the view files or
        /// Check whether the view file name is correct or exists at the given path or
        /// Refer documentation or file issue here: https://github.com/soundaranbu/Razor.Templating.Core"}
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Optional model data</param>
        /// <param name="viewBagOrViewData">Optional view bag or view data</param>
        /// <returns></returns>
        Task<(bool ViewExists, string? RenderedView)> TryRenderPartialAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null);
    }
}
