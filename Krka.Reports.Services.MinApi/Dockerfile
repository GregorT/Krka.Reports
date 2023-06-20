#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Krka.Reports.Services.MinApi/Krka.Reports.Services.MinApi.csproj", "Krka.Reports.Services.MinApi/"]
RUN dotnet restore "Krka.Reports.Services.MinApi/Krka.Reports.Services.MinApi.csproj"
COPY . .
WORKDIR "/src/Krka.Reports.Services.MinApi"
RUN dotnet build "Krka.Reports.Services.MinApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Krka.Reports.Services.MinApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Krka.Reports.Services.MinApi.dll"]