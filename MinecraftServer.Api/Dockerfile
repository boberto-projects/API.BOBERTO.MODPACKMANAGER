FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
ENV TZ="America/Sao_Paulo"
#teste
#ARG disableWarnings=CS8602,CS0168,CS8618,CS8604,CS8603
ARG CONFIG_FILE=./MinecraftServer.Api/appsettings.Production.json

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MinecraftServer.Api/appsettings.Production.json","MinecraftServer.Api/Makefile","MinecraftServer.Api/MinecraftServer.Api.csproj", "MinecraftServer.Api/"]
RUN dotnet restore "MinecraftServer.Api/MinecraftServer.Api.csproj"
RUN echo arquivos antigos && cd MinecraftServer.Api/ && ls
RUN chmod +x MinecraftServer.Api/Makefile
RUN ./MinecraftServer.Api/Makefile
RUN cat echo $CONFIG_FILE
COPY . .
WORKDIR "/src/MinecraftServer.Api"
RUN dotnet build "MinecraftServer.Api.csproj" -c Release -o /app/build -nowarn:CS8602,CS0168,CS8618,CS8604,CS8603


FROM build AS publish
RUN ls
RUN dotnet publish "MinecraftServer.Api.csproj" -c Release -o /app/publish -nowarn:CS8602,CS0168,CS8618,CS8604,CS8603

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinecraftServer.Api.dll"]
