FROM node:20 AS ui
WORKDIR /ui
COPY taskboard-ui/package*.json ./
RUN npm install
COPY taskboard-ui .
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base
WORKDIR /src
COPY ./TaskBoard.Api ./TaskBoard.Api
RUN dotnet publish TaskBoard.Api -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=base /app .
COPY --from=ui /ui/dist ./dist
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskBoard.Api.dll"]