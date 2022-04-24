using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    public interface IRazorTemplateEngine
    {
        Task<string> RenderAsync([DisallowNull] string viewName);
        Task<string> RenderAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model);
        Task<string> RenderAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model, [DisallowNull] Dictionary<string, object> viewData);
    }
}
