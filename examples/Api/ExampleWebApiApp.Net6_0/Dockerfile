#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["examples/Api/ExampleWebApiApp.Net6_0/ExampleWebApiApp.Net6_0.csproj", "examples/Api/ExampleWebApiApp.Net6_0/"]
RUN dotnet restore "examples/Api/ExampleWebApiApp.Net6_0/ExampleWebApiApp.Net6_0.csproj"
COPY . .
WORKDIR "/src/examples/Api/ExampleWebApiApp.Net6_0"
RUN dotnet build "ExampleWebApiApp.Net6_0.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExampleWebApiApp.Net6_0.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExampleWebApiApp.Net6_0.dll"]