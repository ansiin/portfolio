FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HWDemo.sln", "./"]
COPY ["Directory.Build.Props", "./"]
COPY ["WebApp/WebApp.csproj", "WebApp/"]
COPY ["App.BLL/App.BLL.csproj", "App.BLL/"]
COPY ["App.DAL.EF/App.DAL.EF.csproj", "App.DAL.EF/"]
COPY ["App.Domain/App.Domain.csproj", "App.Domain/"]
COPY ["App.DTO/App.DTO.csproj", "App.DTO/"]
COPY ["App.Resources/App.Resources.csproj", "App.Resources/"]
COPY ["Base.Contracts/Base.Contracts.csproj", "Base.Contracts/"]
COPY ["Base.Domain/Base.Domain.csproj", "Base.Domain/"]
COPY ["Base.Helpers/Base.Helpers.csproj", "Base.Helpers/"]
COPY ["App.Tests/App.Tests.csproj", "App.Tests/"]

RUN dotnet restore "WebApp/WebApp.csproj"

COPY . .
RUN dotnet publish "WebApp/WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]
