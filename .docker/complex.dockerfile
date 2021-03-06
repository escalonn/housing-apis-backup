# housing_apis :: complex_dockerfile

## arguments
ARG DOTNET_VERSION=3.1

## stage - restore
FROM mcr.microsoft.com/dotnet/core/sdk:${DOTNET_VERSION} as restore
WORKDIR /src
COPY src/Revature.Complex.Api/*.csproj Revature.Complex.Api/
COPY src/Revature.Complex.DataAccess/*.csproj Revature.Complex.DataAccess/
COPY src/Revature.Complex.Lib/*.csproj Revature.Complex.Lib/
RUN dotnet restore *.Api

## stage - publish
FROM restore as publish
COPY src/Revature.Complex.Api/ Revature.Complex.Api/
COPY src/Revature.Complex.DataAccess/ Revature.Complex.DataAccess/
COPY src/Revature.Complex.Lib/ Revature.Complex.Lib/
RUN dotnet publish *.Api --configuration Release --no-restore --output /src/dist

## stage - deploy
FROM mcr.microsoft.com/dotnet/core/aspnet:${DOTNET_VERSION} as deploy
WORKDIR /api
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=publish /src/dist/ ./
CMD dotnet *.Api.dll
