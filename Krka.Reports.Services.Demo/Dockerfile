#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Krka.Reports.Services.Demo/Krka.Reports.Services.Demo.csproj", "Krka.Reports.Services.Demo/"]
RUN dotnet restore "Krka.Reports.Services.Demo/Krka.Reports.Services.Demo.csproj"
COPY . .
WORKDIR "/src/Krka.Reports.Services.Demo"
RUN dotnet build "Krka.Reports.Services.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Krka.Reports.Services.Demo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Krka.Reports.Services.Demo.dll"]