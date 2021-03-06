# housing_apis :: account_dockerfile

## arguments
ARG DOTNET_VERSION=3.1

## stage - restore
FROM mcr.microsoft.com/dotnet/core/sdk:${DOTNET_VERSION} as restore
WORKDIR /src
COPY src/Revature.Account.Api/*.csproj Revature.Account.Api/
COPY src/Revature.Account.DataAccess/*.csproj Revature.Account.DataAccess/
COPY src/Revature.Account.Lib/*.csproj Revature.Account.Lib/
RUN dotnet restore *.Api

## stage - publish
FROM restore as publish
COPY src/Revature.Account.Api/ Revature.Account.Api/
COPY src/Revature.Account.DataAccess/ Revature.Account.DataAccess/
COPY src/Revature.Account.Lib/ Revature.Account.Lib/
RUN dotnet publish *.Api --configuration Release --no-restore --output /src/dist

## stage - deploy
FROM mcr.microsoft.com/dotnet/core/aspnet:${DOTNET_VERSION} as deploy
WORKDIR /api
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=publish /src/dist/ ./
CMD dotnet *.Api.dll
