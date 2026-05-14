FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY global.json ./
COPY DentalSaaS.sln ./
COPY DentalSaaS.Application/*.csproj DentalSaaS.Application/
COPY DentalSaaS.Application.Tests/*.csproj DentalSaaS.Application.Tests/
COPY DentalSaaS.Domain/*.csproj DentalSaaS.Domain/
COPY DentalSaaS.Infrastructure/*.csproj DentalSaaS.Infrastructure/
COPY DentalSaaS.Shared/*.csproj DentalSaaS.Shared/
COPY DentalSaaS.Web/*.csproj DentalSaaS.Web/

RUN dotnet restore DentalSaaS.sln

COPY . .
RUN dotnet publish DentalSaaS.Web/DentalSaaS.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DentalSaaS.Web.dll"]
