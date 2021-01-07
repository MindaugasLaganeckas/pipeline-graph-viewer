FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY . .
# installs NodeJS and NPM
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && apt-get install -yq nodejs build-essential
# install node
RUN npm install -g npm
# Opt out of .NET Core's telemetry collection
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
# set node to production
ENV NODE_ENV production
# https://github.com/aspnet/JavaScriptServices/issues/1514
RUN dotnet publish PipelineGraph/PipelineGraph.csproj -c Debug -o /app/artifacts/PipelineGraph
HEALTHCHECK --retries=5 --interval=10s --timeout=1s CMD curl --fail http://localhost:8080/weatherforecast || exit 1

WORKDIR /app/artifacts/PipelineGraph
ENTRYPOINT ["dotnet", "PipelineGraph.dll", "--urls", "http://*:8080"]