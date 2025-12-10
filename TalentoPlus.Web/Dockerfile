FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TalentoPlus.Web/TalentoPlus.Web.csproj", "TalentoPlus.Web/"]
COPY ["TalentoPlus.Application/TalentoPlus.Application.csproj", "TalentoPlus.Application/"]
COPY ["TalentoPlus.Infrastructure/TalentoPlus.Infrastructure.csproj", "TalentoPlus.Infrastructure/"]
COPY ["TalentoPlus.Domain/TalentoPlus.Domain.csproj", "TalentoPlus.Domain/"]
RUN dotnet restore "TalentoPlus.Web/TalentoPlus.Web.csproj"
COPY . .
WORKDIR "/src/TalentoPlus.Web"
RUN dotnet build "TalentoPlus.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TalentoPlus.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlus.Web.dll"]
