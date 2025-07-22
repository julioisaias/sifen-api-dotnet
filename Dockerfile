FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/SifenApi.WebApi/SifenApi.WebApi.csproj", "src/SifenApi.WebApi/"]
COPY ["src/SifenApi.Application/SifenApi.Application.csproj", "src/SifenApi.Application/"]
COPY ["src/SifenApi.Domain/SifenApi.Domain.csproj", "src/SifenApi.Domain/"]
COPY ["src/SifenApi.Infrastructure/SifenApi.Infrastructure.csproj", "src/SifenApi.Infrastructure/"]
RUN dotnet restore "src/SifenApi.WebApi/SifenApi.WebApi.csproj"
COPY . .
WORKDIR "/src/src/SifenApi.WebApi"
RUN dotnet build "SifenApi.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SifenApi.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SifenApi.WebApi.dll"]