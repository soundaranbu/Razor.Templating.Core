using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    public interface IRazorTemplateEngine
    {
        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Optional model data</param>
        /// <param name="viewData">Optional view data</param>
        /// <returns></returns>
        Task<string> RenderAsync([DisallowNull] string viewName, object? model = null, Dictionary<string, object>? viewData = null);
    }
}
