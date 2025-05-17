FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
ARG RELEASE_VERSION
ARG BUILD_CONFIGURATION=Release
WORKDIR /sln

COPY ./*.sln ./
COPY src/Imget src/Imget
COPY src/Imget.Client src/Imget.Client
COPY src/Imget.DataAccess src/Imget.DataAccess
COPY src/Imget.DataAccess.Minio src/Imget.DataAccess.Minio
COPY src/Imget.DataAccess.Rmq src/Imget.DataAccess.Rmq
COPY src/Imget.Orchestrator src/Imget.Orchestrator

RUN dotnet restore -a $TARGETARCH

COPY ./src ./src
RUN dotnet build "./src/Imget/Imget.csproj" -c $BUILD_CONFIGURATION -a $TARGETARCH -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./src/Imget/Imget.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -p:VersionPrefix=$RELEASE_VERSION


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Imget.dll"]