#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app
EXPOSE 11000
EXPOSE 12000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Krka.Reports.EventHub/Krka.Reports.EventHub.csproj", "Krka.Reports.EventHub/"]
RUN dotnet restore "Krka.Reports.EventHub/Krka.Reports.EventHub.csproj"
COPY . .
WORKDIR "/src/Krka.Reports.EventHub"
RUN dotnet build "Krka.Reports.EventHub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Krka.Reports.EventHub.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Krka.Reports.EventHub.exe"]