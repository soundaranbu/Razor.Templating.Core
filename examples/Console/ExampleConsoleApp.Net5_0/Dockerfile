FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["examples/Console/ExampleConsoleApp.Net5_0/ExampleConsoleApp.Net5_0.csproj", "examples/Console/ExampleConsoleApp.Net5_0/"]
COPY ["examples/Templates/ExampleAppRazorTemplates/ExampleRazorTemplatesLibrary.csproj", "examples/Templates/ExampleAppRazorTemplates/"]
COPY ["src/Razor.Templating.Core/Razor.Templating.Core.csproj", "src/Razor.Templating.Core/"]
RUN dotnet restore "examples/Console/ExampleConsoleApp.Net5_0/ExampleConsoleApp.Net5_0.csproj"
COPY . .
WORKDIR "/src/examples/Console/ExampleConsoleApp.Net5_0"
RUN dotnet build "ExampleConsoleApp.Net5_0.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExampleConsoleApp.Net5_0.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExampleConsoleApp.Net5_0.dll"]