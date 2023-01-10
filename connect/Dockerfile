FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 443
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["connect.csproj", "./"]
RUN dotnet restore "connect.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "connect.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "connect.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "connect.dll"]
