FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

COPY . ./

RUN dotnet restore

ARG PROJECT_NAME

RUN dotnet build ${PROJECT_NAME} -c Release -o /App/out --no-restore

RUN dotnet publish ${PROJECT_NAME} -c Release -o /App/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

ARG DLL

ENV DLL=${DLL}

ENTRYPOINT dotnet ${DLL}
