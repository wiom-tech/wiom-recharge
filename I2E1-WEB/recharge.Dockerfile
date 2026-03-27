FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./

RUN dotnet restore --source "https://api.nuget.org/v3/index.json" --source "http://nuget.i2e1.in:8000/v3/index.json"

WORKDIR /app/recharge
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
ARG ENVIRONMENT
ARG SERVER
ARG AWS_ACCESS_KEY
ARG AWS_SECRET_KEY

COPY --from=build-env /app/recharge/out .

ENV IS_AWS='true'
ENV DEPLOYED_ON=${ENVIRONMENT}
ENV SERVER=${SERVER}
ENV AWS_ACCESS_KEY=${AWS_ACCESS_KEY}
ENV AWS_SECRET_KEY=${AWS_SECRET_KEY}

ENTRYPOINT ["dotnet", "recharge.dll"]