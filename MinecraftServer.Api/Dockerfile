FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
ENV TZ="America/Sao_Paulo"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MinecraftServer.Api/MinecraftServer.Api.csproj", "MinecraftServer.Api/"]
RUN dotnet restore "MinecraftServer.Api/MinecraftServer.Api.csproj"
COPY . .
WORKDIR "/src/MinecraftServer.Api"
RUN dotnet build "MinecraftServer.Api.csproj" -c Release -o /app/build -nowarn:CS8602,CS0168,CS8618


FROM build AS publish
RUN chmod +x Makefile
RUN bash -c ./Makefile
RUN cat appsettings.Production.json
RUN dotnet publish "MinecraftServer.Api.csproj" -c Release -o /app/publish -nowarn:CS8602,CS0168,CS8618

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinecraftServer.Api.dll"]
