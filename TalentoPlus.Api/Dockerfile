FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TalentoPlus.Api/TalentoPlus.Api.csproj", "TalentoPlus.Api/"]
COPY ["TalentoPlus.Web/TalentoPlus.Web.csproj", "TalentoPlus.Web/"]
COPY ["TalentoPlus.Application/TalentoPlus.Application.csproj", "TalentoPlus.Application/"]
COPY ["TalentoPlus.Infrastructure/TalentoPlus.Infrastructure.csproj", "TalentoPlus.Infrastructure/"]
COPY ["TalentoPlus.Domain/TalentoPlus.Domain.csproj", "TalentoPlus.Domain/"]
RUN dotnet restore "TalentoPlus.Api/TalentoPlus.Api.csproj"
COPY . .
WORKDIR "/src/TalentoPlus.Api"
RUN dotnet build "TalentoPlus.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TalentoPlus.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlus.Api.dll"]
