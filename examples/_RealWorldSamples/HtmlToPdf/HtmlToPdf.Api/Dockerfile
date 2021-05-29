#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

RUN apt-get update

# for jsreport
# install chrome with deps, see https://github.com/jsreport/jsreport/blob/master/docker/full/Dockerfile
RUN apt-get install -y --no-install-recommends libgconf-2-4 gnupg git curl wget ca-certificates libgconf-2-4 && \
    wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - && \
    sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' && \
    apt-get update && \
    apt-get install -y lsb-release google-chrome-stable fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst libxtst6 libxss1 --no-install-recommends
RUN apt-get install -y gconf-service libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libcairo2 libcups2 libdbus-1-3 libexpat1 libfontconfig1 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libglib2.0-0 libgtk-3-0 libnspr4 libpango-1.0-0 libpangocairo-1.0-0 libstdc++6 libx11-6 libx11-xcb1 libxcb1 libxcomposite1 libxcursor1 libxdamage1 libxss1 libxtst6 libappindicator1 libnss3 libasound2 libatk1.0-0 libc6 ca-certificates fonts-liberation lsb-release xdg-utils wget
ENV chrome_launchOptions_executablePath google-chrome-stable
ENV chrome_launchOptions_args --no-sandbox,--disable-dev-shm-usage,--single-process,--no-zygote

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["HtmlToPdf.Api/HtmlToPdf.Api.csproj", "HtmlToPdf.Api/"]
RUN dotnet restore "HtmlToPdf.Api/HtmlToPdf.Api.csproj"
COPY . .
WORKDIR "/src/HtmlToPdf.Api"
RUN dotnet build "HtmlToPdf.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HtmlToPdf.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HtmlToPdf.Api.dll"]