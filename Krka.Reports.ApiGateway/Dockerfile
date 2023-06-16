#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Krka.Reports.ApiGateway/Krka.Reports.ApiGateway.csproj", "Krka.Reports.ApiGateway/"]
RUN dotnet restore "Krka.Reports.ApiGateway/Krka.Reports.ApiGateway.csproj"
COPY . .
WORKDIR "/src/Krka.Reports.ApiGateway"
RUN dotnet build "Krka.Reports.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Krka.Reports.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Krka.Reports.ApiGateway.dll"]