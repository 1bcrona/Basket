FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["Basket.API/Basket.API.csproj", "Basket.API/"]
COPY ["Basket.Caching/Basket.Caching.csproj", "Basket.Caching/"]
COPY ["Basket.Data/Basket.Data.csproj", "Basket.Data/"]
COPY ["Basket.IoC/Basket.IoC.csproj", "Basket.IoC/"]
COPY ["Basket.Library/Basket.Library.csproj", "Basket.Library/"]
COPY ["Basket.Logging/Basket.Logging.csproj", "Basket.Logging/"]
COPY ["Basket.Repository/Basket.Repository.csproj", "Basket.Repository/"]
COPY ["Basket.Service/Basket.Service.csproj", "Basket.Service/"]
COPY ["Basket.Test/Basket.Test.csproj", "Basket.Test/"]
RUN dotnet restore "Basket.API/Basket.API.csproj"
RUN dotnet restore "Basket.Test/Basket.Test.csproj"
COPY . .
WORKDIR "/src/Basket.API"
RUN dotnet build "Basket.API.csproj" -c Release -o /app/build

FROM build AS testrunner
WORKDIR "/src/Basket.Test"
CMD ["dotnet", "test"]

FROM build AS test
WORKDIR "/src/Basket.Test"
RUN dotnet test --logger:trx

FROM build AS publish
WORKDIR "/src/Basket.API"
RUN dotnet publish "Basket.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket.API.dll"]