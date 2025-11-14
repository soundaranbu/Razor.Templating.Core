// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Razor.Templating.Core.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    internal sealed class RazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RazorViewToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> RenderViewToStringAsync(string viewName, object? model, ViewDataDictionary viewDataDictionary, bool isMainPage = true)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewName, isMainPage);

            await using var output = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<object>(viewDataDictionary, model),
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _tempDataProvider),
                output,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext);

            return output.ToString();
        }

        private IView FindView(ActionContext actionContext, string viewName, bool isMainPage)
        {
            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage);
            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage);
            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new string[] {
                    $"Unable to find view '{viewName}'. The following locations were searched:"
                }.Concat(searchedLocations)
                .Concat(new string[]{
                "Hint:",
                "- Check whether you have added reference to the Razor Class Library that contains the view files.",
                "- Check whether the view file name is correct or exists at the given path.",
                "- Refer documentation or file issue here: https://github.com/soundaranbu/Razor.Templating.Core"}));

            throw new ViewNotFoundException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var endpoint = httpContext?.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata.GetMetadata<ActionDescriptor>();

            ActionContext? actionContext;

            if (httpContext is null)
            {
                // Non HTTP request scenarios like console, worker services
                actionContext = GetDefaultActionContext();
            }
            else
            {
                actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), actionDescriptor ?? new ActionDescriptor());
            }

            return actionContext;
        }

        private ActionContext GetDefaultActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            var app = new ApplicationBuilder(_serviceProvider);
            var routeBuilder = new RouteBuilder(app)
            {
                DefaultHandler = new CustomRouter()
            };

            routeBuilder.MapRoute(
                string.Empty,
                "{controller}/{action}/{id}",
                new RouteValueDictionary(new { id = "defaultid" }));

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            actionContext.RouteData.Routers.Add(routeBuilder.Build());
            return actionContext;
        }
    }

    internal class CustomRouter : IRouter
    {
        public VirtualPathData? GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            return Task.CompletedTask;
        }
    }
}
