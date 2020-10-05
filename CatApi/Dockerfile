FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80 
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS build
WORKDIR /src
COPY ["CatApi/CatApi.csproj", "CatApi/"]
RUN dotnet restore "CatApi/CatApi.csproj"
COPY . .
WORKDIR "/src/CatApi"
RUN dotnet build "CatApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CatApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CatApi.dll"]