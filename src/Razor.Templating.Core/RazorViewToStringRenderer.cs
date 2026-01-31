// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Templating.Core;

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

        // Fallback: try to resolve the view through compiled view descriptors (views compiled into referenced assemblies).
        var embeddedView = TryFindViewFromCompiledViewDescriptors(actionContext, viewName, isMainPage);
        if (embeddedView != null)
        {
            return embeddedView;
        }

        var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        var errorMessage = string.Join(
            Environment.NewLine,
            new[] {
                $"Unable to find view '{viewName}'. The following locations were searched:"
            }.Concat(searchedLocations)
            .Concat(new[]{
            "Hint:",
            "- Check whether you have added reference to the Razor Class Library that contains the view files.",
            "- Check whether the view file name is correct or exists at the given path.",
            "- Refer documentation or file issue here: https://github.com/soundaranbu/Razor.Templating.Core"}));

        throw new ViewNotFoundException(errorMessage);
    }

    /// <summary>
    /// Attempts to find a view that is compiled into assemblies (for example via Razor Class Libraries / precompiled views).
    /// </summary>
    private IView? TryFindViewFromCompiledViewDescriptors(ActionContext actionContext, string viewName, bool isMainPage)
    {
        var partManager = _serviceProvider.GetService<ApplicationPartManager>();
        if (partManager is null)
        {
            return null;
        }

        var normalizedViewName = NormalizeViewPath(viewName);

        var viewsFeature = new ViewsFeature();
        partManager.PopulateFeature(viewsFeature);

        foreach (var descriptor in viewsFeature.ViewDescriptors)
        {
            // RelativePath examples: "/Views/Home/Index.cshtml" , "/Views/_ViewStart.cshtml"
            var descriptorPath = NormalizeViewPath(descriptor.RelativePath);

            // Only match exact paths to avoid ambiguous matches (e.g., "Index" matching multiple views).
            if (!string.Equals(descriptorPath, normalizedViewName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Prefer FindView when the caller is using MVC-style names (e.g. "Index").
            var findResult = _viewEngine.FindView(actionContext, descriptor.RelativePath, isMainPage);
            if (findResult.Success)
            {
                return findResult.View;
            }

            var getResult = _viewEngine.GetView(executingFilePath: null, viewPath: descriptor.RelativePath, isMainPage);
            if (getResult.Success)
            {
                return getResult.View;
            }
        }

        return null;
    }

    /// <summary>
    /// Normalizes the view path for consistent comparison.
    /// </summary>
    private static string NormalizeViewPath(string viewPath)
    {
        if (string.IsNullOrEmpty(viewPath))
        {
            return viewPath;
        }

        var normalized = viewPath.TrimStart('~', '/');

        // Normalize path separators
        normalized = normalized.Replace('\\', '/');

        // Ensure .cshtml extension for comparisons when the caller passes bare names like "Index".
        if (!normalized.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
        {
            normalized += ".cshtml";
        }

        return normalized;
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
