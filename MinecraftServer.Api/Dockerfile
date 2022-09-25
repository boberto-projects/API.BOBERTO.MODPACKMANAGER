FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
ENV TZ="America/Sao_Paulo"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MinecraftServer.Api/MinecraftServer.Api.csproj", "MinecraftServer.Api/"]
RUN dotnet restore "MinecraftServer.Api/MinecraftServer.Api.csproj"
RUN chmod +x ./MinecraftServer.Api/Makefile && bash ./MinecraftServer.Api/Makefile
COPY . .
WORKDIR "/src/MinecraftServer.Api"
RUN dotnet build "MinecraftServer.Api.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "MinecraftServer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinecraftServer.Api.dll"]
