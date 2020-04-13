using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{ 
    public class RazorTemplateEngine
    {
        private static RazorViewToStringRenderer _razorViewToStringRenderer;

        /// <summary>
        /// Creates the cache of RazorViewToStringRenderer
        /// </summary>
        public static void Initialize()
        {
            GetRenderer();
        }

        /// <summary>
        /// Get the RazorViewToStringRenderer object from cache if already exists else creates a new object.
        /// </summary>
        /// <returns></returns>
        public static RazorViewToStringRenderer GetRenderer()
        {

            if (_razorViewToStringRenderer is null)
            {
                _razorViewToStringRenderer = RazorViewToStringRendererFactory.CreateRenderer();
            }
            return _razorViewToStringRenderer;
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <returns>Rendered string from the view</returns>
        public static async Task<string> RenderAsync(string viewName)
        {
            return await GetRenderer().RenderViewToStringAsync<object>(viewName, default).ConfigureAwait(false);
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Strongly typed object </param>
        /// <returns></returns>
        public static async Task<string> RenderAsync<TModel>(string viewName, TModel model)
        {
            return await GetRenderer().RenderViewToStringAsync(viewName, model).ConfigureAwait(false);
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Strongly typed object</param>
        /// <param name="viewData">ViewData</param>
        /// <returns></returns>
        public static async Task<string> RenderAsync<TModel>(string viewName, TModel model, Dictionary<string, object> viewData)
        {
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            if (!(viewData is null))
            {
                foreach (var keyValuePair in viewData.ToList())
                {
                    viewDataDictionary.Add(keyValuePair);
                }
            }

            return await GetRenderer().RenderViewToStringAsync(viewName, model, viewDataDictionary).ConfigureAwait(false);
        }
    }
}
