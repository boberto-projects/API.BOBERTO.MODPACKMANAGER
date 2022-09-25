FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
ENV TZ="America/Sao_Paulo"
ARG disableWarnings=CS8602,CS0168,CS8618,CS8604,CS8603

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MinecraftServer.Api/MinecraftServer.Api.csproj", "MinecraftServer.Api/"]
RUN dotnet restore "MinecraftServer.Api/MinecraftServer.Api.csproj"
RUN chmod +x MinecraftServer.Api/Makefile
RUN ./MinecraftServer.Api/Makefile
RUN cat appsettings.Production.json
COPY . .
WORKDIR "/src/MinecraftServer.Api"
RUN dotnet build "MinecraftServer.Api.csproj" -c Release -o /app/build -nowarn:$disableWarnings


FROM build AS publish
RUN ls
RUN dotnet publish "MinecraftServer.Api.csproj" -c Release -o /app/publish -nowarn:$disableWarnings

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinecraftServer.Api.dll"]
