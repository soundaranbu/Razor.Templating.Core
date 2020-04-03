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

        public static RazorViewToStringRenderer GetRenderer()
        {

            if (_razorViewToStringRenderer is null)
            {
                _razorViewToStringRenderer = RazorViewToStringRendererFactory.CreateRenderer();
            }
            return _razorViewToStringRenderer;
        }

        public static async Task<string> RenderAsync<TModel>(string viewName, TModel model)
        {
            return await GetRenderer().RenderViewToStringAsync(viewName, model).ConfigureAwait(false);
        }

        public static async Task<string> RenderAsync<TModel>(string viewName, TModel model, Dictionary<string, object> viewData)
        {
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            if(!(viewData is null))
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
