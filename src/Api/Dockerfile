 #See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:latest AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /src
COPY ["BasketBallStats.csproj", "."]
RUN dotnet restore "BasketBallStats.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "BasketBallStats.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BasketBallStats.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasketBallStats.dll"]
