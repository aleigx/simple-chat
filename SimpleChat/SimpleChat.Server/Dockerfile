FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY SimpleChat.Server/SimpleChat.Server.csproj ./SimpleChat.Server/
COPY SimpleChat.Common/SimpleChat.Common.csproj ./SimpleChat.Common/
RUN dotnet restore SimpleChat.Server/SimpleChat.Server.csproj

COPY SimpleChat.Server/. ./SimpleChat.Server/
COPY SimpleChat.Common/. ./SimpleChat.Common/
WORKDIR /app/SimpleChat.Server
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/SimpleChat.Server/out ./

EXPOSE 9898

ENTRYPOINT ["dotnet", "SimpleChat.Server.dll"]
