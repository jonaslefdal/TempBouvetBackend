#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5279
EXPOSE 5279

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . /src
RUN ls /src
WORKDIR "/src"
RUN ls "/src"
RUN dotnet restore 
RUN dotnet build -c Release  --no-restore

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish  --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=bouvetdb;Username=admin;Password=admin"

ENTRYPOINT ["dotnet", "BouvetBackend.dll"]